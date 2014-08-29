using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Server.State
{
    internal class StateHolder
    {
        public Dictionary<string, ObjectStateHolder> Objects { get; private set; }

        public StateHolder()
        {
            Objects = new Dictionary<string, ObjectStateHolder>();
        }

    }
}
