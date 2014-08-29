using LiNGS.Common;
using LiNGS.Common.GameCycle;
using LiNGS.Common.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LiNGS.Server.State
{
    internal class Dispatcher : IUpdatable
    {
        private LiNGSServer server;
        private StateManager stateManager;

        public Dispatcher(LiNGSServer server)
        {
            this.server = server;
            stateManager = server.StateManager;
        }

        private void DispatchInformationToClients()
        {
            foreach (var client in stateManager.ClientStates)
            {
                NetworkMessage clientMessage = new NetworkMessage(NetworkMessage.MessageType.Data);

                foreach (var obj in client.State.Objects.Values)
                {
                    /* Already done in the StateManager.UpdateState
                    if (!server.GameLogicProcessor.DoesClientNeedToKnowAboutObject(client.Client, obj))
                    {
                        continue;
                    }
                    */
                    if (!obj.ActivatedKnown)
                    {
                        obj.ActivatedKnown = true;
                        MessageData data = new MessageData() { Object = obj.Name, Property = LiNGSMarkers.SetActive, Value = obj.Activated.ToString() };
                        clientMessage.NeedsAck = true;
                        clientMessage.Data.Add(data);
                    }

                    if (!obj.Sent && obj.AutoCreateObject)
                    {
                        MessageData odata = new MessageData() { Object = LiNGSMarkers.CreateObject, Property = obj.Name, Value = obj.Type.ToString() };
                        clientMessage.Data.Add(odata);

                        foreach (var item in obj.Fields.Values)
                        {
                            MessageData data = new MessageData() { Object = obj.Name, Property = item.Name, Value = item.Value };

                            clientMessage.Data.Add(data);
                            item.Sent = true;
                        }

                        obj.Sent = true;
                        clientMessage.NeedsAck = true;
                    }
                    else
                    {
                        foreach (var item in obj.Fields.Values.Where(f => f.Sent == false))
                        {
                            MessageData data = new MessageData() { Object = obj.Name, Property = item.Name, Value = item.Value };

                            clientMessage.Data.Add(data);
                            item.Sent = true;
                        }
                    }



                }

                if (clientMessage.Data.Count > 0)
                {
                    server.Manager.SendMessage(client.Client.NetworkClient, clientMessage);
                }
            }
        }

        #region IUpdatable Members

        public void Update(TimeSpan timeSinceLastUpdate)
        {
            DispatchInformationToClients();
        }

        #endregion


    }
}
