using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Common.GameCycle
{
    /// <summary>
    /// Manage the updating of <see cref="IUpdatable"/> and <see cref="ILateUpdatable"/> objects.
    /// </summary>
    public class UpdateManager
    {
        private List<WeakReference<IUpdatable>> updatables;
        private List<WeakReference<ILateUpdatable>> lateUpdatables;
        private DateTime lastUpdateTime;
        private DateTime lastLateUpdateTime;

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        public UpdateManager()
        {
            this.updatables = new List<WeakReference<IUpdatable>>();
            this.lateUpdatables = new List<WeakReference<ILateUpdatable>>();

            this.lastUpdateTime = DateTime.Now;
            this.lastLateUpdateTime = DateTime.Now;
        }

        /// <summary>
        /// Adds a new <see cref="IUpdatable"/> to be updated.
        /// </summary>
        /// <param name="updatable">The object to be updated.</param>
        public void AddUpdatable(IUpdatable updatable)
        {
            updatables.Add(new WeakReference<IUpdatable>(updatable));
        }

        /// <summary>
        /// Adds a new <see cref="ILateUpdatable"/> to be updated.
        /// </summary>
        /// <param name="lateUpdatable">The object to be updated.</param>
        public void AddLateUpdatable(ILateUpdatable lateUpdatable)
        {
            lateUpdatables.Add(new WeakReference<ILateUpdatable>(lateUpdatable));
        }

        /// <summary>
        /// Updates this object. 
        /// All the <see cref="IUpdatable"/> and <see cref="ILateUpdatable"/> added to this object will be updated as well.
        /// </summary>
        public void Update()
        {
            TimeSpan timeSinceLastUpdate = DateTime.Now - lastUpdateTime;
            lastUpdateTime = DateTime.Now;

            for (int i = 0; i < updatables.Count; i++)
            {
                IUpdatable updatable = updatables[i].Target;
                if (updatable != null)
                {
                    updatable.Update(timeSinceLastUpdate);
                }
                else
                {
                    updatables.RemoveAt(i--);
                }
            }

            TimeSpan timeSinceLastLateUpdate = DateTime.Now - lastLateUpdateTime;
            lastLateUpdateTime = DateTime.Now;

            for (int i = 0; i < lateUpdatables.Count; i++)
            {
                ILateUpdatable lateUpdatable = lateUpdatables[i].Target;
                if (lateUpdatable != null)
                {
                    lateUpdatable.LateUpdate(timeSinceLastLateUpdate);
                }
                else
                {
                    lateUpdatables.RemoveAt(i--);
                }
            }

        }

    }
}
