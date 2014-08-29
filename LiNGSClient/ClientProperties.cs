using LiNGS.Client.Simulation.Simulators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Client
{
    /// <summary>
    /// Properties to configure the behaviour of LiNGS Client
    /// </summary>
    public class ClientProperties
    {
        /// <summary>
        /// Default <see cref="ClientProperties"/> values.
        /// </summary>
        public static ClientProperties Default
        {
            get
            {
                return new ClientProperties();
            }
        }

        /// <summary>
        /// The maximum size of a transmitted and received message, in bytes.
        /// </summary>
        public int MaxMessageSize { get; set; }

        /// <summary>
        /// Maximum time in milliseconds to wait for an answer to a important message.
        /// </summary>
        public int ImportantMessageTimeout { get; set; }

        /// <summary>
        /// Time in milliseconds to wait for an answer to a connect message.
        /// </summary>
        public int ConnectionEstablishingTimeout { get; set; }

        /// <summary>
        /// Maximum time allowed to operate without any communication from the server. 
        /// </summary>
        public int MaxServerBlackoutTime { get; set; }

        /// <summary>
        /// Maximum number of retries to send an important message. 
        /// If the client doesn't get an answer after all those tries 
        /// then the connection to the server is closed.
        /// </summary>
        public int MaxImportantMessageRetries { get; set; }

        /// <summary>
        /// Enables or disabled the client log. Not implemented.
        /// </summary>
        public bool EnableLog { get; set; }

        /// <summary>
        /// Logic used to simulate the values of Simulated Fields.
        /// </summary>
        public SimulatorLogic FieldSimulationLogic { get; set; }

        internal int MaxMessageDataSize
        {
            get
            {
                return MaxMessageSize - 13;
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="ClientProperties"/> with default values.
        /// </summary>
        public ClientProperties()
        {
            MaxMessageSize = 512;
            ImportantMessageTimeout = 250;
            ConnectionEstablishingTimeout = 10000;
            MaxServerBlackoutTime = 5000;
            MaxImportantMessageRetries = 10;
            EnableLog = false;
            FieldSimulationLogic = new LinearExtrapolationSimulatorLogic();
        }

        /// <summary>
        /// Creates a new instance of <see cref="ClientProperties"/> based on the values of an already existent instance.
        /// </summary>
        /// <param name="other">Other <see cref="ClientProperties"/> instace.</param>
        public ClientProperties(ClientProperties other)
        {
            this.MaxMessageSize = other.MaxMessageSize;
            this.ImportantMessageTimeout = other.ImportantMessageTimeout;
            this.ConnectionEstablishingTimeout = other.ConnectionEstablishingTimeout;
            this.MaxServerBlackoutTime = other.MaxServerBlackoutTime;
            this.MaxImportantMessageRetries = other.MaxImportantMessageRetries;
            this.EnableLog = other.EnableLog;
            this.FieldSimulationLogic = other.FieldSimulationLogic;
        }

    }
}
