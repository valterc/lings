using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace LiNGS.Common.Network
{
    /// <summary>
    /// Represent a network client.
    /// </summary>
    public class NetworkClient
    {
        /// <summary>
        /// The last time a connection was received from the client.
        /// </summary>
        protected DateTime LastReceivedConnectionTime { get; set; }

        /// <summary>
        /// The last time a connection was sent to the client.
        /// </summary>
        protected DateTime LastSentConnectionTime { get; set; }


        /// <summary>
        /// The client endpoint.
        /// </summary>
        public EndPoint EndPoint { get; protected set; }

        /// <summary>
        /// The latency between this system and the client.
        /// </summary>
        public TimeSpan Latency { get; protected set; }

        /// <summary>
        /// The time offset of the client.
        /// </summary>
        public TimeSpan TimeOffset { get; protected set; }

        /// <summary>
        /// The current message Id of this particular client.
        /// </summary>
        public int MessageId
        {
            get
            {
                return Interlocked.Increment(ref messageId);
            }
        }

        private int messageId;

    }
}
