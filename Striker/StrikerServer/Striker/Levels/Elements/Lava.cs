using System.Collections;

namespace Striker.Levels.Elements
{
    public class Lava : Element
    {

        public override void Start()
        {
            canWalk = false;
            transparent = true;
            emitsLight = true;
        }

        public override void Update()
        {

        }
    }
}
