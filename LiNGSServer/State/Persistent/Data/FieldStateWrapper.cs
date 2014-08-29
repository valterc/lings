using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Server.State.Persistent.Data
{
    /// <summary>
    /// Used to wrap a object field from a client state.
    /// </summary>
    public class FieldStateWrapper
    {
        /// <summary>
        /// The name of the field.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The value of the field.
        /// </summary>
        public String Value { get; set; }

        /// <summary>
        /// Indicates if the field is known by the client.
        /// </summary>
        public bool Known { get; set; }

        /// <summary>
        /// Indicates if the value of the field was sent to the client.
        /// </summary>
        public bool Sent { get; set; }
    }
}
