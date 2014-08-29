using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Common.GameCycle
{
    /// <summary>
    /// Represents an object that can be updated. 
    /// This object will be updated after the <see cref="IUpdatable"/> objects.
    /// </summary>
    public interface ILateUpdatable
    {
        /// <summary>
        /// Update the object.
        /// </summary>
        /// <param name="timeSinceLastLateUpdate">Time elapsed since this function was last called.</param>
        void LateUpdate(TimeSpan timeSinceLastLateUpdate);
    }
}
