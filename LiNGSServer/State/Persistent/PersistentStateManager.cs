using LiNGS.Server.State.Persistent.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LiNGS.Server.State.Persistent
{
    internal class PersistentStateManager
    {
        private LiNGSServer server;
        private bool enabled;
        private StorageEngine<ClientStateWrapper> storageEngine;

        internal PersistentStateManager(LiNGSServer server)
        {
            this.server = server;
            this.enabled = server.ServerProperties.EnablePersistentStates;
            this.storageEngine = new StorageEngine<ClientStateWrapper>(server.ServerProperties.SessionStorageBaseDirectory, GameClient.SessionGUID.ToString());
        }

        internal bool HasClientState(GameClient client)
        {
            if (!this.enabled)
            {
                return false;
            }

            return storageEngine.StateFileExists(client.UserId);
        }

        internal ClientState RestoreState(GameClient client)
        {
            ClientStateWrapper stateWrapper = storageEngine.RestoreState(client.UserId);

            //Recreate the client game state awareness

            ClientState clientState = new ClientState(client);

            foreach (var obj in stateWrapper.ClientState)
	        {
                ObjectStateHolder objState = new ObjectStateHolder() 
                { 
                    AutoCreateObject = obj.AutoCreateObject,
                    Known = obj.Known,
                    Name = obj.Name,
                    Sent = obj.Sent,
                    Type = Type.GetType(obj.TypeName),
                };

                for (int i = 0; i < obj.Fields.Count; i++)
                {
                    FieldStateWrapper fieldWrapper = obj.Fields[i];

                    objState.Fields.Add(i.ToString(), new FieldStateHolder() 
                    {
                        Known = fieldWrapper.Known,
                        Name = fieldWrapper.Name,
                        Sent = fieldWrapper.Sent,
                        Value = fieldWrapper.Value
                    });

                }

                clientState.State.Objects.Add(objState.Name, objState);
	        }

            //Recreate the clients objects in the server

            foreach (var item in stateWrapper.ClientObjects)
            {
                server.GameLogicProcessor.RestoreClientStateObject(client, item);
            }

            return clientState;
        }

        internal void SaveState(GameClient client)
        {
            List<ClientObjectStateWrapper> ClientObjects = new List<ClientObjectStateWrapper>();
            List<ClientObjectStateWrapper>  ClientState = new List<ClientObjectStateWrapper>();

            foreach (var item in server.StateManager.ClientStates.FirstOrDefault(cs => cs.Client == client).State.Objects.Values)
            {
                ClientObjectStateWrapper objectWrapper = new ClientObjectStateWrapper()
                {
                    Name = item.Name,
                    AutoCreateObject = item.AutoCreateObject,
                    Known = item.Known,
                    Sent = item.Sent,
                    TypeName = item.Type.AssemblyQualifiedName,
                    Fields = item.Fields.Values.Select(f => new FieldStateWrapper() { Known = f.Known, Name = f.Name, Sent = f.Sent, Value = f.Value }).ToList()
                };

                ClientState.Add(objectWrapper);
            }

            foreach (var item in server.GameLogicProcessor.ClientNetworkedObjects.Values.Where(co => co.Client == client))
            {
                ClientObjectStateWrapper objectWrapper = new ClientObjectStateWrapper()
                {
                    Name = item.Name,
                    AutoCreateObject = item.AutoCreateObject,
                    TypeName = item.OriginalObject.GetType().ToString(),
                    Fields = item.Fields.Select((f, index) => 
                    { 
                        Object oValue = f.GetValue(item.OriginalObject);
                        return new FieldStateWrapper() { Name = index.ToString(), Value = (oValue != null ? oValue.ToString() : null) };
                    }).ToList()
                };

                ClientObjects.Add(objectWrapper);
            }

            ClientStateWrapper stateWrapper = new ClientStateWrapper(ClientState, ClientObjects);
                
            storageEngine.SaveState(client.UserId, stateWrapper);
        }

        internal void ClearStates()
        {
            storageEngine.ClearStates();
        }

    }
}
