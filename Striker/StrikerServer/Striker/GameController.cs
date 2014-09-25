using LiNGS.Common.Network;
using LiNGS.Server;
using LiNGS.Server.GameLogic;
using Striker.Characters;
using Striker.Elements;
using Striker.Levels;
using Striker.Levels.Elements;
using Striker.States;
using Striker.States.Menu;
using Striker.States.Play;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Striker
{
    public class GameController : INetworkedGame
    {
        public static GameController instance;

        public LiNGSServer server;

        public float mapElementSize = 2f;
        public string levelName;
        public List<Player> newPlayers;
        public List<Character> characters;
        public List<CPlayer> clientPlayers;
        public List<PlayerController> controllers;
        public List<Bullet> bullets;
        public Level level;
        private GameState currentState;

        private GameController()
        {
            instance = this;
            characters = new List<Character>();
            newPlayers = new List<Player>();
            clientPlayers = new List<CPlayer>();
            controllers = new List<PlayerController>();
            bullets = new List<Bullet>();

            SwitchState(new MenuGameState(this));
        }

#if NOT_UNITY
        public GameController(string levelName, string levelString, string namesFileContent)
            : this()
        {
            this.levelName = levelName;
            level = LevelLoader.LoadLevel(levelString);
            CharName.BuildCache(namesFileContent);
        }
#else
        public GameController(string levelName) : this()
        {
            this.levelName = levelName;
            level = LevelLoader.LoadLevel(Resources.Load("Levels/" + levelName).ToString());
        }
#endif

        internal void Update()
        {
            currentState.Update();
        }

        internal void StartGame()
        {
            foreach (var item in server.LogicProcessor.GetConnectedClients())
            {
                server.LogicProcessor.SendMessageTo(item, new MessageData() { Object = "StartLevel", Value = levelName });
            }
            SwitchState(new PlayGameState(this));
        }

        public void SwitchState(GameState newState)
        {
            if (currentState != null)
            {
                currentState.OnExit();
            }

            currentState = newState;

            if (currentState != null)
            {
                currentState.OnEnter();
            }
        }

        public SpawnPoint GetSpawnLocation(Character c)
        {
            bool spawnFound = true;
            foreach (var spawn in level.availableSpawns)
            {
                spawnFound = true;

                float sX = spawn.x * mapElementSize;
                float sY = ((level.Height - 1) - spawn.y) * mapElementSize;

                foreach (var character in characters)
                {
                    if (character.DistanceFrom(sX, sY) < 10)
                    {
                        spawnFound = false;
                        break;
                    }
                }

                if (spawnFound)
                {
                    return new SpawnPoint() { x = sX, y = sY };
                }
            }

            return level.availableSpawns[0];
        }

        internal Vector2 GetRandomFloorLocation()
        {
            System.Random random = new System.Random();
            Element element = null;
            do
            {
                element = level.map[random.Next(0, level.map.Count)];
            } while (!element.canWalk);

            int x = level.map.IndexOf(element) % level.Width;
            int y = level.map.IndexOf(element) / level.Width;

            float positionX = x * mapElementSize;
            float positionY = ((level.Height - 1) - y) * mapElementSize;

            return new Vector2(positionX, positionY);
        }

        #region INetworkedGame Members

        public ClientConnectionResponse AcceptClient(LiNGS.Server.GameClient client, LiNGS.Common.Network.NetworkMessage message, bool savedState)
        {
            if (currentState is PlayGameState && !savedState)
            {
                return new ClientConnectionResponse(false, false, "Game in progress");
            }
            else if (savedState)
            {
                CPlayer cPlayer = clientPlayers.FirstOrDefault(cp => cp.networkGameClient.SessionUserId == client.SessionUserId);
                if (cPlayer != null)
                {
                    PlayerController controller = controllers.FirstOrDefault(c => c.client.SessionUserId == client.SessionUserId);
                    if (controller != null)
                    {
                        controller.client = client;
                    }

                    cPlayer.networkGameClient = client;
                    cPlayer.player.networkGameClient = client;
                    cPlayer.disconnected = false;
                    cPlayer.player.disconnected = false;
                    return new ClientConnectionResponse(true, true);
                }
                return new ClientConnectionResponse(false, false, "Invalid game state.");
            }
            else
            {
                Player p = new Player(client);
                newPlayers.Add(p);
                clientPlayers.Add(new CPlayer(client, p));
                return new ClientConnectionResponse(true, false);
            }
        }

        public bool ClientDisconnected(LiNGS.Server.GameClient client)
        {
            CPlayer cPlayer = clientPlayers.FirstOrDefault(cp => cp.networkGameClient == client);
            if (cPlayer != null)
            {
                cPlayer.disconnected = true;
                cPlayer.player.disconnected = true;
            }
            return true;
        }

        public bool DoesClientNeedToKnowAboutObject(LiNGS.Server.GameClient client, LiNGS.Common.GameLogic.INetworkedObject networkedObject)
        {
            if (networkedObject is CPlayer)
            {
                return (networkedObject as CPlayer).networkGameClient == client;
            }
            else
            {
                if (networkedObject is Player)
                {
                    if ((networkedObject as Player).networkGameClient != client)
                    {
                        return true;
                    }
                    return false;
                }

                return true;
            }

            if (networkedObject is Character && characters.Contains((Character)networkedObject))
            {
                if (networkedObject is Player)
                {
                    if ((networkedObject as Player).networkGameClient != client)
                    {
                        Character c = clientPlayers.FirstOrDefault(cp => cp.networkGameClient == client);
                        if (c != null)
                        {
                            if (c.DistanceFromOther(networkedObject as Player) < 15)
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }
                else
                {
                    Character c = clientPlayers.FirstOrDefault(cp => cp.networkGameClient == client);
                    if (c != null)
                    {
                        if (c.DistanceFromOther(networkedObject as Character) < 15)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }
            else
            {
                if (networkedObject is CPlayer)
                {
                    return (networkedObject as CPlayer).networkGameClient == client;
                }
                else if (networkedObject is Character)
                {
                    Character c = clientPlayers.FirstOrDefault(cp => cp.networkGameClient == client);
                    if (c != null)
                    {
                        if (c.DistanceFromOther(networkedObject as Character) < 15)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }

            return true;
        }

        public void ReceiveErrorMessage(LiNGS.Server.GameClient client, LiNGS.Common.Network.NetworkMessage message)
        {

        }

        public void ReceiveEventMessage(LiNGS.Server.GameClient client, LiNGS.Common.Network.NetworkMessage message)
        {

        }

        public void ReceiveGameMessage(LiNGS.Server.GameClient client, LiNGS.Common.Network.NetworkMessage message)
        {

        }

        public LiNGS.Common.GameLogic.INetworkedObject CreateClientObject(LiNGS.Server.GameClient client, string typeName, string name)
        {
            PlayerController pController = controllers.FirstOrDefault(pc => pc.client == client);
            if (pController != null)
            {
                return pController;
            }

            PlayerController c = new PlayerController(client, clientPlayers.FirstOrDefault(cp => cp.networkGameClient == client));
            controllers.Add(c);
            return c;
        }

        public void DestroyClientObject(LiNGS.Server.GameClient client, LiNGS.Common.GameLogic.INetworkedObject networkedObject, string name)
        {
            //Keep client objects
            return;
        }

        #endregion
                
    }
}
