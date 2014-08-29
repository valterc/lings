using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Common
{
    /// <summary>
    /// String markers used internally on the LiNGS System.
    /// </summary>
    public static class LiNGSMarkers
    {
        /// <summary>
        /// The namespace used to identify the message.
        /// </summary>
        public static readonly string Namespace = "0L";

        /// <summary>
        /// Separator used when multiple data is joined together.
        /// </summary>
        public static readonly string Separator = "$";

        /// <summary>
        /// Indicates that the data is an Id.
        /// </summary>
        public static readonly string Id = Namespace + ":id";

        /// <summary>
        /// Indicates that the data is an SessionUserId.
        /// </summary>
        public static readonly string SessionUserId = Namespace + ":uid";

        /// <summary>
        /// Indicates that the data is a time offset.
        /// </summary>
        public static readonly string TimeOffset = Namespace + ":toffset";

        /// <summary>
        /// Indicates that an error has occurred.
        /// </summary>
        public static readonly string Error = Namespace + ":err";

        /// <summary>
        /// Indicates that the request was successful.
        /// </summary>
        public static readonly string Ok = Namespace + ":ok";

        /// <summary>
        /// Indicates that an object should be created.
        /// </summary>
        public static readonly string CreateObject = Namespace + ":ctobj";

        /// <summary>
        /// Indicates the type of an object.
        /// </summary>
        public static readonly string ObjectType = Namespace + ":otype";

        /// <summary>
        /// Indicates if the object was automatically created.
        /// </summary>
        public static readonly string AutoCreatedObject = Namespace + ":o:";

        /// <summary>
        /// Indicates that an object should be destroyed.
        /// </summary>
        public static readonly string DestroyObject = Namespace + ":dtobj";

        /// <summary>
        /// Indicates that a saved state will be used.
        /// </summary>
        public static readonly string UsingSavedState = Namespace + ":scached";

        /// <summary>
        /// Indicates a reason for the response.
        /// </summary>
        public static readonly string Reason = Namespace + ":reason";

        /// <summary>
        /// Indicates that an object should be activated/deactivated.
        /// </summary>
        public static readonly string SetActive = Namespace + ":act";

    }
}
