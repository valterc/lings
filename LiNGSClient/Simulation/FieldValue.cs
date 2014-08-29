using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Client.Simulation
{
    /// <summary>
    /// Represents a <see cref="LiNGS.Common.GameLogic.NetworkedField"/> value on a given time
    /// </summary>
    public class FieldValue
    {
        /// <summary>
        /// Value of the <see cref="LiNGS.Common.GameLogic.NetworkedField"/>.
        /// </summary>
        public String Value { get; internal set; }

        /// <summary>
        /// Time when the value was set.
        /// </summary>
        public DateTime Time { get; internal set; }

        /// <summary>
        /// Creates a new instance with a empty value and the Time set as now.
        /// </summary>
        public FieldValue()
        {
            this.Time = DateTime.Now;
        }

        /// <summary>
        /// Creates a new instance with the specified value and the Time set as now.
        /// </summary>
        /// <param name="value">The value of the field.</param>
        public FieldValue(String value)
        {
            this.Value = value;
            this.Time = DateTime.Now;
        }

    }
}
