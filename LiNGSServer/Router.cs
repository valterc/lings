using LiNGS.Common.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LiNGS.Server
{
    internal class Router
    {
        private LiNGSServer server;

        public Router(LiNGSServer server)
        {
            this.server = server;
        }

        public void RouteMessage(NetworkMessage message)
        {
            if (message == null)
            {
                return;
            }

            switch (message.Type)
            {
                case NetworkMessage.MessageType.Connect:
                    server.Manager.ClientConnecting(message);
                    break;
                case NetworkMessage.MessageType.Disconnect:
                    server.Manager.ClientDisconnecting(message);
                    break;
                case NetworkMessage.MessageType.Ack:
                    server.Manager.ClientConnection(message);
                    break;
                case NetworkMessage.MessageType.Data:
                    server.Manager.ClientConnection(message);
                    server.GameLogicProcessor.ReceiveDataMessage(message);
                    break;
                case NetworkMessage.MessageType.Game:
                    server.Manager.ClientConnection(message);
                    server.GameLogicProcessor.ReceiveGameMessage(message);
                    break;
                case NetworkMessage.MessageType.Event:
                    server.Manager.ClientConnection(message);
                    server.GameLogicProcessor.ReceiveEventMessage(message);
                    break;
                case NetworkMessage.MessageType.ErrorConnect:
                    server.Manager.ClientConnection(message);
                    break;
                case NetworkMessage.MessageType.Error:
                    server.Manager.ClientConnection(message);
                    server.GameLogicProcessor.ReceiveErrorMessage(message);
                    break;
                case NetworkMessage.MessageType.Heartbeat:
                    server.Manager.ClientConnection(message);
                    break;
            }
        }

    }
}
