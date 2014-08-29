using LiNGS.Common;
using LiNGS.Common.GameCycle;
using LiNGS.Common.GameLogic;
using LiNGS.Common.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LiNGS.Client.Synchronization
{
    internal class Synchronizer: IUpdatable
    {
        private LiNGSClient client;
        private Dictionary<string, SynchronizedObject> SynchronizedObjects;

        internal Synchronizer(LiNGSClient client)
        {
            this.client = client;
            this.SynchronizedObjects = new Dictionary<string, SynchronizedObject>();
        }

        internal void RemoveObject(INetworkedObject networkedObject, bool deleteFromServer = true)
        {
            List<String> objectKeys = SynchronizedObjects.Where(kv => kv.Value.Object.OriginalObject == networkedObject).Select(kv => kv.Key).ToList();
            foreach (var item in objectKeys)
            {
                SynchronizedObjects.Remove(item);

                if (deleteFromServer)
                {
                    NetworkMessage networkMessage = new NetworkMessage() { NeedsAck = true, Type = NetworkMessage.MessageType.Data };
                    networkMessage.Data.Add(new MessageData() { Object = LiNGSMarkers.DestroyObject, Value = item });
                    client.Manager.SendMessage(networkMessage);
                }
            }
        }

        #region IUpdatable Members

        public void Update(TimeSpan timeSinceLastUpdate)
        {
            NetworkMessage serverMessage = new NetworkMessage(NetworkMessage.MessageType.Data);
            bool anyChange = false;

            foreach (var item in client.ClientLogicProcessor.NetworkedLocalObjects.Keys)
            {
                if (SynchronizedObjects.ContainsKey(item))
                {
                    List<String> keys = SynchronizedObjects[item].FieldsValue.Keys.ToList();
                    foreach (var key in keys)
                    {
                        FieldInfo field = client.ClientLogicProcessor.NetworkedLocalObjects[item].Fields[int.Parse(key)];
                        string fieldValue = field.GetValue(client.ClientLogicProcessor.NetworkedLocalObjects[item].OriginalObject) != null ? field.GetValue(client.ClientLogicProcessor.NetworkedLocalObjects[item].OriginalObject).ToString() : null;

                        if (SynchronizedObjects[item].FieldsValue[key] != fieldValue)
                        {
                            SynchronizedObjects[item].FieldsValue[key] = fieldValue;

                            MessageData data = new MessageData() { Object = item, Property = key, Value = SynchronizedObjects[item].FieldsValue[key] };
                            serverMessage.Data.Add(data);
                            anyChange = true;
                        }                        
                    }
                }
                else
                {
                    SynchronizedObjects.Add(item, new SynchronizedObject(item, client.ClientLogicProcessor.NetworkedLocalObjects[item]));

                    serverMessage.NeedsAck = true;
                    serverMessage.Data.Add(new MessageData() { Object = LiNGSMarkers.CreateObject, Property = item, Value = client.ClientLogicProcessor.NetworkedLocalObjects[item].OriginalObject.GetType().ToString() });

                    foreach (var kv in SynchronizedObjects[item].FieldsValue)
                    {
                        MessageData data = new MessageData() { Object = item, Property = kv.Key, Value = kv.Value };
                        serverMessage.Data.Add(data);
                        anyChange = true;
                    }

                }
            }

            if (anyChange)
            {
                client.Manager.SendMessage(serverMessage);
            }
            
        }

        #endregion

    }
}
