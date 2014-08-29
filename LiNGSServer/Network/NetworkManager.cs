using LiNGS.Common.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LiNGS.Server.Network
{
    internal class NetworkManager
    {
        private LiNGSServer server;
        private Socket serverSocket;
        private IPEndPoint endPoint;
        private byte[] receivedData;

        internal NetworkManager(LiNGSServer server, int listenPort)
        {
            this.server = server;
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.endPoint = new IPEndPoint(IPAddress.Any, listenPort);
            this.receivedData = new byte[server.ServerProperties.MaxMessageSize]; //Maximum message size is defaulted to 512 bytes to avoid network fragmentation, see: RFC 1122 Section 3.3.2

            this.serverSocket.Bind(endPoint);

            IPEndPoint senderEndPoint = new IPEndPoint(IPAddress.Any, 0);
            EndPoint epSender = (EndPoint)senderEndPoint;

            this.serverSocket.BeginReceiveFrom(receivedData, 0, receivedData.Length, SocketFlags.None, ref epSender, new AsyncCallback(OnReceive), epSender);  
        }

        private void OnReceive(IAsyncResult result)
        {
            IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint epSender = (EndPoint)ipeSender;

            try
            {
                serverSocket.EndReceiveFrom(result, ref epSender);
            }
            catch (Exception)
            {
                //TODO: Client socket probably closed, disconnected client
                return;
            }

            NetworkMessage message = null;

            //Ignore the received data if that is not a valid message
            try
            {
                message = new NetworkMessage(receivedData, epSender);
            }
            catch (Exception)
            {
                //TODO: Create a log with all events, warnings and errors
            }

            if (message != null)
            {
                server.Router.RouteMessage(message);
            }

            serverSocket.BeginReceiveFrom(receivedData, 0, receivedData.Length, SocketFlags.None, ref epSender, new AsyncCallback(OnReceive), epSender);  
        }

        private void OnSend(IAsyncResult result)
        {
            serverSocket.EndSendTo(result);
        }

        
        internal void SendMessage(EndPoint destination, NetworkMessage message)
        {
            byte[] messageData = message.Serialize();
            serverSocket.BeginSendTo(messageData, 0, messageData.Length, SocketFlags.None, destination, new AsyncCallback(OnSend), destination);
        }

        internal void SendMessage(NetworkClient client, NetworkMessage message)
        {
            SendMessage(client.EndPoint, message);
        }

        internal void Shutdown()
        {
            serverSocket.Close();
        }

    }
}
