using LiNGS.Client.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LiNGS.Client.Simulation
{
    /// <summary>
    /// Information of a Simulated <see cref="LiNGS.Common.GameLogic.NetworkedField"/> to be used for simulation
    /// </summary>
    public class SimulatedObjectField
    {
        internal NetworkedObject NetworkedObject { get; set; }
        internal FieldInfo Field { get; set; }
        internal String FieldName { get; set; }
        internal String LastSimulatedValue { get; set; }

        /// <summary>
        /// List of previous values of the <see cref="LiNGS.Common.GameLogic.NetworkedField"/>
        /// </summary>
        public List<FieldValue> LastReceivedFieldValues { get; private set; }

        internal SimulatedObjectField()
        {
            LastReceivedFieldValues = new List<FieldValue>();
        }

        internal void ReceivedValue(String value)
        {
            if (LastReceivedFieldValues.Count > 10)
            {
                LastReceivedFieldValues.RemoveAt(0);
            }

            LastReceivedFieldValues.Add(new FieldValue(value)); 
        }

    }
}
