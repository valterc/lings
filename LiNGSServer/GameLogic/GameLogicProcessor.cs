using LiNGS.Common;
using LiNGS.Common.GameCycle;
using LiNGS.Common.GameLogic;
using LiNGS.Common.Network;
using LiNGS.Server.State.Persistent.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LiNGS.Server.GameLogic
{
    /// <summary>
    /// Handles the logic processing of the server side of the LiNGS System.
    /// </summary>
    public class GameLogicProcessor : IUpdatable
    {
        private LiNGSServer server;
        private List<QueuedClientNetworkedObjectData> networkObjectQueue;
        private List<GameClient> disconnectedClients;

        internal Dictionary<string, NetworkedObject> NetworkedObjects;
        internal Dictionary<string, ClientNetworkedObject> ClientNetworkedObjects;

        internal GameLogicProcessor(LiNGSServer server)
        {
            this.server = server;
            this.NetworkedObjects = new Dictionary<string, NetworkedObject>();
            this.ClientNetworkedObjects = new Dictionary<string, ClientNetworkedObject>();
            this.networkObjectQueue = new List<QueuedClientNetworkedObjectData>();
            this.disconnectedClients = new List<GameClient>();
        }

        internal void ReceiveDataMessage(NetworkMessage message)
        {
            GameClient client = server.Manager.GetClient(message.From);

            if (client == null)
            {
                return;
            }

            lock (networkObjectQueue)
            {
                foreach (var item in message.Data)
                {
                    networkObjectQueue.Add(new QueuedClientNetworkedObjectData() { MessageData = item, ObjectName = item.Object, Client = client });
                }
            }
        }

        internal void ReceiveGameMessage(NetworkMessage message)
        {
            GameClient client = server.Manager.GetClient(message.From);
            if (client != null)
            {
                server.NetworkedGameInstance.ReceiveGameMessage(client, message);
            }
        }

        internal void ReceiveEventMessage(NetworkMessage message)
        {
            GameClient client = server.Manager.GetClient(message.From);
            if (client != null)
            {
                server.NetworkedGameInstance.ReceiveEventMessage(client, message);
            }
        }

        internal void ReceiveErrorMessage(NetworkMessage message)
        {
            GameClient client = server.Manager.GetClient(message.From);
            if (client != null)
            {
                server.NetworkedGameInstance.ReceiveErrorMessage(client, message);
            }
        }
        
        internal bool DoesClientNeedToKnowAboutObject(GameClient client, NetworkedObject networkedObject)
        {
            return server.NetworkedGameInstance.DoesClientNeedToKnowAboutObject(client, networkedObject.OriginalObject);
        }

        internal ClientConnectionResponse AcceptClient(GameClient client, NetworkMessage message, bool savedState)
        {
            return server.NetworkedGameInstance.AcceptClient(client, message, savedState);
        }

        /// <summary>
        /// Invoked when a client disconnects from the server
        /// </summary>
        /// <param name="client">Disconnected client</param>
        /// <returns>True if client session should be stored, False otherwise</returns>
        internal bool ClientDisconnected(GameClient client)
        {
            return server.NetworkedGameInstance.ClientDisconnected(client);
        }

        internal void RemoveClient(GameClient client)
        {
            lock (disconnectedClients)
            {
                disconnectedClients.Add(client);
            }
        }

        internal void RestoreClientStateObject(GameClient client, ClientObjectStateWrapper objectStateWrapper)
        {
            CreateClientObject(client, objectStateWrapper.TypeName, objectStateWrapper.Name);

            foreach (var item in objectStateWrapper.Fields)
            {
                ApplyClientObjectValue(client, objectStateWrapper.Name, item.Name, item.Value);
            }

        }


        private void CreateClientObject(GameClient client, string typeName, string name)
        {
            String objName = LiNGSMarkers.AutoCreatedObject + client.UserId + LiNGSMarkers.Separator + name;
            if (!ClientNetworkedObjects.ContainsKey(objName))
            {
                //TODO: Log this
                INetworkedObject nObject = server.NetworkedGameInstance.CreateClientObject(client, typeName, name);
                ClientNetworkedObjects.Add(objName, new ClientNetworkedObject(client, nObject, name));
            }            
        }

        private void DestroyClientObject(GameClient client, string name)
        {
            String objName = LiNGSMarkers.AutoCreatedObject + client.UserId + LiNGSMarkers.Separator + name;

            ClientNetworkedObject no;
            if (ClientNetworkedObjects.TryGetValue(objName, out no))
	        {
                server.NetworkedGameInstance.DestroyClientObject(client, no.OriginalObject, name);
                ClientNetworkedObjects.Remove(objName);
	        }
        }

        private void ApplyClientObjectValue(GameClient client, string objectName, string propertyName, string value)
        {
            String objName = LiNGSMarkers.AutoCreatedObject + client.UserId + LiNGSMarkers.Separator + objectName;

            ClientNetworkedObject no;
            if (ClientNetworkedObjects.TryGetValue(objName, out no))
            {
                no.ReceiveValue(propertyName, value);
            }
        }


        /// <summary>
        /// Registers a new <see cref="INetworkedObject"/> which will be created and syncronized on all clients. 
        /// </summary>
        /// <param name="networkedObject">The networked object</param>
        public void RegisterNetworkedObject(INetworkedObject networkedObject)
        {
            NetworkedObject no = new NetworkedObject(networkedObject, server.ServerProperties.UseRealClassNames);
            NetworkedObjects[no.Name] = no;
        }
        
        /// <summary>
        /// Unregisters a <see cref="INetworkedObject"/> from the server logic. Optionally, the <see cref="INetworkedObject"/> can be auto destroyed on all clients.
        /// </summary>
        /// <param name="networkedObject">The networked object</param>
        /// <param name="destroyObjectOnClient">True to delete the object from all client, False to keep the object alive on the clients</param>
        public void UnregisterNetworkedObject(INetworkedObject networkedObject, bool destroyObjectOnClient = true)
        {
            var noKv = NetworkedObjects.FirstOrDefault(kv => kv.Value.OriginalObject == networkedObject);
            if (noKv.Key != null)
            {
                NetworkedObjects.Remove(noKv.Key);
                server.StateManager.RemoveObjectFromClients(noKv.Key, destroyObjectOnClient);
            }
        }

        /// <summary>
        /// Registers a new <see cref="INetworkedObject"/> which will be syncronized on all clients. The Clients must already know about this object, the object will not be auto created on the clients.
        /// </summary>
        /// <param name="networkedObject">The networked object</param>
        /// <param name="name">UNIQUE object name</param>
        public void RegisterClientAwareNetworkedObject(INetworkedObject networkedObject, string name)
        {
            NetworkedObject no = new NetworkedObject(networkedObject, name);
            NetworkedObjects[no.Name] = no;
        }

        /// <summary>
        /// Unregisters a <see cref="INetworkedObject"/> from the server logic. The Clients must be aware of the removal of this object, the object will not be auto destroyed on the clients.
        /// </summary>
        /// <param name="networkedObject">The networked object</param>
        public void UnregisterClientAwareNetworkedObject(INetworkedObject networkedObject)
        {
            var noKv = NetworkedObjects.FirstOrDefault(kv => kv.Value.OriginalObject == networkedObject);
            if (noKv.Key != null)
            {
                NetworkedObjects.Remove(noKv.Key);
                server.StateManager.RemoveObjectFromClients(noKv.Key, false);
            }
        }

        /// <summary>
        /// Send a custom <see cref="NetworkMessage"/> to a <see cref="GameClient"/>
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the <see cref="NetworkMessage"/> Type is not Event, Game or Error</exception>
        /// <param name="client">Destination client</param>
        /// <param name="message">Message to send</param>
        public void SendMessageTo(GameClient client, NetworkMessage message)
        {
            if (message.Type != NetworkMessage.MessageType.Error && 
                message.Type != NetworkMessage.MessageType.Event && 
                message.Type != NetworkMessage.MessageType.Game)
            {
                throw new ArgumentException("Message Type must be Event, Game or Error.");
            }

            message.MessageId = client.NetworkClient.MessageId;
            message.NeedsAck = true;
            server.Manager.SendMessage(client.NetworkClient, message);
        }

        /// <summary>
        /// Send a custom <see cref="NetworkMessage"/> to a <see cref="GameClient"/>.
        /// The message type is <see cref="NetworkMessage.MessageType.Game"/>
        /// </summary>
        /// <param name="client">Destination client</param>
        /// <param name="data">The data that will be sent on the message</param>
        public void SendMessageTo(GameClient client, params MessageData[] data)
        {
            NetworkMessage message = new NetworkMessage();
            message.MessageId = client.NetworkClient.MessageId;
            message.NeedsAck = true;
            message.Type = NetworkMessage.MessageType.Game;
            message.Data.AddRange(data);

            server.Manager.SendMessage(client.NetworkClient, message);
        }

        /// <summary>
        /// Returns a collection with all the current connected <see cref="GameClient"/>
        /// </summary>
        /// <returns>Connected Clients</returns>
        public IEnumerable<GameClient> GetConnectedClients()
        {
            //Create a new list to avoid synchronization problems
            //If any data received from the network changes the original list 
            //while the list is being used by the user then a InvalidOperationException will be thrown by the underlying framework
            //this copy is created to avoid that risk and to also avoid the user messing with the original list (Add/Remove clients)
            return server.Manager.ConnectedClients.ToList();
        }

        /// <summary>
        /// Disconnects a client from the server
        /// </summary>
        /// <param name="client">Client to disconnect</param>
        /// <param name="reason">Reason for disconnect</param>
        public void DisconnectClient(GameClient client, String reason = null)
        {
            server.Manager.DisconnectClient(client, reason);
        }

        #region IUpdatable Members

        /// <summary>
        /// Update the component internals. Do not call this directly.
        /// </summary>
        /// <param name="timeSinceLastUpdate">Elapsed time since this function was last called.</param>
        public void Update(TimeSpan timeSinceLastUpdate)
        {

            lock (disconnectedClients)
            {
                foreach (var client in disconnectedClients)
                {
                    networkObjectQueue.RemoveAll(q => q.Client == client);

                    List<ClientNetworkedObject> objectsOfClient = ClientNetworkedObjects.Where(kv => kv.Value.Client == client).Select(kv => kv.Value).ToList();
                    foreach (var obj in objectsOfClient)
                    {
                        DestroyClientObject(client, obj.Name);
                    }
                }

                disconnectedClients.Clear();
            }

            lock (networkObjectQueue)
            {
                IEnumerable<QueuedClientNetworkedObjectData> destroyObjects = networkObjectQueue.Where(q => q.MessageData.Object.StartsWith(LiNGSMarkers.DestroyObject));

                foreach (var item in destroyObjects)
                {
                    DestroyClientObject(item.Client, item.MessageData.Value);
                }

                IEnumerable<QueuedClientNetworkedObjectData> createObjects = networkObjectQueue.Where(q => q.MessageData.Object.StartsWith(LiNGSMarkers.CreateObject));

                foreach (var item in createObjects)
                {
                    CreateClientObject(item.Client, item.MessageData.Value, item.MessageData.Property);
                }

                IEnumerable<QueuedClientNetworkedObjectData> fieldsData = networkObjectQueue.Where(q => !q.MessageData.Object.StartsWith(LiNGSMarkers.CreateObject) && !q.MessageData.Object.StartsWith(LiNGSMarkers.DestroyObject));

                foreach (var item in fieldsData)
                {
                    ApplyClientObjectValue(item.Client, item.ObjectName, item.MessageData.Property, item.MessageData.Value);
                }

                networkObjectQueue.Clear();
            }

        }

        #endregion

    }
}
