using LiNGS.Common.GameCycle;
using LiNGS.Common.Network;
using LiNGS.Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Server.Aggregator
{
    internal class MessageAggregator : IUpdatable
    {
        private LiNGSServer server;
        private Dictionary<NetworkClient, BufferedNetworkMessage> bufferedMessages;
        private List<BufferedNetworkMessage> messagesToSend;

        public MessageAggregator(LiNGSServer server)
        {
            this.server = server;
            this.bufferedMessages = new Dictionary<NetworkClient, BufferedNetworkMessage>();
            this.messagesToSend = new List<BufferedNetworkMessage>();
        }

        public void BufferMessage(NetworkClient client, NetworkMessage message)
        {
            if (!bufferedMessages.ContainsKey(client))
            {
                bufferedMessages.Add(client, new BufferedNetworkMessage() { Destination = client });
            }

            if (bufferedMessages[client].Message.Length + message.Length > server.ServerProperties.MaxMessageDataSize)
            {
                messagesToSend.Add(bufferedMessages[client]);
                bufferedMessages[client] = new BufferedNetworkMessage() { Destination = client };
            }

            bufferedMessages[client].AppendMessageData(message.Data);
            
        }

        #region IUpdatable Members

        public void Update(TimeSpan timeSinceLastUpdate)
        {
            List<NetworkClient> keys = bufferedMessages.Keys.ToList();
            foreach (var key in keys)
            {
                BufferedNetworkMessage bMessage = bufferedMessages[key];
                if (DateTime.Now - bMessage.FirstMessageDataTime > bMessage.MaxBufferTime - timeSinceLastUpdate)
                {
                    messagesToSend.Add(bMessage);
                    bufferedMessages.Remove(key);
                }
            }

            foreach (var message in messagesToSend)
            {
                (message.Destination as InternalNetworkClient).LastSentConnectionTime = DateTime.Now;
                server.NetworkManager.SendMessage(message.Destination, message.Message);
            }

            messagesToSend.Clear();
        }

        #endregion

    }
}
