using LiNGS.Common.Network;
using LiNGS.Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Server.Aggregator
{
    internal class BufferedNetworkMessage
    {
        public NetworkMessage Message { get; set; }
        public DateTime FirstMessageDataTime { get; set; }
        public DateTime LastMessageDataTime { get; set; }
        public TimeSpan MaxBufferTime { get; set; }
        public NetworkClient Destination { get; set; }

        public BufferedNetworkMessage() : this(TimeSpan.FromMilliseconds(10))
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="BufferedNetworkMessage"/>
        /// </summary>
        /// <param name="timeout">The maximum time that the message can be hold in the buffer</param>
        public BufferedNetworkMessage(TimeSpan timeout)
        {
            this.Message = new NetworkMessage(LiNGS.Common.Network.NetworkMessage.MessageType.Data);
            this.MaxBufferTime = timeout;
            this.FirstMessageDataTime = DateTime.Now;
        }

        public void AppendMessageData(MessageData data)
        {
            this.Message.Data.Add(data);
            this.LastMessageDataTime = DateTime.Now;
        }

        public void AppendMessageData(List<MessageData> data)
        {
            this.Message.Data.AddRange(data);
            this.LastMessageDataTime = DateTime.Now;
        }

    }
}
