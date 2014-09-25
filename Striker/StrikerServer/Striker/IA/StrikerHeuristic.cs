using GenericAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Striker.IA
{
    public class StrikerHeuristic : Heuristic
    {
        public override int CostBetweenStates(State state0, State state1)
        {
            return 1;
        }

        public override int EstimateCostToGoal(State current)
        {
            StrikerIAState strikerState = current as StrikerIAState;
            return (int)(Math.Abs(strikerState.position.x - strikerState.goalPosition.x) + Math.Abs(strikerState.position.y - strikerState.goalPosition.y));
        }
    }
}
