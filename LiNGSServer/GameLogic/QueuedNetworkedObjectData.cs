using LiNGS.Common.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Server.GameLogic
{
    internal class QueuedClientNetworkedObjectData
    {
        internal MessageData MessageData { get; set; }
        internal String ObjectName { get; set; }
        internal GameClient Client { get; set; }

        public QueuedClientNetworkedObjectData()
        {

        }
    }
}
