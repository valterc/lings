using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Client
{
    /// <summary>
    /// Represents the known information of the server.
    /// </summary>
    public class ServerInfo
    {
        /// <summary>
        /// A <see cref="ServerInfo"/> with the details of the default local server.
        /// </summary>
        public static ServerInfo DefaultLocal
        {
            get
            {
                return new ServerInfo("127.0.0.1", 7146);
            }
        }

        /// <summary>
        /// The IP address of the server.
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// The port of the server.
        /// </summary>
        public int Port { get; set; }

        private ServerInfo()
        {

        }

        /// <summary>
        /// Create a new instance based on other <see cref="ServerInfo"/>.
        /// </summary>
        /// <param name="other">The existing instance</param>
        public ServerInfo(ServerInfo other)
        {
            this.IP = other.IP;
            this.Port = other.Port;
        }

        /// <summary>
        /// Create a new instance with the required data.
        /// </summary>
        /// <param name="ip">The IP address of the server.</param>
        /// <param name="port">The port of the server.</param>
        public ServerInfo(string ip, int port)
        {
            this.IP = ip;
            this.Port = port;
        }

    }
}
