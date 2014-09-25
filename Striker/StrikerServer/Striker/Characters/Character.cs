using LiNGS.Common.GameLogic;
using Striker.States.Play;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Striker.Characters
{
    public abstract class Character : INetworkedObject
    {

        [NetworkedField]
        public float positionX;

        [NetworkedField]
        public float positionY;

        [NetworkedField]
        public float rotation;

        [NetworkedField]
        public string charName;

        [NetworkedField]
        public int color;

        [NetworkedField]
        public bool dead;

        [NetworkedField]
        public int health;

        [NetworkedField]
        public int kills;

        [NetworkedField]
        public bool disconnected;

        protected DateTime lastFireTime;

        protected DateTime spawnTime;
        protected DateTime deathTime;

        public Character(int seed)
        {
            charName = CharName.GetName();
            Random random = new Random(seed);
            string r = random.Next(100, 256).ToString("X");
            string g = random.Next(100, 256).ToString("X");
            string b = random.Next(100, 256).ToString("X");
            color = int.Parse(r + g + b, System.Globalization.NumberStyles.HexNumber | System.Globalization.NumberStyles.AllowHexSpecifier);

            spawnTime = DateTime.Now;
            health = 10;
        }

        public Character()
        {
            charName = CharName.GetName();
            Random random = new Random();
            string r = random.Next(100, 256).ToString("X");
            string g = random.Next(100, 256).ToString("X");
            string b = random.Next(100, 256).ToString("X");
            color = int.Parse(r + g + b, System.Globalization.NumberStyles.HexNumber | System.Globalization.NumberStyles.AllowHexSpecifier);

            spawnTime = DateTime.Now;
            health = 10;
        }

        internal virtual void Update()
        {
            if (!this.dead && health <= 0)
            {
                Die();
            }

            if (this.dead && DateTime.Now - deathTime > TimeSpan.FromSeconds(3))
            {
                Respawn();
            }
        }

        public override string ToString()
        {
            return String.Format("Character: name=\"{0}\", PositionX=\"{1}\", PositionY=\"{2}\"", charName, positionX, positionY);
        }

        public double DistanceFromOther(Character c)
        {
            return Math.Sqrt(Math.Pow((this.positionX - c.positionX), 2) + Math.Pow((this.positionY - c.positionY), 2));
        }

        internal double DistanceFrom(float sX, float sY)
        {
            return Math.Sqrt(Math.Pow((this.positionX - sX), 2) + Math.Pow((this.positionY - sY), 2));
        }

        protected virtual void Die()
        {
            this.dead = true;
            deathTime = DateTime.Now;
        }

        protected virtual void Respawn()
        {
            SpawnPoint sp = GameController.instance.GetSpawnLocation(this);

            this.health = 10;
            this.positionX = sp.x;
            this.positionY = sp.y;
            this.dead = false;
            this.spawnTime = DateTime.Now;
        }

    }
}
