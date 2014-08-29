using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Server.State
{
    internal class ObjectStateHolder
    {
        public String Name { get; set; }
        public Type Type { get; set; }
        public bool Known { get; set; }
        public bool Sent { get; set; }
        public bool AutoCreateObject { get; set; }
        public bool Activated { get; set; }
        public bool ActivatedKnown { get; set; }
        public Dictionary<string, FieldStateHolder> Fields { get; private set; }

        public ObjectStateHolder()
        {
            Fields = new Dictionary<string, FieldStateHolder>();
        }

    }
}
