using LiNGS.Common.GameCycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Client.Management
{
    internal class ConnectionEstablisherHelper : IUpdatable
    {
        private Object Lock = new Object();
        private LiNGSClient client;
        private DateTime ConnectionBeginDate;

        internal  bool Connecting { get; private set; }

        public ConnectionEstablisherHelper(LiNGSClient client)
        {
            this.client = client;
        }

        public void Connect()
        {
            Connecting = true;
            this.ConnectionBeginDate = DateTime.Now;
        }

        public void ConnectEnd()
        {
            lock (Lock)
            {
                Connecting = false;
            }
        }

        #region IUpdatable Members

        public void Update(TimeSpan timeSinceLastUpdate)
        {
            lock (Lock)
            {
                if (!Connecting)
                {
                    return;
                }

                if (DateTime.Now - this.ConnectionBeginDate > TimeSpan.FromMilliseconds(client.ClientProperties.ConnectionEstablishingTimeout))
                {
                    client.Manager.UnableToConnect("Connection timed out");
                }
            }
        }

        #endregion

    }
}
