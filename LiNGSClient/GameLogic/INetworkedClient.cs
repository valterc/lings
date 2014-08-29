using LiNGS.Common.GameLogic;
using LiNGS.Common.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Client.GameLogic
{
    /// <summary>
    /// Represents the client part of a networked game. The game must implement this interface to use <see cref="LiNGSClient"/>.
    /// </summary>
    public interface INetworkedClient
    {
        /// <summary>
        /// The connection to the server was successful.
        /// </summary>
        /// <param name="message">Response message</param>
        /// <param name="usingSavedState">True if a saved state will be used, false otherwise</param>
        void ConnectionAccepted(NetworkMessage message, bool usingSavedState);

        /// <summary>
        /// The connected to the server was refused.
        /// </summary>
        /// <param name="message">Response message</param>
        void ConnectionRefused(NetworkMessage message);

        /// <summary>
        /// The client is unable to comunicate with the server.
        /// </summary>
        /// <param name="reason">Explanation of the problem ou NULL</param>
        void UnableToConnect(String reason);

        /// <summary>
        /// The connection to the server was closed.
        /// </summary>
        /// <param name="reason">The reason for the connection ending.</param>
        void Disconnected(String reason);


        /// <summary>
        /// Game type <see cref="NetworkMessage"/> received from the server.
        /// </summary>
        /// <param name="message">Game message</param>
        void ReceiveGameMessage(NetworkMessage message);

        /// <summary>
        /// Event type <see cref="NetworkMessage"/> received from the server.
        /// </summary>
        /// <param name="message">Event message</param>
        void ReceiveEventMessage(NetworkMessage message);

        /// <summary>
        /// Error type <see cref="NetworkMessage"/> received from the server.
        /// </summary>
        /// <param name="message">Error message</param>
        void ReceiveErrorMessage(NetworkMessage message);


        /// <summary>
        /// Create a new local <see cref="INetworkedObject"/> to be synchronized from the server.
        /// </summary>
        /// <param name="typeName">Name of type of object</param>
        /// <param name="name">Name of the object</param>
        /// <returns>The created object</returns>
        INetworkedObject CreateObject(string typeName, string name);

        /// <summary>
        /// Destroy a previously created <see cref="INetworkedObject"/>.
        /// </summary>
        /// <param name="networkedObject">The object to be destroyed</param>
        /// <param name="name">The object name</param>
        void DestroyObject(INetworkedObject networkedObject, string name);

    }
}
