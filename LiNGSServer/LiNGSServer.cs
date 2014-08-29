using LiNGS.Common.Debug;
using LiNGS.Common.GameCycle;
using LiNGS.Server.Aggregator;
using LiNGS.Server.GameLogic;
using LiNGS.Server.Management;
using LiNGS.Server.Network;
using LiNGS.Server.State;
using LiNGS.Server.State.Persistent;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LiNGS.Server
{
    /// <summary>
    /// Server part of the LiNGS System.
    /// </summary>
    public class LiNGSServer
    {
        internal NetworkManager NetworkManager { get; private set; }
        internal Router Router { get; private set; }
        internal Manager Manager { get; private set; }
        internal MessageAggregator MessageAggregator { get; private set; }
        internal GameLogicProcessor GameLogicProcessor { get; private set; }
        internal StateManager StateManager { get; private set; }
        internal PersistentStateManager PersistentStateManager { get; private set; }
        internal Analyzer Analyzer { get; private set; }
        internal Dispatcher Dispatcher { get; private set; }
        internal INetworkedGame NetworkedGameInstance { get; private set; }
        internal ServerProperties ServerProperties { get; private set; }

        private UpdateManager updateManager;

        /// <summary>
        /// The properties of LiNGS System.
        /// </summary>
        public ServerProperties Properties
        {
            get
            {
                return ServerProperties;
            }
        }

        /// <summary>
        /// Logic Processor used to manage <see cref="LiNGS.Common.GameLogic.INetworkedObject"/>s and send <see cref="LiNGS.Common.Network.NetworkMessage"/>s to <see cref="GameClient"/>s.
        /// </summary>
        public GameLogicProcessor LogicProcessor
        {
            get
            {
                return GameLogicProcessor;
            }
        }

        /// <summary>
        /// Creates a new LiNGS Server instance. The server starts immediately.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when any param is null.</exception>
        /// <param name="properties">Properties of this server instance. The properties are not changeable at runtime.</param>
        /// <param name="networkedGame">The instance of your game logic.</param>
        public LiNGSServer(ServerProperties properties, INetworkedGame networkedGame)
        {
            if (properties == null)
            {
                throw new ArgumentNullException("Server properties cannot be null.");
            }

            if (networkedGame == null)
            {
                throw new ArgumentNullException("NetworkedGame cannot be null.");
            }

            this.updateManager = new UpdateManager();
            ServerProperties = new ServerProperties(properties);

            NetworkManager = new NetworkManager(this, properties.ListenPort);
            Manager = new Manager(this, properties.MaxClients);
            Router = new Router(this);
            MessageAggregator = new MessageAggregator(this);
            GameLogicProcessor = new GameLogicProcessor(this);
            StateManager = new StateManager(this);
            PersistentStateManager = new PersistentStateManager(this);
            Analyzer = new Analyzer(this);
            Dispatcher = new Dispatcher(this);
            NetworkedGameInstance = networkedGame;

            this.updateManager.AddUpdatable(MessageAggregator);
            this.updateManager.AddUpdatable(StateManager);
            this.updateManager.AddUpdatable(Dispatcher);
            this.updateManager.AddUpdatable(Manager);
            this.updateManager.AddUpdatable(Analyzer);
            this.updateManager.AddUpdatable(GameLogicProcessor);
        }

        /// <summary>
        /// Updates all server logic. 
        /// Call this method on the same thread as the game logic to avoid synchronization issues.
        /// </summary>
        public void Update()
        {
            this.updateManager.Update();
        }

        /// <summary>
        /// Disconnects all clients, terminates all connections and releases server resources
        /// </summary>
        public void Shutdown()
        {
            NetworkManager.Shutdown();
            Manager.Shutdown();
            if (ServerProperties.DeleteSessionFilesOnExit)
            {
                PersistentStateManager.ClearStates();
            }
        }

    }
}
