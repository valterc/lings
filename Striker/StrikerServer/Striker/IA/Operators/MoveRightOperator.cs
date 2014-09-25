using GenericAI;
using Striker.Levels.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Striker.IA.Operators
{
    public class MoveRightOperator : Operator
    {
        public override State Apply(State state)
        {
            StrikerIAState strikerState = (StrikerIAState)state;
            Vector2 newPosition = new Vector2(strikerState.position.x + 1, strikerState.position.y);

            Element e = strikerState.level.GetMapTileNormalized((int)newPosition.x, (int)newPosition.y);

            if (e.canWalk)
            {
                strikerState = (StrikerIAState)state.Copy();
                strikerState.position = newPosition;
            }

            return strikerState;
        }
    }
}
