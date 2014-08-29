using LiNGS.Common;
using LiNGS.Common.GameCycle;
using LiNGS.Common.Network;
using LiNGS.Server.GameLogic;
using LiNGS.Server.Network;
using LiNGS.Server.State;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace LiNGS.Server.Management
{
    internal class Manager : IUpdatable
    {
        private LiNGSServer server;
        private int maxClients;
        private List<ImportantMessageWrapper> importantMessages;
        private List<GameClient> disconnectingClients;
        private List<NetworkMessage> connectingClients;

        internal List<GameClient> ConnectedClients { get; private set; }

        internal Manager(LiNGSServer server, int maxClients)
        {
            this.server = server;
            this.maxClients = maxClients;
            this.importantMessages = new List<ImportantMessageWrapper>();
            this.disconnectingClients = new List<GameClient>();
            this.connectingClients = new List<NetworkMessage>();

            this.ConnectedClients = new List<GameClient>();
        }

        internal void ClientConnecting(NetworkMessage message)
        {
            lock (connectingClients)
            {
                connectingClients.Add(message);
            }
        }

        internal void ClientConnection(NetworkMessage message)
        {
            GameClient client = GetClient(message.From);
            if (client != null)
            {
                InternalNetworkClient internalNetworkClient = client.NetworkClient as InternalNetworkClient;
                internalNetworkClient.LastReceivedConnectionTime = DateTime.Now;

                switch (message.Type)
                {
                    case NetworkMessage.MessageType.Ack:

                        if (message.Data.FirstOrDefault(d => d.Object == LiNGSMarkers.Id) != null && message.Data.FirstOrDefault(d => d.Object == LiNGSMarkers.Id).Value != null)
                        {
                            int messageId = int.Parse(message.Data.FirstOrDefault(d => d.Object == LiNGSMarkers.Id).Value);

                            IEnumerable<ImportantMessageWrapper> messages;
                            lock (this.importantMessages)
                            {
                                messages = this.importantMessages.Where(im => im.Client == client.NetworkClient && im.Message.MessageId == messageId).ToList();
                            }

                            foreach (var item in messages)
                            {
                                item.ConfirmationReceived = true;
                                item.Message.ConfirmReception();

                                //Calculate Latency based on message travel time
                                internalNetworkClient.Latency = TimeSpan.FromTicks((DateTime.Now - item.SentDate).Ticks / 2);

                                //Reset heartbeat sent flag
                                if (item.Message.Type == NetworkMessage.MessageType.Heartbeat)
                                {
                                    internalNetworkClient.HeartbeatSent = false;
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
                    server.NetworkManager.SendMessage(message.From, new NetworkMessage(NetworkMessage.MessageType.Ack, new MessageData() { Object = LiNGSMarkers.Id, Value = message.MessageId.ToString() }));
                }

            }
        }

        internal void ClientDisconnecting(NetworkMessage message)
        {
            GameClient client = GetClient(message.From);
            if (client != null)
            {
                lock (disconnectingClients)
                {
                    disconnectingClients.Add(client);
                }
            }
        }

        internal void SendMessage(NetworkClient client, NetworkMessage message)
        {
            if (message.Length > server.ServerProperties.MaxMessageSize)
            {
                SplitAndSendMessage(client, message);
                return;
            }

            if (message.NeedsAck)
            {
                if (message.MessageId == 0)
                {
                    message.MessageId = client.MessageId;
                }

                lock (this.importantMessages)
                {
                    this.importantMessages.Add(new ImportantMessageWrapper() { Client = client, Message = message, SentDate = DateTime.Now });
                }

                (client as InternalNetworkClient).LastSentConnectionTime = DateTime.Now;
                server.NetworkManager.SendMessage(client.EndPoint, message);
            }
            else
            {
                //server.NetworkManager.SendMessage(client.EndPoint, message);
                server.MessageAggregator.BufferMessage(client, message);
            }
        }

        internal GameClient GetClient(EndPoint client)
        {
            lock (ConnectedClients)
            {
                return ConnectedClients.FirstOrDefault(c => c.NetworkClient.EndPoint.Equals(client));
            }
        }

        internal void DisconnectClient(GameClient client, String reason = null)
        {
            if (client == null)
            {
                return;
            }

            bool clientExists = true;
            lock (ConnectedClients)
            {
               clientExists = ConnectedClients.Contains(client);
            }

            if (!clientExists)
            {
                return;
            }

            NetworkMessage message = new NetworkMessage(NetworkMessage.MessageType.Disconnect);
            MessageData data = new MessageData() { Object = LiNGSMarkers.Reason, Value = reason ?? "Connection closed" };
            SendMessage(client.NetworkClient, message);

            server.StateManager.RemoveClient(client);
            server.GameLogicProcessor.RemoveClient(client);

            lock (this.importantMessages)
            {
                this.importantMessages.RemoveAll(ip => ip.Client.Equals(client.NetworkClient));
            }

            lock (ConnectedClients)
            {
                ConnectedClients.Remove(client); 
            }
        }

        internal void Shutdown()
        {
            lock (connectingClients)
            {
                connectingClients.Clear();
            }

            lock (disconnectingClients)
            {
                disconnectingClients.Clear();
            }

            lock (ConnectedClients)
            {
                foreach (var client in ConnectedClients)
                {
                    NetworkMessage message = new NetworkMessage(NetworkMessage.MessageType.Disconnect);
                    MessageData data = new MessageData() { Object = LiNGSMarkers.Reason, Value = "Server Shutdown" };
                    SendMessage(client.NetworkClient, message);

                    server.StateManager.RemoveClient(client);
                    server.GameLogicProcessor.RemoveClient(client);
                    this.importantMessages.RemoveAll(ip => ip.Client.Equals(client.NetworkClient));
                }

                ConnectedClients.Clear();
            }
        }

        private void SplitAndSendMessage(NetworkClient client, NetworkMessage message)
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
                if (nt.Length + dataToSend[0].Length + 4 < server.ServerProperties.MaxMessageSize)
                {
                    nt.Data.Add(dataToSend[0]);
                    dataToSend.RemoveAt(0);
                }
                else if (dataToSend[0].Length + 4 > server.ServerProperties.MaxMessageDataSize)
                {
                    //Well, this piece of data is too big to send
                    dataToSend.RemoveAt(0);
                }
                else
                {
                    SendMessage(client, nt);
                    nt = new NetworkMessage();
                    nt.NeedsAck = message.NeedsAck;
                    nt.Type = message.Type;
                    nt.OnReceived = message.OnReceived;
                    nt.MessageId = 0;
                }
            } while (dataToSend.Count != 0);

            if (nt.Data.Count > 0)
            {
                SendMessage(client, nt);
            }

        }

        private void HandleDisconnectingClient(GameClient client)
        {
            bool saveState = server.GameLogicProcessor.ClientDisconnected(client);
            if (saveState)
            {
                server.StateManager.PersistClientState(client);
            }

            server.StateManager.RemoveClient(client);
            server.GameLogicProcessor.RemoveClient(client);
            this.importantMessages.RemoveAll(ip => ip.Client.Equals(client.NetworkClient));

            lock (ConnectedClients)
            {
                ConnectedClients.Remove(client);
            }
        }

        private void HandleConnectingClient(NetworkMessage message)
        {
            lock (ConnectedClients)
            {
                if (ConnectedClients.Count < maxClients)
                {

                    //Check if the user sent a valid sessionString
                    //If so then the user was already on this game and his session may be cached on the server

                    string sessionString = null;
                    MessageData sessionData = message.Data.FirstOrDefault(d => d.Object == LiNGSMarkers.SessionUserId);
                    if (sessionData != null)
                    {
                        sessionString = sessionData.Value;
                    }

                    GameClient client = null;
                    if (sessionString != null)
                    {
                        try
                        {
                            string session = sessionString.Split(LiNGSMarkers.Separator[0])[0];
                            if (session == GameClient.SessionGUID.ToString())
                            {
                                client = new GameClient(sessionString);
                            }
                            else
                            {
                                client = new GameClient();
                            }
                            
                        }
                        catch (Exception)
                        {
                            client = new GameClient();
                        }
                    }
                    else
                    {
                        client = new GameClient();
                    }

                    client.NetworkClient = new InternalNetworkClient { EndPoint = message.From };

                    bool hasSavedState = server.StateManager.HasClientState(client);

                    ClientConnectionResponse connectionResponse = server.GameLogicProcessor.AcceptClient(client, message, hasSavedState);

                    if (connectionResponse == null)
                    {
                        server.NetworkManager.SendMessage(message.From, new NetworkMessage(NetworkMessage.MessageType.ErrorConnect, new MessageData { Object = LiNGSMarkers.Error, Value = "Connection rejected" }));
                        return;
                    }

                    if (connectionResponse.Accept)
                    {
                        ConnectedClients.Add(client);
                        bool usingSavedState = server.StateManager.AddClient(client, connectionResponse.UseSavedState && hasSavedState);

                        NetworkMessage responseMessage = new NetworkMessage(NetworkMessage.MessageType.Connect,
                                                                            new MessageData { Object = LiNGSMarkers.Ok, Value = "Connection Accepted" },
                                                                            new MessageData { Object = LiNGSMarkers.SessionUserId, Value = client.SessionUserId },
                                                                            new MessageData { Object = LiNGSMarkers.UsingSavedState, Value = usingSavedState.ToString() }
                                                                            );
                        responseMessage.NeedsAck = true;
                        SendMessage(client.NetworkClient, responseMessage);
                    }
                    else
                    {
                        server.NetworkManager.SendMessage(message.From, new NetworkMessage(NetworkMessage.MessageType.ErrorConnect, new MessageData { Object = LiNGSMarkers.Error, Value = connectionResponse.RefuseMessage ?? "Connection rejected" }));
                    }

                }
                else
                {
                    server.NetworkManager.SendMessage(message.From, new NetworkMessage(NetworkMessage.MessageType.ErrorConnect, new MessageData { Object = LiNGSMarkers.Error, Value = "Server is full" }));
                }
            }
        }

        #region IUpdatable Members

        public void Update(TimeSpan timeSinceLastUpdate)
        {
            IEnumerable<ImportantMessageWrapper> messages = null;
            lock (this.importantMessages)
            {
                messages = this.importantMessages.ToList();
            }

            foreach (var item in messages)
            {
                if (!item.ConfirmationReceived)
                {
                    if (DateTime.Now - item.SentDate > TimeSpan.FromMilliseconds(server.ServerProperties.ImportantMessageTimeout))
                    {
                        //Resend message
                        item.SentDate = DateTime.Now;
                        item.Retries++;

                        if (item.Retries > server.ServerProperties.MaxImportantMessageRetries)
                        {
                            DisconnectClient(GetClient(item.Client.EndPoint), "Bad Connection");
                        }
                        else
                        {
                            (item.Client as InternalNetworkClient).LastSentConnectionTime = DateTime.Now;
                            server.NetworkManager.SendMessage(item.Client.EndPoint, item.Message);
                        }
                    }
                }
            }

            

            lock (connectingClients)
            {
                foreach (var clientMessage in connectingClients)
                {
                    HandleConnectingClient(clientMessage);
                }
                connectingClients.Clear();
            }

            lock (disconnectingClients)
            {
                foreach (var client in disconnectingClients)
	            {
                    HandleDisconnectingClient(client);
	            }
                disconnectingClients.Clear();
            }
        }

        #endregion

    }
}
