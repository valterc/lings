using LiNGS.Common.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Client.Management
{
    internal class ImportantMessageWrapper
    {
        internal NetworkClient Client { get; set; }
        internal NetworkMessage Message { get; set; }
        internal DateTime SentDate { get; set; }
        internal bool ConfirmationReceived { get; set; }
        internal int Retries { get; set; }

        public ImportantMessageWrapper()
        {

        }

    }
}
