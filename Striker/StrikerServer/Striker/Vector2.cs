using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Striker
{
    public struct Vector2
    {
        public readonly static Vector2 zero = new Vector2(0, 0);
        public float x;
        public float y;
        
        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2(Vector2 position)
        {
            this.x = position.x;
            this.y = position.y;
        }

        public static bool operator == (Vector2 v1, Vector2 v2){
            return v1.x == v2.x && v1.y == v2.y;
        }

        public static bool operator != (Vector2 v1, Vector2 v2)
        {
            return v1.x != v2.x || v1.y != v2.y;
        }
    }
}
