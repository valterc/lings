using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Server.State
{
    internal class FieldStateHolder
    {
        public String Name { get; set; }
        public String Value { get; set; }
        public bool Known { get; set; }
        public bool Sent { get; set; }
    }
}
