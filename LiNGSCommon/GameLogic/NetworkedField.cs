using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Common.GameLogic
{
    /// <summary>
    /// Represent a attribute that indicates that a field should be synchronized by the LiNGS System.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class NetworkedField : Attribute
    {
        /// <summary>
        /// Indicates if a field should be simulated.
        /// </summary>
        public bool Simulated { get; set; }

    }
}
