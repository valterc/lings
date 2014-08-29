using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Server.GameLogic
{
    /// <summary>
    /// Represents an answer to a client connection request.
    /// </summary>
    public class ClientConnectionResponse
    {

        /// <summary>
        /// Indicates if the connecting client should be accepted into the server
        /// </summary>
        public bool Accept { get; set; }

        /// <summary>
        /// Indicates if the server should try to use any saved state for the connecting user
        /// </summary>
        public bool UseSavedState { get; set; }

        /// <summary>
        /// Reason for the connection refusal if that was the case
        /// </summary>
        public string RefuseMessage { get; set; }

        /// <summary>
        /// Constructs a new <see cref="ClientConnectionResponse"/> refusing the user connection.
        /// </summary>
        public ClientConnectionResponse()
        {
        }

        /// <summary>
        /// Constructs a new <see cref="ClientConnectionResponse"/> indicating if the client should be accepted and if the server should restore a previously saved state.
        /// </summary>
        /// <param name="accept">True to accept user connection, False to refuse</param>
        /// <param name="useSavedState">True to try to use a previously saved state, False otherwise</param>
        /// <param name="refuseMessage">Text message to be sent to the user explaining why his connection was refused if that was the case</param>
        public ClientConnectionResponse(bool accept, bool useSavedState = false, string refuseMessage = null)
        {
            this.Accept = accept;
            this.UseSavedState = useSavedState;
            this.RefuseMessage = refuseMessage;
        }

    }
}
