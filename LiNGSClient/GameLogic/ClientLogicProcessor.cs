using LiNGS.Common;
using LiNGS.Common.GameCycle;
using LiNGS.Common.GameLogic;
using LiNGS.Common.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LiNGS.Client.GameLogic
{
    /// <summary>
    /// Handles the client side logic of a networked game.
    /// </summary>
    public class ClientLogicProcessor : IUpdatable
    {
        private LiNGSClient client;
        private List<QueuedNetworkedObjectData> networkObjectQueue;
        internal Dictionary<string, NetworkedObject> NetworkedObjects { get; private set; }
        internal Dictionary<string, NetworkedObject> NetworkedLocalObjects { get; private set; }

        internal ClientLogicProcessor(LiNGSClient client)
        {
            this.client = client;
            this.networkObjectQueue = new List<QueuedNetworkedObjectData>();

            this.NetworkedObjects = new Dictionary<string, NetworkedObject>();
            this.NetworkedLocalObjects = new Dictionary<string, NetworkedObject>();
        }

        internal void ReceiveDataMessage(NetworkMessage message)
        {
            lock (networkObjectQueue)
            {
                foreach (var item in message.Data)
                {
                    networkObjectQueue.Add(new QueuedNetworkedObjectData() { MessageData = item, ObjectName = item.Object });
                }
            }
        }

        internal void ReceiveGameMessage(NetworkMessage message)
        {
            client.NetworkedClientInstance.ReceiveGameMessage(message);
        }

        internal void ReceiveEventMessage(NetworkMessage message)
        {
            client.NetworkedClientInstance.ReceiveEventMessage(message);
        }

        internal void ReceiveErrorMessage(NetworkMessage message)
        {
            client.NetworkedClientInstance.ReceiveErrorMessage(message);
        }

        internal void CreateObject(string typeName, String name)
        {
            if (!NetworkedObjects.ContainsKey(name))
            {
                INetworkedObject networkedObject = client.NetworkedClientInstance.CreateObject(typeName, name);
                NetworkedObjects.Add(name, new NetworkedObject(networkedObject, name));
                client.Simulator.RegisterNetworkedObject(NetworkedObjects[name]);
            }
        }

        internal void DestroyObject(String name)
        {
            if (NetworkedObjects.ContainsKey(name))
            {
                client.NetworkedClientInstance.DestroyObject(NetworkedObjects[name].OriginalObject, name);
                client.Simulator.UnregisterNetworkedObject(NetworkedObjects[name]);
                NetworkedObjects.Remove(name);
            }
        }

        /// <summary>
        /// Registers a local created <see cref="INetworkedObject"/> to be update with data from the server. This object will not be syncronized on the server.
        /// </summary>
        /// <param name="networkedObject">The networked object</param>
        /// <param name="objectName">UNIQUE object name</param>
        public void RegisterNetworkAwareObject(INetworkedObject networkedObject, string objectName)
        {
            NetworkedObjects.Add(objectName, new NetworkedObject(networkedObject, objectName));
            client.Simulator.RegisterNetworkedObject(NetworkedObjects[objectName]);
        }

        /// <summary>
        /// Unregisters a local created <see cref="INetworkedObject"/>. The object will no longer be updated with data received from the server.
        /// </summary>
        /// <param name="networkedObject">The networked object</param>
        public void UnregisterNetworkAwareObject(INetworkedObject networkedObject)
        {
            var noKv = NetworkedObjects.FirstOrDefault(kv => kv.Value.OriginalObject == networkedObject);
            if (noKv.Key != null)
            {
                client.Simulator.UnregisterNetworkedObject(noKv.Value);
                NetworkedObjects.Remove(noKv.Key);
            }
        }

        /// <summary>
        /// Register a <see cref="INetworkedObject"/> created on the client that will be synchronized to the server.
        /// </summary>
        /// <param name="networkedObject">The networked object</param>
        /// <param name="name">UNIQUE name of the networked object</param>
        public void RegisterNetworkedObject(INetworkedObject networkedObject, string name)
        {
            if (NetworkedLocalObjects.ContainsKey(name))
            {
                throw new ArgumentException("A NetworkObject with the specified name was already registered.");
            }

            NetworkedObject no = new NetworkedObject(networkedObject, name);
            NetworkedLocalObjects.Add(name, no);
        }

        /// <summary>
        /// Unregisters a local <see cref="INetworkedObject"/>. The object will be destroyed on the server.
        /// </summary>
        /// <param name="networkedObject">The networked object to destroy.</param>
        public void UnregisterNetworkedObject(INetworkedObject networkedObject)
        {
            var noKv = NetworkedLocalObjects.FirstOrDefault(kv => kv.Value.OriginalObject == networkedObject);
            if (noKv.Key != null)
            {
                NetworkedLocalObjects.Remove(noKv.Key);
                client.Synchronizer.RemoveObject(networkedObject);
            }
        }

        /// <summary>
        /// Unregisters a local <see cref="INetworkedObject"/>. The object will be destroyed on the server.
        /// </summary>
        /// <param name="name">UNIQUE name of the networked object.</param>
        public void UnregisterNetworkedObject(String name)
        {
            if (NetworkedLocalObjects.ContainsKey(name))
            {
                client.Synchronizer.RemoveObject(NetworkedLocalObjects[name].OriginalObject);
                NetworkedLocalObjects.Remove(name);
            }
        }

        #region IUpdatable Members

        /// <summary>
        /// Update the component internals. Do not call this directly.
        /// </summary>
        /// <param name="timeSinceLastUpdate">Elapsed time since this function was last called.</param>
        public void Update(TimeSpan timeSinceLastUpdate)
        {
            lock (networkObjectQueue)
            {
                //Order is:
                //Destroy objects
                //Create objects
                //Update objects values

                //TODO: Changed order

                IEnumerable<QueuedNetworkedObjectData> createObjects = networkObjectQueue.Where(q => q.MessageData.Object.StartsWith(LiNGSMarkers.CreateObject));

                foreach (var item in createObjects)
                {
                    CreateObject(item.MessageData.Value, item.MessageData.Property);
                }

                IEnumerable<QueuedNetworkedObjectData> destroyObjects = networkObjectQueue.Where(q => q.MessageData.Object.StartsWith(LiNGSMarkers.DestroyObject));

                foreach (var item in destroyObjects)
                {
                    DestroyObject(item.MessageData.Value);
                }

                IEnumerable<QueuedNetworkedObjectData> fieldsData = networkObjectQueue.Where(q => !q.MessageData.Object.StartsWith(LiNGSMarkers.CreateObject) && !q.MessageData.Object.StartsWith(LiNGSMarkers.DestroyObject));

                foreach (var item in fieldsData)
                {
                    NetworkedObject no;
                    if (NetworkedObjects.TryGetValue(item.ObjectName, out no))
                    {
                        if (item.MessageData.Property == LiNGSMarkers.SetActive)
                        {
                            no.SetActive(bool.Parse(item.MessageData.Value));
                        }
                        else
                        {
                            client.Simulator.NewFieldValue(no, item.MessageData.Property, item.MessageData.Value);
                        }
                    }
                }

                networkObjectQueue.Clear();
            }


        }

        #endregion

    }
}
