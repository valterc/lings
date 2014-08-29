using LiNGS.Common.GameLogic;
using LiNGS.Common.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Server.GameLogic
{
    /// <summary>
    /// Represents the access to the server game implementation logic. 
    /// The game must implement this in order to use the LiNGS System. 
    /// </summary>
    public interface INetworkedGame
    {

        /// <summary>
        /// Accept or refuse a <see cref="GameClient"/> connection to the server
        /// </summary>
        /// <param name="client">Information about the connecting client</param>
        /// <param name="message">The Connect Message sent by the client</param>
        /// <param name="savedState">True if the server has saved a state from the last session of the Client, false otherwise</param>
        /// <returns>The response of client the connection</returns>
        ClientConnectionResponse AcceptClient(GameClient client, NetworkMessage message, bool savedState);

        /// <summary>
        /// A <see cref="GameClient"/> has disconnected from the game
        /// </summary>
        /// <param name="client">Client disconnected</param>
        /// <returns>True to save the current state of the client, false otherwise</returns>
        bool ClientDisconnected(GameClient client);

        /// <summary>
        /// Checks if the <see cref="GameClient"/> needs to to know about the current state of the NetworkedObject
        /// </summary>
        /// <param name="client">The Client</param>
        /// <param name="networkedObject">The Gameobject</param>
        /// <returns>True if the Client most know about the game object on his current state, false otherwise</returns>
        bool DoesClientNeedToKnowAboutObject(GameClient client, INetworkedObject networkedObject);

        /// <summary>
        /// Error Message received from a <see cref="GameClient"/>
        /// </summary>
        /// <param name="client">Message sender</param>
        /// <param name="message">Received Message</param>
        void ReceiveErrorMessage(GameClient client, NetworkMessage message);

        /// <summary>
        /// Event Message received from a <see cref="GameClient"/>
        /// </summary>
        /// <param name="client">Message sender</param>
        /// <param name="message">Received Message</param>
        void ReceiveEventMessage(GameClient client, NetworkMessage message);

        /// <summary>
        /// Game Message received from a <see cref="GameClient"/>
        /// </summary>
        /// <param name="client">Message sender</param>
        /// <param name="message">Received Message</param>
        void ReceiveGameMessage(GameClient client, NetworkMessage message);
       
        /// <summary>
        /// Create a new local <see cref="INetworkedObject"/> to be synchronized from a <see cref="GameClient"/>
        /// </summary>
        /// <param name="client">The client of which the object belongs</param>
        /// <param name="typeName">The type of the object that will be created</param>
        /// <param name="name">The object name</param>
        /// <returns>The created object.</returns>
        INetworkedObject CreateClientObject(GameClient client, String typeName, String name);

        /// <summary>
        /// Destroy a previously created <see cref="INetworkedObject"/> from a <see cref="GameClient"/>
        /// </summary>
        /// <param name="client">The client</param>
        /// <param name="networkedObject">The reference to the object</param>
        /// <param name="name">The name of the object</param>
        void DestroyClientObject(GameClient client, INetworkedObject networkedObject, String name);

    }
}
