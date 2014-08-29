using LiNGS.Common;
using LiNGS.Common.GameCycle;
using LiNGS.Common.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LiNGS.Server.State
{
    internal class StateManager : IUpdatable
    {
        private LiNGSServer server;
        private List<ClientState> clientStates;
        private List<ClientState> newClients;
        private List<GameClient> disconnectedClients;
        internal IEnumerable<ClientState> ClientStates 
        {
            get 
            {
                return clientStates;
            }
        }

        internal StateManager(LiNGSServer server)
        {
            this.server = server;
            this.clientStates = new List<ClientState>();
            this.newClients = new List<ClientState>();
            this.disconnectedClients = new List<GameClient>();
        }

        private void UpdateStates()
        {
            IEnumerable<String> NetworkedObjectKeyCollection = server.GameLogicProcessor.NetworkedObjects.Keys;

            foreach (var obj in server.GameLogicProcessor.NetworkedObjects.Values)
            {
                foreach (var client in clientStates)
                {
                    if (!server.GameLogicProcessor.DoesClientNeedToKnowAboutObject(client.Client, obj))
                    {
                        if (client.State.Objects.ContainsKey(obj.Name))
                        {
                            ObjectStateHolder objStateHolder = client.State.Objects[obj.Name];
                            if (objStateHolder != null && objStateHolder.Activated)
                            {
                                objStateHolder.Activated = false;
                                objStateHolder.ActivatedKnown = false;
                            }
                        }
                        continue;
                    }

                    if (!client.State.Objects.ContainsKey(obj.Name))
                    {
                        ObjectStateHolder objStateHolder = new ObjectStateHolder() { Name = obj.Name, Type = obj.OriginalObject.GetType(), AutoCreateObject = obj.AutoCreateObject, Activated = true };
                        client.State.Objects.Add(obj.Name, objStateHolder);
                    }

                    ObjectStateHolder stateHolder = client.State.Objects[obj.Name];

                    if (!stateHolder.Activated)
                    {
                        stateHolder.Activated = true;
                        stateHolder.ActivatedKnown = false;
                    }

                    for (int i = 0; i < obj.Fields.Length; i++)
                    {
                        FieldInfo field = obj.Fields[i];
                        string fieldName = i.ToString();
                        String fieldValue = field.GetValue(obj.OriginalObject) != null ? field.GetValue(obj.OriginalObject).ToString() : null;

                        if (stateHolder.Fields.ContainsKey(fieldName))
                        {
                            if (stateHolder.Fields[fieldName].Value != fieldValue)
                            {
                                stateHolder.Fields[fieldName].Value = fieldValue;
                                stateHolder.Fields[fieldName].Known = false;
                                stateHolder.Fields[fieldName].Sent = false;
                            }
                        }
                        else
                        {
                            stateHolder.Fields.Add(fieldName, new FieldStateHolder() { Name = fieldName, Value = fieldValue });
                        }

                    }
                }
            }

            //TODO: Process this on state restoration
            //Remove dead objects from clients. This is also done in the RemoveObjectFromClients, 
            //but since saved states can be restored we update the their state here allowing for dead object removal
            foreach (var client in clientStates)
            {
                IEnumerable<String> ClientNetworkedObjectKeyCollection = client.State.Objects.Keys.ToList();
                foreach (var item in ClientNetworkedObjectKeyCollection.Where(o => !NetworkedObjectKeyCollection.Contains(o)))
	            {
                    bool deleted = client.State.Objects.Remove(item);
                    if (deleted)
                    {
                        NetworkMessage networkMessage = new NetworkMessage() { NeedsAck = true, Type = NetworkMessage.MessageType.Data };
                        networkMessage.Data.Add(new MessageData() { Object = LiNGSMarkers.DestroyObject, Value = item });

                        server.Manager.SendMessage(client.Client.NetworkClient, networkMessage);
                    }
	            }
            }

        }

        internal void RemoveObjectFromClients(string objectName, bool destroyObjectOnClient = true)
        {
            foreach (var item in clientStates)
            {
                bool deleted = item.State.Objects.Remove(objectName);
                if (deleted)
                {
                    if (destroyObjectOnClient)
                    {
                        NetworkMessage networkMessage = new NetworkMessage() { NeedsAck = true, Type = NetworkMessage.MessageType.Data };
                        networkMessage.Data.Add(new MessageData() { Object = LiNGSMarkers.DestroyObject, Value = objectName });

                        server.Manager.SendMessage(item.Client.NetworkClient, networkMessage);
                    }
                }
            }
        }

        internal void PersistClientState(GameClient client)
        {
            server.PersistentStateManager.SaveState(client);
        }

        internal bool HasClientState(GameClient client)
        {
            return server.PersistentStateManager.HasClientState(client);
        }

        internal bool AddClient(GameClient client, bool tryUseSavedState = false)
        {
            if (tryUseSavedState)
            {
                ClientState restoredClient = server.PersistentStateManager.RestoreState(client);
                if (restoredClient != null)
                {
                    newClients.Add(restoredClient);
                    return true;
                }
            }

            newClients.Add(new ClientState(client));
            return false;   
        }

        internal void RemoveClient(GameClient client)
        {
            disconnectedClients.Add(client);
        }
        
        #region IUpdatable Members

        public void Update(TimeSpan timeSinceLastUpdate)
        {
            clientStates.AddRange(newClients);
            clientStates.RemoveAll(c => disconnectedClients.Contains(c.Client));

            newClients.Clear();
            disconnectedClients.Clear();

            UpdateStates();
        }

        #endregion

    }
}
