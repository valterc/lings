using System.Collections;

namespace Striker.Levels.Elements
{
    public class Wall : Element
    {

        public override void Start()
        {
            canWalk = false;
            transparent = false;
            emitsLight = false;
        }

        public override void Update()
        {

        }
    }
}
