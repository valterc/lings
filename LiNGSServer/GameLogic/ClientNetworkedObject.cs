using LiNGS.Common;
using LiNGS.Common.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LiNGS.Server.GameLogic
{
    internal class ClientNetworkedObject : NetworkedObject
    {
        internal GameClient Client;

        public ClientNetworkedObject(GameClient client, INetworkedObject obj, bool useRealName = false, bool autoCreateObject = true)
            : base(obj, useRealName, autoCreateObject)
        {
            this.Client = client;
        }

        public ClientNetworkedObject(GameClient client, INetworkedObject obj, string name)
            : base(obj, name)
        {
            this.Client = client;
        }

    }
}
