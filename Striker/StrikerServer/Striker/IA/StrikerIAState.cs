using GenericAI;
using Striker.Levels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Striker.IA
{
    public class StrikerIAState : State
    {
        public Level level;
        public Vector2 position;
        public Vector2 goalPosition;

        private StrikerIAState()
        {

        }

        public StrikerIAState(Vector2 goalPosition, Vector2 position, Level level)
        {
            this.level = level;
            this.position = position;
            this.goalPosition = goalPosition;
        }

        public StrikerIAState(Vector2 goalPosition)
        {
            this.goalPosition = goalPosition;
        }

        public override State Copy()
        {
            return new StrikerIAState() { level = this.level, position = new Vector2(position), goalPosition = goalPosition };
        }

        public override bool Equals(State other)
        {
            StrikerIAState strikerState = other as StrikerIAState;
            return strikerState.position == position;
        }

        public override bool IsGoal()
        {
            return position == goalPosition;
        }
    }
}
