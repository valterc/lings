using LiNGS.Client;
using LiNGS.Common.GameCycle;
using LiNGS.Common.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Client.Aggregator
{
    internal class MessageAggregator : IUpdatable
    {
        private LiNGSClient client;
        private Dictionary<NetworkClient, BufferedNetworkMessage> bufferedMessages;
        private List<BufferedNetworkMessage> messagesToSend;

        public MessageAggregator(LiNGSClient client)
        {
            this.client = client;
            this.bufferedMessages = new Dictionary<NetworkClient, BufferedNetworkMessage>();
            this.messagesToSend = new List<BufferedNetworkMessage>();
        }

        public void BufferMessage(NetworkClient client, NetworkMessage message)
        {
            if (!bufferedMessages.ContainsKey(client))
            {
                bufferedMessages.Add(client, new BufferedNetworkMessage() { Destination = client });
            }

            if (bufferedMessages[client].Message.Length + message.Length > this.client.ClientProperties.MaxMessageDataSize)
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
                client.NetworkManager.SendMessage(message.Destination, message.Message);
            }

            messagesToSend.Clear();
        }

        #endregion

    }
}
