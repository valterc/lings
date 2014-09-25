using LiNGS.Server;
using LiNGS.Server.GameLogic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Striker
{
    public class StrikerServer
    {
        public GameController game;
        private LiNGSServer server;

#if NOT_UNITY
        public StrikerServer(string levelName, string levelString, string namesString)
        {
            ServerProperties sp = ServerProperties.Default;
            sp.MaxClients = 2;
            sp.ListenPort = 6666;
            

            game = new GameController(levelName, levelString, namesString);
            server = new LiNGSServer(sp, game);
            game.server = server;
        }
#else
        public StrikerServer(string levelName)
        {
            ServerProperties sp = ServerProperties.Default;
            sp.MaxClients = 2;
            sp.ListenPort = 6666;

            game = new GameController(levelName);
            server = new LiNGSServer(sp, game);
            game.server = server;
        }
#endif

        public void Update()
        {
            game.Update();
            server.Update();
        }

        public void StartGame()
        {
            game.StartGame();
        }

        public void Shutdown()
        {
            server.Shutdown();
        }

        public void EnableNPCs(bool enable)
        {

        }

    }
}
