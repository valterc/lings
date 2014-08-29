using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Client.Simulation.Simulators
{
    /// <summary>
    /// Performs simulation on a field value. Simulates a value for a object field given the history of that field value.
    /// </summary>
    public abstract class SimulatorLogic
    {

        /// <summary>
        /// Runs the simulation on a Simulated <see cref="LiNGS.Common.GameLogic.NetworkedField"/>
        /// </summary>
        /// <param name="fieldObject">Field to perform simulation</param>
        public abstract void RunSimulation(SimulatedObjectField fieldObject);

    }
}
