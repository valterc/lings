using LiNGS.Common;
using LiNGS.Common.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LiNGS.Client
{
    internal class Router
    {
        private LiNGSClient client;

        public Router(LiNGSClient client)
        {
            this.client = client;
        }

        public void RouteMessage(NetworkMessage message)
        {
            //Filter invalid messages
            if (message == null || (message.Type != NetworkMessage.MessageType.Connect && !client.ClientStatus.Connected))
            {
                return;
            }

            /*
            Debug.WriteLine(message.ToString());

            foreach (var item in message.Data)
            {
                Debug.WriteLine(item.ToString());
            }
            */            

            switch (message.Type)
            {
                case NetworkMessage.MessageType.Connect:
                    client.Manager.ConnectionAccepted(message);
                    break;
                case NetworkMessage.MessageType.Disconnect:
                    client.Manager.Disconnect(message);
                    break;
                case NetworkMessage.MessageType.Ack:
                    client.Manager.ServerConnection(message);
                    break;
                case NetworkMessage.MessageType.Data:
                    if (ValidateConnection(message))
                    {
                        client.Manager.ServerConnection(message);
                        client.ClientLogicProcessor.ReceiveDataMessage(message);
                    }
                    break;
                case NetworkMessage.MessageType.Game:
                    if (ValidateConnection(message))
                    {
                        client.Manager.ServerConnection(message);
                        client.ClientLogicProcessor.ReceiveGameMessage(message);
                    }
                    break;
                case NetworkMessage.MessageType.Event:
                    if (ValidateConnection(message))
                    {
                        client.Manager.ServerConnection(message);
                        client.ClientLogicProcessor.ReceiveEventMessage(message);
                    }
                    break;
                case NetworkMessage.MessageType.ErrorConnect:
                    client.Manager.ConnectionRefused(message);
                    break;
                case NetworkMessage.MessageType.Error:
                    if (ValidateConnection(message))
                    {
                        client.Manager.ServerConnection(message);
                        client.ClientLogicProcessor.ReceiveErrorMessage(message);
                    }
                    break;
                case NetworkMessage.MessageType.Heartbeat:
                    client.Manager.ServerConnection(message);
                    break;
            }
        }

        /// <summary>
        /// Checks if a received message is thrustworthy
        /// </summary>
        /// <param name="message">Received message</param>
        /// <returns>True if the message came from the known server, False otherwise</returns>
        private bool ValidateConnection(NetworkMessage message)
        {
            return client.Manager.Server != null && client.Manager.Server.EndPoint.Equals(message.From);
        }

    }
}
