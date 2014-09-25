using LiNGS.Common.GameLogic;
using LiNGS.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Striker.Characters
{
    public class Player : Character
    {
        public GameClient networkGameClient;

        public Player(GameClient client)
        {
            this.networkGameClient = client;
        }

    }
}
