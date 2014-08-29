using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LiNGS.Server
{
    /// <summary>
    /// Properties to configure the behaviour the LiNGS Server.
    /// </summary>
    public class ServerProperties
    {
        /// <summary>
        /// The Default values for <see cref="ServerProperties"/>.
        /// </summary>
        public static ServerProperties Default
        {
            get
            {
                return new ServerProperties();
            }
        }

        /// <summary>
        /// The server port.
        /// </summary>
        public int ListenPort { get; set; }

        /// <summary>
        /// Maximum message size.
        /// </summary>
        public int MaxMessageSize { get; set; }

        /// <summary>
        /// Maximum clients that the server will accept.
        /// </summary>
        public int MaxClients { get; set; }

        /// <summary>
        /// Time in milliseconds to wait for a answer to an important message.
        /// </summary>
        public int ImportantMessageTimeout { get; set; }

        /// <summary>
        /// The maximum time a important message can be resend. After those tries the connection with the receiver will be closed.
        /// </summary>
        public int MaxImportantMessageRetries { get; set; }

        /// <summary>
        /// Maximum time in milliseconds that a client can be without comunicating with the server.
        /// </summary>
        public int MaxClientBlackoutTime { get; set; }

        /// <summary>
        /// Time in milliseconds that a message can be queued before being sent.
        /// </summary>
        public int MaxMessageWaitTime { get; set; }

        /// <summary>
        /// The path for the base directory in which the session files will be stored.
        /// </summary>
        public String SessionStorageBaseDirectory { get; set; }

        /// <summary>
        /// Flag to use real class names instead of LiNGS generated names. Debug only.
        /// </summary>
        public bool UseRealClassNames { get; set; }

        /// <summary>
        /// Indicates if the server should use a simple state manager. Not implemented.
        /// </summary>
        public bool UseSimpleStateManager { get; set; }

        /// <summary>
        /// Indicates if the server should persist client states.
        /// </summary>
        public bool EnablePersistentStates { get; set; }

        /// <summary>
        /// Indicates if the operations of the server are stored in a log. Not implemented.
        /// </summary>
        public bool EnableLog { get; set; }

        /// <summary>
        /// Indicates if the server should delete all session files after the server is closed.
        /// </summary>
        public bool DeleteSessionFilesOnExit { get; set; }

        internal int MaxMessageDataSize
        {
            get
            {
                return MaxMessageSize - 13;
            }
        }

        /// <summary>
        /// Creates a new instance of this class with the default values.
        /// </summary>
        public ServerProperties()
        {
            ListenPort = 7146;
            MaxMessageSize = 512; //Maximum message size is defaulted to 512 bytes to avoid network fragmentation, see: RFC 1122 Section 3.3.2
            MaxClients = 4;
            ImportantMessageTimeout = 1000;
            MaxImportantMessageRetries = 5;
            MaxClientBlackoutTime = 10000;
            MaxMessageWaitTime = 50;
            SessionStorageBaseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "SessionStorage");
            UseRealClassNames = false;
            UseSimpleStateManager = false;
            EnablePersistentStates = true;
            EnableLog = false;
            DeleteSessionFilesOnExit = true;
        }

        /// <summary>
        /// Creates a new instance of this class with values based on other instance.
        /// </summary>
        /// <param name="properties">Existent object</param>
        public ServerProperties(ServerProperties properties)
        {
            this.ListenPort = properties.ListenPort;
            this.MaxMessageSize = properties.MaxMessageSize;
            this.MaxClients = properties.MaxClients;
            this.ImportantMessageTimeout = properties.ImportantMessageTimeout;
            this.MaxImportantMessageRetries = properties.MaxImportantMessageRetries;
            this.MaxClientBlackoutTime = properties.MaxClientBlackoutTime;
            this.MaxMessageWaitTime = properties.MaxMessageWaitTime;
            this.SessionStorageBaseDirectory = properties.SessionStorageBaseDirectory;
            this.UseRealClassNames = properties.UseRealClassNames;
            this.UseSimpleStateManager = properties.UseSimpleStateManager;
            this.EnablePersistentStates = properties.EnablePersistentStates;
            this.EnableLog = properties.EnableLog;
            this.DeleteSessionFilesOnExit = properties.DeleteSessionFilesOnExit;
        }

    }
}
