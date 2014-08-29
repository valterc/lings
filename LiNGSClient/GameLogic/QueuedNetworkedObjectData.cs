using LiNGS.Common.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Client.GameLogic
{
    internal class QueuedNetworkedObjectData
    {
        internal MessageData MessageData { get; set; }
        internal String ObjectName { get; set; }

        public QueuedNetworkedObjectData()
        {

        }
    }
}
