using LiNGS.Client.GameLogic;
using LiNGS.Client.Simulation.Simulators;
using LiNGS.Common.GameCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Client.Simulation
{
    internal class Simulator : IUpdatable
    {
        private LiNGSClient client;
        private List<SimulatedObjectField> simulatedFields;
        private SimulatorLogic simulatorLogic;

        internal Simulator(LiNGSClient client)
        {
            this.client = client;
            this.simulatedFields = new List<SimulatedObjectField>();
            this.simulatorLogic = client.ClientProperties.FieldSimulationLogic;
        }

        internal void NewFieldValue(NetworkedObject networkedObject, string fieldName, string value)
        {
            SimulatedObjectField simulatedObjectField = simulatedFields.FirstOrDefault(sf => sf.NetworkedObject == networkedObject && sf.FieldName == fieldName);
            if (simulatedObjectField != null)
            {
                simulatedObjectField.ReceivedValue(value);
            }
            else
            {
                networkedObject.ReceiveValue(fieldName, value);
            }
        }

        internal void RegisterNetworkedObject(NetworkedObject networkedObject)
        {
            for (int i = 0; i < networkedObject.Fields.Length; i++)
            {
                if (networkedObject.IsFieldSimulated[i])
                {
                    simulatedFields.Add(new SimulatedObjectField() { Field = networkedObject.Fields[i], FieldName = i.ToString(), NetworkedObject = networkedObject });
                }
            }
        }

        internal void UnregisterNetworkedObject(NetworkedObject networkedObject)
        {
            simulatedFields.RemoveAll(sf => sf.NetworkedObject == networkedObject);
        }

        #region IUpdatable Members

        public void Update(TimeSpan timeSinceLastUpdate)
        {
            foreach (var item in simulatedFields)
            {
                simulatorLogic.RunSimulation(item);
            }
        }

        #endregion

    }
}
