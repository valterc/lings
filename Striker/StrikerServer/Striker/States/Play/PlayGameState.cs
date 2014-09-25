using LiNGS.Common.Network;
using Striker.Characters;
using Striker.Elements;
using Striker.States.End;
using Striker.States.Play;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Striker.States.Play
{
    public class PlayGameState : GameState
    {
        private DateTime gameStart;
        private bool gameStarted;

        public PlayGameState(GameController game) : base(game)
        {
            
        }

        public override void OnEnter()
        {
            gameStart = DateTime.Now;
            gameController.characters.AddRange(gameController.newPlayers.Cast<Character>());
            CreateNPCs();
            SetSpawns();
        }

        public override void Update()
        {
            if (!gameStarted)
            {
                if (DateTime.Now - gameStart > TimeSpan.FromSeconds(3))
                {
                    gameStarted = true;
                    InitServerStuff();
                }
                return;
            }

            for (int i = 0; i < gameController.characters.Count; i++)
            {
                Character c = gameController.characters[i];
                c.Update();

                if (c.kills >= 15)
                {
                    gameController.SwitchState(new GameEndState(gameController));
                    return;
                }

            }

            for (int i = 0; i < gameController.controllers.Count; i++)
            {
                PlayerController pc = gameController.controllers[i];
                if (pc.shooting && !pc.player.dead && !pc.player.disconnected)
                {
                    if (DateTime.Now - pc.lastFireTime > TimeSpan.FromMilliseconds(400))
                    {
                        pc.lastFireTime = System.DateTime.Now;
                        Bullet b = new Bullet(gameController.level, pc.player.player);
                        gameController.bullets.Add(b);
                        gameController.server.LogicProcessor.RegisterNetworkedObject(b);
                    }
                }
                pc.Update();
            }

            for (int i = 0; i < gameController.clientPlayers.Count; i++)
            {
                CPlayer cp = gameController.clientPlayers[i];
                cp.Update();
            }

            for (int i = 0; i < gameController.bullets.Count; i++)
            {
                Bullet bullet = gameController.bullets[i];

                foreach (var character in gameController.characters)
                {
                    if (character != bullet.character && !character.disconnected && !character.dead && bullet.CheckCollision(character))
                    {
                        character.health -= 2;
                        if (character.health <= 0)
                        {
                            bullet.character.kills++;
                        }
                        break;
                    }
                }

                if (bullet.Update())
                {
                    gameController.bullets.RemoveAt(i--);
                    gameController.server.LogicProcessor.UnregisterNetworkedObject(bullet);
                }
            }

        }

        private void InitServerStuff()
        {
            foreach (var item in gameController.characters)
            {
                gameController.server.LogicProcessor.RegisterNetworkedObject(item);
            }

            foreach (var item in gameController.clientPlayers)
            {
                gameController.server.LogicProcessor.RegisterNetworkedObject(item);
            }

            foreach (var item in gameController.server.LogicProcessor.GetConnectedClients())
            {
                gameController.server.LogicProcessor.SendMessageTo(item, new MessageData() { Object = "StartTime", Property = DateTime.Now.Ticks.ToString(), Value = "3" });
            }
        }

        private void CreateNPCs()
        {
            Random random = new Random();
            for (int i = 0; i < 4; i++)
            {
                gameController.characters.Add(new NPC(random.Next()));
            }
        }

        private void SetSpawns()
        {
            List<SpawnPoint> spawnsAvailable = gameController.level.availableSpawns.ToList();

            for (int i = 0; i < gameController.characters.Count; i++)
            {
                Character c = gameController.characters[i];

                SpawnPoint sp = spawnsAvailable[0];
                float positionX = sp.x * gameController.mapElementSize;
                float positionY = ((gameController.level.Height - 1) - sp.y) * gameController.mapElementSize;

                c.positionX = positionX;
                c.positionY = positionY;

                if (c is Player)
                {
                    CPlayer cplayer = gameController.clientPlayers.FirstOrDefault(cp => cp.networkGameClient == (c as Player).networkGameClient);
                    if (cplayer != null)
                    {
                        cplayer.positionX = positionX;
                        cplayer.positionY = positionY;
                    }

                }

                spawnsAvailable.RemoveAt(0);
            }

        }
        

    }
}
