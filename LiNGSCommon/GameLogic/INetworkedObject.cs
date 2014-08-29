using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Common.GameLogic
{ 
    /// <summary>
    /// Represent an object that can be synchronized with the LiNGS System.
    /// <remarks>
    /// The synchronization system can stop syncing this object if the client doesn't need the object information at that time. 
    /// In order for the object to known if it was deactivated/activated the object must implement a method with the following signature:
    /// <code>
    /// public void LiNGSSetActive(bool active){ }
    /// </code>
    /// This method will be invoked to activate/deactivate the object.
    /// </remarks>
    /// 
    /// </summary>
    public interface INetworkedObject
    {
    }
}
