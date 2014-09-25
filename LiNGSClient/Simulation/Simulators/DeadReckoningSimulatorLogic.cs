using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LiNGS.Client.Simulation.Simulators
{
    /// <summary>
    /// Simulated a field values based on the 'Dead Reckoning' algorithm.
    /// </summary>
    public class DeadReckoningSimulatorLogic : SimulatorLogic
    {
        /// <summary>
        /// Creates a new object of this class.
        /// </summary>
        public DeadReckoningSimulatorLogic()
        {

        }

        /// <summary>
        /// Performs the simulation of the field based on the field lastest values.
        /// </summary>
        /// <param name="lastValues">The field lastest values</param>
        public override void RunSimulation(SimulatedObjectField lastValues)
        {
            double[] values = null;

            try
            {
                values = lastValues.LastReceivedFieldValues.Select(fv => double.Parse(fv.Value)).ToArray();
            }
            catch (Exception)
            {
                return;
            }

            if (values.Length < 2)
            {
                if (values.Length > 0)
                {
                    double value = values[0];
                    SetFieldValue(lastValues, value);
                }

                return;
            }


            double acc = values[values.Length - 1] - values[values.Length - 2];
            DateTime time = lastValues.LastReceivedFieldValues[values.Length - 1].Time;
            double currentValue = GetFieldValue(lastValues);
            double newValue = 0;

            if (lastValues.LastSimulatedValue != lastValues.LastReceivedFieldValues[values.Length - 1].Value)
            {
                newValue = values[values.Length - 1] + acc * (DateTime.Now - time).TotalSeconds;
            }
            else
            {
                double elapsedTimeSinceLastConnect = (DateTime.Now - time).TotalMilliseconds;
                double maxAllowedTime = 250;
                double currentTime = maxAllowedTime - elapsedTimeSinceLastConnect;

                if (currentTime < 0)
                {
                    acc = 0;
                }
                else
                {
                    double accP = currentTime / maxAllowedTime;
                    acc *= accP;
                }

                newValue = currentValue + acc * (DateTime.Now - time).TotalSeconds;
            }

            lastValues.LastSimulatedValue = lastValues.LastReceivedFieldValues[values.Length - 1].Value;

            SetFieldValue(lastValues, newValue);
        }

        private void SetFieldValue(SimulatedObjectField objectField, double value)
        {
            objectField.Field.SetValue(objectField.NetworkedObject.OriginalObject, Convert.ChangeType(value, objectField.Field.FieldType, CultureInfo.InvariantCulture));
        }

        private double GetFieldValue(SimulatedObjectField objectField)
        {
            return Double.Parse(objectField.Field.GetValue(objectField.NetworkedObject.OriginalObject).ToString());
        }

    }
}
