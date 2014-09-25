using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Striker.States
{
    public abstract class GameState
    {
        protected GameController gameController;

        public GameState(GameController game)
        {
            this.gameController = game;
        }

        public virtual void OnEnter()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void OnExit()
        {

        }

    }
}
