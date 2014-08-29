using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace LiNGS.Client
{
    /// <summary>
    /// Represent the status of a client
    /// </summary>
    public class ClientStatus
    {
        private LiNGSClient client;

        internal bool WasConnected { get; set; }

        /// <summary>
        /// The client <see cref="EndPoint"/>.
        /// </summary>
        public IPEndPoint EndPoint { get; internal set; }

        /// <summary>
        /// The Id of the current session. This can be used to restore the client state after a disconnection.
        /// </summary>
        public String SessionUserId { get; internal set; }

        /// <summary>
        /// Indicates if the client is currently connected or not to a server.
        /// </summary>
        public bool Connected { get; internal set; }

        internal ClientStatus(LiNGSClient liNGSClient)
        {
            this.client = liNGSClient;
        }

    }
}
