using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Server
{
    [Serializable]
    internal class WeakReference<T> : WeakReference where T : class 
    {
        public WeakReference(T target) : base(target)
        {

        }

        public new T Target
        {
            get
            {
                return (T)base.Target;
            }

            set
            {
                base.Target = value;
            }
        }

    }
}
