using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LiNGS.Client.Simulation.Simulators
{
    /// <summary>
    /// Simulates the value for a Simulated <see cref="LiNGS.Common.GameLogic.NetworkedField"/> using a Linear Extrapolation method.
    /// </summary>
    public class LinearExtrapolationSimulatorLogic : SimulatorLogic
    {
        /// <summary>
        /// Created a new object of this class.
        /// </summary>
        public LinearExtrapolationSimulatorLogic()
        {

        }

        /// <summary>
        /// Runs the simulation on a Simulated <see cref="LiNGS.Common.GameLogic.NetworkedField"/> following a Linear Extrapolation logic adjusted for network simulation.
        /// </summary>
        /// <param name="objectField">Field to perform simulation.</param>
        public override void RunSimulation(SimulatedObjectField objectField)
        {
            double[] intValues = null;

            try
            {
                intValues = objectField.LastReceivedFieldValues.Select(fv => double.Parse(fv.Value)).ToArray();
            }
            catch (Exception)
            {
                return;
            }

            double newValue = 0;

            if (intValues.Length > 1)
            {
                if (DateTime.Now - objectField.LastReceivedFieldValues[intValues.Length - 1].Time > TimeSpan.FromMilliseconds(200))
                {
                    newValue = Lerp(double.Parse(objectField.LastSimulatedValue), double.Parse(objectField.LastReceivedFieldValues[intValues.Length - 1].Value), .15f);
                    objectField.LastSimulatedValue = newValue.ToString();
                    objectField.LastReceivedFieldValues[intValues.Length - 1].Time = DateTime.Now - TimeSpan.FromMilliseconds(200);
                }
                else 
                {
                    newValue = intValues[intValues.Length - 2] + (DateTime.Now - TimeSpan.FromMilliseconds(200) - objectField.LastReceivedFieldValues[intValues.Length - 2].Time).TotalMilliseconds / Math.Max(0.1, (objectField.LastReceivedFieldValues[intValues.Length - 1].Time - objectField.LastReceivedFieldValues[intValues.Length - 2].Time).TotalMilliseconds) * (intValues[intValues.Length - 1] - intValues[intValues.Length - 2]);
                    objectField.LastSimulatedValue = newValue.ToString();
                }
            }
            else
            {
                newValue = intValues[intValues.Length - 1];
            }

            objectField.Field.SetValue(objectField.NetworkedObject.OriginalObject, Convert.ChangeType(newValue, objectField.Field.FieldType, CultureInfo.InvariantCulture));

        }

        private double Lerp(double start, double end, float percent)
        {
             return (start + percent * (end - start));
        }

    }
}
