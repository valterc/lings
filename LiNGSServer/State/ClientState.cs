using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Server.State
{
    internal class ClientState
    {
        public GameClient Client { get; private set; }
        public StateHolder State { get; private set; }

        public ClientState(GameClient client)
        {
            this.Client = client;
            this.State = new StateHolder();
        }

    }
}
