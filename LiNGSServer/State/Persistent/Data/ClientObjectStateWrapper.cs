using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Server.State.Persistent.Data
{
    /// <summary>
    /// Used to wrap a client object state.
    /// </summary>
    public class ClientObjectStateWrapper
    {
        /// <summary>
        /// Name of the object.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Type of the object.
        /// </summary>
        public String TypeName { get; set; }

        /// <summary>
        /// Indicates if the client knowns about the object.
        /// </summary>
        public bool Known { get; set; }

        /// <summary>
        /// Indicates if the object information was sent to the client.
        /// </summary>
        public bool Sent { get; set; }

        /// <summary>
        /// Indicates if this object was automatically created.
        /// </summary>
        public bool AutoCreateObject { get; set; }

        /// <summary>
        /// The list of <see cref="FieldStateWrapper"/> of the object.
        /// </summary>
        public List<FieldStateWrapper> Fields { get; set; }

        /// <summary>
        /// Creates a new empty instance of this class.
        /// </summary>
        public ClientObjectStateWrapper()
        {
            Fields = new List<FieldStateWrapper>();
        }

    }
}
