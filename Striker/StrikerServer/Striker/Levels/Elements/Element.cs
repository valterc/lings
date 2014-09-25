using System.Collections;

namespace Striker.Levels.Elements
{
    public abstract class Element
    {
        public bool canWalk;
        public bool transparent;
        public bool emitsLight;
        public bool isSpawnLocation;
        public bool isItemSpawner;

        public virtual void Start()
        {

        }

        public virtual void Update()
        {

        }
    }
}