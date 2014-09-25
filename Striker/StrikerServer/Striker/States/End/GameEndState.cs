using LiNGS.Common.Network;
using Striker.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Striker.States.End
{
    public class GameEndState : GameState
    {
        public GameEndState(GameController gameController)
            : base(gameController)
        {

        }

        public override void OnEnter()
        {
            for (int i = 0; i < gameController.characters.Count; i++)
            {
                Character c = gameController.characters[i];
                if (c.kills >= 15)
                {
                    foreach (var client in gameController.server.LogicProcessor.GetConnectedClients())
	                {
                        gameController.server.LogicProcessor.SendMessageTo(client, new MessageData() { Object = "GameEnd", Value = c.charName + ":" + c.kills });
	                }
                    break;
                }
            }
        }

        public override void Update()
        {

        }
    }
}
