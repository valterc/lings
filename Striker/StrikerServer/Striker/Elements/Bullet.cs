using LiNGS.Common.GameLogic;
using Striker.Characters;
using Striker.Levels;
using Striker.Levels.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Striker.Elements
{
    public class Bullet : INetworkedObject
    {
        [NetworkedField]
        public float startPositionX;

        [NetworkedField]
        public float startPositionY;

        [NetworkedField]
        public float directionX;

        [NetworkedField]
        public float directionY;

        [NetworkedField]
        public float rotation;

        private DateTime spawnTime;
        private Level level;
        private bool toRemove;
        private float positionX;
        private float positionY;
        public Character character;

        public Bullet(Level level, Character character)
        {
            this.level = level;
            this.character = character;

            startPositionX = character.positionX;
            startPositionY = character.positionY;
            rotation = character.rotation;
            directionX = (float)Math.Sin(0.0174533f * rotation); //Magic number is conversion from Deg to Rad
            directionY = -(float)Math.Cos(0.0174533f * rotation);

            spawnTime = DateTime.Now;
            /*
            b.startPositionX = player.transform.position.x;
            b.startPositionY = player.transform.position.y;
            b.rotation = player.transform.eulerAngles.z;
            b.directionX = Mathf.Sin(Mathf.Deg2Rad * player.transform.eulerAngles.z);
            b.directionY = -Mathf.Cos(Mathf.Deg2Rad * player.transform.eulerAngles.z);
            */
        }

        public bool Update()
        {
            if (toRemove)
            {
                 return true;
            }

            TimeSpan aliveTime = DateTime.Now - spawnTime;

            positionX = (float)(startPositionX + directionX * aliveTime.TotalSeconds * 8);
            positionY = (float)(startPositionY + directionY * aliveTime.TotalSeconds * 8);

            Element e = level.GetMapTile(positionX, positionY);
            if (e != null)
            {
                if (!e.transparent)
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

            if (aliveTime > TimeSpan.FromSeconds(2.5f))
            {
                return true;
            }

            return false;
        }

        public bool CheckCollision(Character c)
        {
            if (c.DistanceFrom(positionX, positionY) < 2f)
            {
                toRemove = true;
                return true;
            }
            return false;
        }

    }
}
