using LiNGS.Client.Aggregator;
using LiNGS.Client.GameLogic;
using LiNGS.Client.Management;
using LiNGS.Client.Network;
using LiNGS.Client.Simulation;
using LiNGS.Client.Synchronization;
using LiNGS.Common.Debug;
using LiNGS.Common.GameCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Client
{
    /// <summary>
    /// Client side of the LiNGS System.
    /// </summary>
    public class LiNGSClient
    {
        internal NetworkManager NetworkManager { get; private set; }
        internal Router Router { get; private set; }
        internal MessageAggregator MessageAggregator { get; private set; }
        internal Manager Manager { get; private set; }
        internal Analyzer Analyzer { get; private set; }
        internal ClientLogicProcessor ClientLogicProcessor { get; private set; }
        internal Simulator Simulator { get; private set; }
        internal Synchronizer Synchronizer { get; private set; }
        internal ClientProperties ClientProperties { get; private set; }
        internal ServerInfo ServerInfo { get; private set; }
        internal INetworkedClient NetworkedClientInstance { get; private set; }
        internal ClientStatus ClientStatus { get; private set; }

        internal UpdateManager UpdateManager { get; private set; }

        /// <summary>
        /// Access to the Logic Processor used to manage Networked objects
        /// </summary>
        public ClientLogicProcessor LogicProcessor
        {
            get
            {
                return ClientLogicProcessor;
            }
        }

        /// <summary>
        /// Status of the client system
        /// </summary>
        public ClientStatus Status
        {
            get
            {
                return ClientStatus;
            }
        }

        /// <summary>
        /// Constructor of LiNGS Client. Creates a new instance of <see cref="LiNGSClient"/>. 
        /// </summary>
        /// <exception cref="ArgumentNullException">When any of the params is null.</exception>
        /// <param name="properties">Properties of the client. These properties cannot be changed after the server is running.</param>
        /// <param name="serverInfo">Information used to connect to the server.</param>
        /// <param name="networkedClient">Game logic to receive callbacks and manage game objects.</param>
        public LiNGSClient(ClientProperties properties, ServerInfo serverInfo, INetworkedClient networkedClient)
        {
            if (properties == null)
            {
                throw new ArgumentNullException("The client properties cannot be null.");
            }

            if (serverInfo == null)
            {
                throw new ArgumentNullException("The server information cannot be null.");
            }

            if (networkedClient == null)
            {
                throw new ArgumentNullException("The networkedClient cannot be null.");
            }

            this.UpdateManager = new UpdateManager();
            this.ClientProperties = new ClientProperties(properties);
            this.ServerInfo = new ServerInfo(serverInfo);

            NetworkManager = new NetworkManager(this, ServerInfo.IP, ServerInfo.Port);
            Router = new Router(this);
            MessageAggregator = new MessageAggregator(this);
            Manager = new Manager(this);
            Analyzer = new Analyzer(this);
            ClientLogicProcessor = new ClientLogicProcessor(this);
            Simulator = new Simulator(this);
            Synchronizer = new Synchronizer(this);
            ClientStatus = new ClientStatus(this);
            NetworkedClientInstance = networkedClient;

            this.UpdateManager.AddUpdatable(MessageAggregator);
            this.UpdateManager.AddUpdatable(Manager);
            this.UpdateManager.AddUpdatable(Analyzer);
            this.UpdateManager.AddUpdatable(ClientLogicProcessor);
            this.UpdateManager.AddUpdatable(Simulator);
            this.UpdateManager.AddUpdatable(Synchronizer);
        }

        /// <summary>
        /// Initializes a connection to the server.
        /// </summary>
        public void Connect()
        {
            if (!ClientStatus.Connected)
            {
                Manager.Connect();
                try
                {
                    NetworkManager.Connect();
                }
                catch (Exception ex)
                {
                    Manager.UnableToConnect(ex.Message);
                }
            }
        }

        /// <summary>
        /// Disconnects this client instance from the server if a connection is established.
        /// </summary>
        public void Disconnect()
        {
            if (ClientStatus.Connected)
            {
                Manager.Disconnect();
            }
        }

        /// <summary>
        /// Update the entire LiNGS System. This function should be invoked on your game cycle on the same thread of the game objects. 
        /// </summary>
        public void Update()
        {
            if (ClientStatus.Connected)
            {
                this.UpdateManager.Update();
            }
            else
            {
                Manager.ConnectionEstablisherHelper.Update(TimeSpan.Zero);    
            }
            
        }

    }
}
