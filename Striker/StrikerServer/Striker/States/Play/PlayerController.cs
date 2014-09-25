using LiNGS.Common.GameLogic;
using LiNGS.Server;
using Striker.Characters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Striker.States.Play
{
    public class PlayerController: INetworkedObject
    {
        [NetworkedField]
        public float positionX;

        [NetworkedField]
        public float positionY;

        [NetworkedField]
        public float rotation;

        [NetworkedField]
        public bool spawned;

        [NetworkedField]
        public bool shooting;

        public DateTime lastFireTime;

        public GameClient client;
        public CPlayer player;
        public DateTime SpawnTime;

        public PlayerController(GameClient client, CPlayer player)
        {
            this.client = client;
            this.player = player;
        }

        public void Update()
        {
            if (spawned && DateTime.Now - SpawnTime > TimeSpan.FromMilliseconds(500))
            {
                player.positionX = positionX;
                player.positionY = positionY;
                player.rotation = rotation;
            }
            
        }

    }
}
