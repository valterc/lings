using LiNGS.Common.GameLogic;
using LiNGS.Common.Network;
using LiNGS.Server;
using Striker.States.Play;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Striker.Characters
{
    public class CPlayer : Player, INetworkedObject
    {
        private GameController gameController;
        public Player player;

        public CPlayer(GameClient client, Player player)
            : base(client)
        {
            this.gameController = GameController.instance;
            this.player = player;
            this.charName = player.charName;

        }

        internal override void Update()
        {
            player.positionX = this.positionX;
            player.positionY = this.positionY;
            player.rotation = this.rotation;

            kills = player.kills;
            health = player.health;
            base.Update();
        }

        protected override void Die()
        {
            this.dead = true;
            player.dead = true;
            deathTime = DateTime.Now;
        }

        protected override void Respawn()
        {
            SpawnPoint sp = gameController.GetSpawnLocation(this);

            this.positionX = sp.x;
            this.positionY = sp.y;

            player.positionX = sp.x;
            player.positionY = sp.y;
            player.health = 10;


            gameController.server.LogicProcessor.SendMessageTo(networkGameClient, new MessageData() { Object = "PlayerSpawn", Value = positionX + ":" + positionY });
            this.dead = false;
            player.dead = false;
            spawnTime = DateTime.Now;

            PlayerController pc = gameController.controllers.FirstOrDefault(c => c.player == this);
            if (pc != null)
            {
                pc.positionX = sp.x;
                pc.positionY = sp.y;
                pc.SpawnTime = DateTime.Now;
            }

        }

    }
}
