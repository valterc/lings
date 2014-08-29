using LiNGS.Client.Network;
using LiNGS.Common;
using LiNGS.Common.GameCycle;
using LiNGS.Common.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LiNGS.Client.Management
{
    internal class Manager : IUpdatable
    {
        private LiNGSClient client;
        private List<ImportantMessageWrapper> importantMessages;
        internal ConnectionEstablisherHelper ConnectionEstablisherHelper { get; private set; }

        internal NetworkClient Server { get; private set; }

        internal Manager(LiNGSClient client)
        {
            this.client = client;
            this.importantMessages = new List<ImportantMessageWrapper>();
            this.ConnectionEstablisherHelper = new ConnectionEstablisherHelper(client);

            client.UpdateManager.AddUpdatable(ConnectionEstablisherHelper);
        }

        internal void Connect()
        {
            this.ConnectionEstablisherHelper.Connect();
        }

        internal void Disconnect(NetworkMessage message)
        {
            MessageData data = message.Data.FirstOrDefault(d => d.Object == LiNGSMarkers.Reason);
            if (data != null)
	        {
                Disconnect(data.Value);
            }
            else
            {
                Disconnect("Server closed connection.");
            }
        }

        internal void Disconnect(String reason = null)
        {
            client.NetworkManager.Disconnect();
            client.ClientStatus.Connected = false;
            client.NetworkedClientInstance.Disconnected(reason);
        }

        internal void ConnectionAccepted(NetworkMessage message)
        {
            if (client.ClientStatus.Connected || !ConnectionEstablisherHelper.Connecting)
            {
                return;
            }

            ConnectionEstablisherHelper.ConnectEnd();

            if (message.Data.Any(d => d.Object == LiNGSMarkers.Ok) && message.Data.Any(d => d.Object == LiNGSMarkers.SessionUserId))
            {
                MessageData md = message.Data.FirstOrDefault(d => d.Object == LiNGSMarkers.UsingSavedState);
                bool usingSavedState = (md != null && md.Value == true.ToString());

                //Connection to server was successfull
                Server = new InternalNetworkClient() { EndPoint = message.From };
                client.ClientStatus.SessionUserId = message.Data.First(d => d.Object == LiNGSMarkers.SessionUserId).Value;
                client.ClientStatus.Connected = true;
                client.ClientStatus.WasConnected = true;
                client.NetworkManager.SendMessage(new NetworkMessage(LiNGS.Common.Network.NetworkMessage.MessageType.Ack, new MessageData() { Object = LiNGSMarkers.Id, Value = message.MessageId.ToString() }));
                client.NetworkedClientInstance.ConnectionAccepted(message, usingSavedState);
            }
        }

        internal void ConnectionRefused(NetworkMessage message)
        {
            ConnectionEstablisherHelper.ConnectEnd();
            client.NetworkedClientInstance.ConnectionRefused(message);
        }

        internal void UnableToConnect(String reason = null)
        {
            ConnectionEstablisherHelper.ConnectEnd();
            client.NetworkedClientInstance.UnableToConnect(reason);
        }

        internal void ServerConnection(NetworkMessage message)
        {
            if (!client.ClientStatus.Connected || Server == null || !Server.EndPoint.Equals(message.From))
            {
                //If the client is not connected to a server or if the message sender is not the server then ignore the message
                return;
            }

            InternalNetworkClient serverNetworkClient = Server as InternalNetworkClient;
            serverNetworkClient.LastReceivedConnectionTime = DateTime.Now;

            switch (message.Type)
            {
                case NetworkMessage.MessageType.Ack:

                    if (message.Data.FirstOrDefault(d => d.Object == LiNGSMarkers.Id) != null && message.Data.FirstOrDefault(d => d.Object == LiNGSMarkers.Id).Value != null)
                    {
                        int messageId = int.Parse(message.Data.FirstOrDefault(d => d.Object == LiNGSMarkers.Id).Value);

                        IEnumerable<ImportantMessageWrapper> messages;
                        lock (this.importantMessages)
                        {
                            messages = this.importantMessages.Where(im => im.Message.MessageId == messageId).ToList();
                        }

                        foreach (var item in messages)
                        {
                            item.ConfirmationReceived = true;
                            item.Message.ConfirmReception();

                            //Calculate Latency based on message travel time
                            serverNetworkClient.Latency = TimeSpan.FromTicks((DateTime.Now - item.SentDate).Ticks / 2);

                            //Reset heartbeat sent flag
                            if (item.Message.Type == NetworkMessage.MessageType.Heartbeat)
                            {
                                serverNetworkClient.HeartbeatSent = false;
                            }

                            lock (this.importantMessages)
                            {
                                this.importantMessages.Remove(item);
                            }
                        }

                    }

                    break;
            }

            if (message.NeedsAck)
            {
                client.NetworkManager.SendMessage(message.From, new NetworkMessage(NetworkMessage.MessageType.Ack, new MessageData() { Object = LiNGSMarkers.Id, Value = message.MessageId.ToString() }));
            }

        }

        internal void SendMessage(NetworkMessage message)
        {
            if (!client.ClientStatus.Connected)
            {
                return;
            }

            if (message.Length > client.ClientProperties.MaxMessageSize)
            {
                SplitAndSendMessage(message);
                return;
            }

            if (message.NeedsAck)
            {
                if (message.MessageId == 0)
                {
                    message.MessageId = Server.MessageId;
                }
                importantMessages.Add(new ImportantMessageWrapper() { Message = message, SentDate = DateTime.Now });
                client.NetworkManager.SendMessage(Server, message);
            }
            else
            {
                client.MessageAggregator.BufferMessage(Server, message);
                //client.NetworkManager.SendMessage(Server, message);
            }
        }

        private void SplitAndSendMessage(NetworkMessage message)
        {
            NetworkMessage nt = new NetworkMessage();
            nt.NeedsAck = message.NeedsAck;
            nt.Type = message.Type;
            nt.OnReceived = message.OnReceived;
            nt.MessageId = message.MessageId;

            List<MessageData> dataToSend = new List<MessageData>();

            //Cheap ordering, lets send all the LiNGS stuff before sending any other data.
            //This is done because if an object is to be created then the order to create the said object needs to arrive first 
            //than the data for the object itself
            dataToSend.AddRange(message.Data.Where(md => md.Object.StartsWith(LiNGSMarkers.Namespace) && (md.Property ?? LiNGSMarkers.Namespace).StartsWith(LiNGSMarkers.Namespace)));
            dataToSend.AddRange(message.Data.Where(md => !md.Object.StartsWith(LiNGSMarkers.Namespace) || !(md.Property ?? String.Empty).StartsWith(LiNGSMarkers.Namespace)));

            do
            {
                if (nt.Length + dataToSend[0].Length + 4 < client.ClientProperties.MaxMessageSize)
                {
                    nt.Data.Add(dataToSend[0]);
                    dataToSend.RemoveAt(0);
                }
                else if (dataToSend[0].Length + 4 > client.ClientProperties.MaxMessageDataSize)
                {
                    //Well, this piece of data is too big to send
                    dataToSend.RemoveAt(0);
                }
                else
                {
                    SendMessage(nt);
                    nt = new NetworkMessage();
                    nt.NeedsAck = message.NeedsAck;
                    nt.Type = message.Type;
                    nt.OnReceived = message.OnReceived;
                    nt.MessageId = 0;
                }
            } while (dataToSend.Count != 0);

            if (nt.Data.Count > 0)
            {
                SendMessage(nt);
            }
        }


        #region IUpdatable Members

        public void Update(TimeSpan timeSinceLastUpdate)
        {
            lock (this.importantMessages)
            {
                foreach (var item in importantMessages)
                {
                    if (!item.ConfirmationReceived)
                    {
                        if (DateTime.Now - item.SentDate > TimeSpan.FromMilliseconds(client.ClientProperties.ImportantMessageTimeout))
                        {
                            //Resend message
                            item.SentDate = DateTime.Now;
                            item.Retries++;

                            if (item.Retries > client.ClientProperties.MaxImportantMessageRetries)
                            {
                                Disconnect("Bad Connection");
                            }
                            else
                            {
                                client.NetworkManager.SendMessage(Server, item.Message);
                            }

                        }
                    }
                }
            }
        }

        #endregion

    }
}
