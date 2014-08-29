using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Common.GameCycle
{
    /// <summary>
    /// Represents an object that can be updated.
    /// </summary>
    public interface IUpdatable
    {
        /// <summary>
        /// Update the object.
        /// </summary>
        /// <param name="timeSinceLastUpdate">Time elapsed since this function was last called.</param>
        void Update(TimeSpan timeSinceLastUpdate);
    }
}
