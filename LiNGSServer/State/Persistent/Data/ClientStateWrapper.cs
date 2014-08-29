using LiNGS.Server.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Server.State.Persistent.Data
{
    /// <summary>
    /// Used to wrap a client state.
    /// </summary>
    public class ClientStateWrapper
    {
        /// <summary>
        /// Time at which this state was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The objects of the client.
        /// </summary>
        public List<ClientObjectStateWrapper> ClientObjects { get; set; }

        /// <summary>
        /// The state of the server that was known by the client.
        /// </summary>
        public List<ClientObjectStateWrapper> ClientState { get; set; }

        /// <summary>
        /// Creates a new empty instance of this class.
        /// </summary>
        private ClientStateWrapper ()
	    {
                CreatedAt = DateTime.Now;
	    }

        /// <summary>
        /// Creates a new instance of this class with the provided state information.
        /// </summary>
        /// <param name="clientState">The client state.</param>
        /// <param name="clientObjects">The client objects.</param>
        public ClientStateWrapper(List<ClientObjectStateWrapper> clientState, List<ClientObjectStateWrapper> clientObjects)
            : this()
        {
            this.ClientState = clientState;
            this.ClientObjects = clientObjects;
        }

    }
}
