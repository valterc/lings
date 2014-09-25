using GenericAI;
using LiNGS.Common.GameLogic;
using Striker.Elements;
using Striker.IA;
using Striker.IA.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Striker.Characters
{
    public class NPC : Character
    {
        private Character enemy;
        private Vector2 focusedPosition;
        private Vector2 targetPosition;
        private Vector2 startPosition;
        private float moveTicker;
        private DateTime lastMoveTick;

        public NPC(int seed)
            : base(seed)
        {

        }

        internal override void Update()
        {
            base.Update();

            if (dead)
            {
                return;
            }

            if (enemy != null)
            {
                if (enemy.dead || enemy.disconnected || this.DistanceFromOther(enemy) > 15)
                {
                    enemy = null;
                }
                else
                {
                    if (targetPosition == Vector2.zero)
                    {
                        MoveTowardsEnemy();  
                    }                    

                    if (this.DistanceFromOther(enemy) < 5)
                    {
                        if (DateTime.Now - lastFireTime > TimeSpan.FromMilliseconds(400))
                        {
                            lastFireTime = DateTime.Now;
                            Bullet b = new Bullet(GameController.instance.level, this);
                            GameController.instance.bullets.Add(b);
                            GameController.instance.server.LogicProcessor.RegisterNetworkedObject(b);
                        }
                    }

                }
            }
            else
            {
                foreach (var item in GameController.instance.characters)
                {
                    if (item != this)
                    {
                        if (!item.dead && !item.disconnected && this.DistanceFromOther(item) < 15)
                        {
                            enemy = item;
                            focusedPosition = Vector2.zero;
                        }
                    }
                }

                if (enemy == null)
                {
                    if (targetPosition == Vector2.zero)
                    {
                        MoveToRandomPosition();  
                    }
                }
            }

            if (targetPosition != Vector2.zero)
            {
                MoveToTargetPosition();
                FaceTargetPosition();
                return;
            }

        }

        protected override void Respawn()
        {
            base.Respawn();
            focusedPosition = Vector2.zero;
            targetPosition = Vector2.zero;
            enemy = null;
        }

        private void FaceTargetPosition()
        {
            FacePosition(focusedPosition.x, focusedPosition.y);
        }

        private void FacePosition(float x, float y)
        {
            float xDiff = positionX - x; 
            float yDiff = positionY - y;
            rotation = (float)(Math.Atan2(yDiff, xDiff) * (180 / Math.PI) - 90); 
        }

        private void MoveToRandomPosition()
        {
            if (focusedPosition == Vector2.zero || (Math.Abs(positionX - focusedPosition.x) < 2f && Math.Abs(positionY - focusedPosition.y) < 2f))
            {
                focusedPosition = GameController.instance.GetRandomFloorLocation();
            }

            MoveToPosition(focusedPosition);
        }

        private void MoveTowardsEnemy()
        {
            MoveToPosition(new Vector2(enemy.positionX, enemy.positionY));
        }

        private void MoveToPosition(Vector2 position)
        {
            focusedPosition = position;

            int tpX = (int)((position.x + 1f) / 2f);
            int tpY = (int)(GameController.instance.level.Height - ((position.y + 1f) / 2f));

            position = new Vector2(tpX, tpY);

            int mpX = (int)((positionX + 1f) / 2f);
            int mpY = (int)(GameController.instance.level.Height - ((positionY + 1f) / 2f));
            Vector2 myPosition = new Vector2(mpX, mpY);

            StrikerIAState strikerState = new StrikerIAState(position, myPosition, GameController.instance.level);
            GenericAI.AAsterisk a = new GenericAI.AAsterisk(strikerState,
                new List<GenericAI.Operator>() 
                { 
                    new MoveUpOperator(), 
                    new MoveDownOperator(), 
                    new MoveLeftOperator(), 
                    new MoveRightOperator() 
                },
                new StrikerHeuristic());

            a.OnSearchComplete += a_OnSearchComplete;
            a.StartSearch();

            while (a.IsRunning)
            {
            }
        }

        void a_OnSearchComplete(object sender, GenericAI.ResultEventArgs e)
        {
            if (e.Result != null && e.Result.Operations != null && e.Result.Operations.Count > 0)
            {
                Operator nextOperator = e.Result.Operations[0];

                startPosition = new Vector2(positionX, positionY);
                moveTicker = 0;

                if (nextOperator is MoveUpOperator)
                {
                    targetPosition = new Vector2(positionX, positionY + 2f);
                }
                else if (nextOperator is MoveDownOperator)
                {
                    targetPosition = new Vector2(positionX, positionY - 2f);
                }
                else if (nextOperator is MoveLeftOperator)
                {
                    targetPosition = new Vector2(positionX - 2f, positionY);
                }
                else if (nextOperator is MoveRightOperator)
                {
                    targetPosition = new Vector2(positionX + 2f, positionY);
                }
                lastMoveTick = DateTime.Now;
            }
        }

        private void MoveToTargetPosition()
        {
            TimeSpan timeElapsed = DateTime.Now - lastMoveTick;
            if (timeElapsed > TimeSpan.FromMilliseconds(5))
            {
                moveTicker = Math.Min(moveTicker + 0.02f, 1f);
                lastMoveTick = DateTime.Now;
            }
            

            positionY = lerp(startPosition.y, targetPosition.y, moveTicker);
            positionX = lerp(startPosition.x, targetPosition.x, moveTicker);

            if (Math.Abs(positionX - targetPosition.x) < 0.01 && Math.Abs(positionY - targetPosition.y) < 0.01)
            {
                positionY = targetPosition.y;
                positionX = targetPosition.x;
                targetPosition = Vector2.zero;
                startPosition = Vector2.zero;
            }

        }

        float lerp(float v0, float v1, float t)
        {
            return (1 - t) * v0 + t * v1;
        }

    }
}
