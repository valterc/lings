using LiNGS.Common.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace LiNGS.Client.Network
{
    internal class InternalNetworkClient : NetworkClient
    {

        internal new EndPoint EndPoint
        {
            get
            {
                return base.EndPoint;
            }
            set
            {
                base.EndPoint = value;
            }
        }

        internal new TimeSpan Latency
        {
            get
            {
                return base.Latency;
            }
            set
            {
                base.Latency = value;
            }
        }

        internal new DateTime LastReceivedConnectionTime
        {
            get
            {
                return base.LastReceivedConnectionTime;
            }
            set
            {
                base.LastReceivedConnectionTime = value;
            }
        }

        internal new DateTime LastSentConnectionTime
        {
            get
            {
                return base.LastSentConnectionTime;
            }
            set
            {
                base.LastSentConnectionTime = value;
            }
        }

        internal new TimeSpan TimeOffset
        {
            get
            {
                return base.TimeOffset;
            }
            set
            {
                base.TimeOffset = value;
            }
        }

        internal Boolean HeartbeatSent { get; set; }

    }
}
