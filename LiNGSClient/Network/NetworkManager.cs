using LiNGS.Common;
using LiNGS.Common.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LiNGS.Client.Network
{
    internal class NetworkManager
    {
        private LiNGSClient client;
        private Socket clientSocket;
        private byte[] receivedData;
        private EndPoint epServer;

        internal NetworkManager(LiNGSClient client, string serverIp, int serverPort)
        {
            this.client = client;
            this.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.receivedData = new byte[client.ClientProperties.MaxMessageSize]; //Maximum message size is defaulted to 512 to avoid network fragmentation, see: RFC 1122 Section 3.3.2

            IPAddress serverIpAddress = IPAddress.Parse(serverIp);
            IPEndPoint serverEndPoint = new IPEndPoint(serverIpAddress, serverPort);
            epServer = (EndPoint)serverEndPoint;
        }

        private void OnReceive(IAsyncResult result)
        {
            clientSocket.EndReceiveFrom(result, ref epServer);

            NetworkMessage message = null;
            try
            {
                message = new NetworkMessage(receivedData, epServer);
            }
            catch (Exception)
            {
            }

            if (message != null)
            {
                client.Router.RouteMessage(message);
            }
            

            clientSocket.BeginReceiveFrom(receivedData, 0, receivedData.Length, SocketFlags.None, ref epServer, new AsyncCallback(OnReceive), epServer);
        }

        private void OnSend(IAsyncResult result)
        {
            clientSocket.EndSendTo(result);
        }


        internal void SendMessage(EndPoint destination, NetworkMessage message)
        {
            byte[] messageData = message.Serialize();
            clientSocket.BeginSendTo(messageData, 0, messageData.Length, SocketFlags.None, destination, new AsyncCallback(OnSend), destination);
        }

        internal void SendMessage(NetworkClient client, NetworkMessage message)
        {
            SendMessage(client.EndPoint, message);
        }

        internal void SendMessage(NetworkMessage message)
        {
            SendMessage(epServer, message);
        }

        internal void Connect()
        {
            NetworkMessage connectMessage = null;

            if (client.ClientStatus.SessionUserId != null)
            {
                connectMessage = new NetworkMessage(NetworkMessage.MessageType.Connect, new MessageData() { Object = LiNGSMarkers.SessionUserId, Value = client.ClientStatus.SessionUserId });
            }
            else
            {
                connectMessage = new NetworkMessage(NetworkMessage.MessageType.Connect);
            }
            
            byte[] messageData = connectMessage.Serialize();
            clientSocket.BeginSendTo(messageData, 0, messageData.Length, SocketFlags.None, epServer, new AsyncCallback(OnSend), null);
            clientSocket.BeginReceiveFrom(receivedData, 0, receivedData.Length, SocketFlags.None, ref epServer, new AsyncCallback(OnReceive), epServer);

            client.ClientStatus.EndPoint = clientSocket.LocalEndPoint as IPEndPoint;
        }

        internal void Disconnect()
        {
            NetworkMessage connectMessage = new NetworkMessage(NetworkMessage.MessageType.Disconnect);
            byte[] messageData = connectMessage.Serialize();
            clientSocket.BeginSendTo(messageData, 0, messageData.Length, SocketFlags.None, epServer, new AsyncCallback(OnSend), null);
        }

    }
}
