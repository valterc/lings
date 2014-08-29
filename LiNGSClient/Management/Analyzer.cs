using LiNGS.Client.Network;
using LiNGS.Common.GameCycle;
using LiNGS.Common.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LiNGS.Client.Management
{
    internal class Analyzer : IUpdatable
    {
        private LiNGSClient client;

        public Analyzer(LiNGSClient client)
        {
            this.client = client;
        }

        #region IUpdatable Members

        public void Update(TimeSpan timeSinceLastUpdate)
        {
            if (client.ClientStatus.Connected && client.Manager.Server != null)
            {
                InternalNetworkClient serverNetworkClient = client.Manager.Server as InternalNetworkClient;

                TimeSpan time = DateTime.Now - serverNetworkClient.LastReceivedConnectionTime;
                if (!serverNetworkClient.HeartbeatSent && time.TotalMilliseconds > client.ClientProperties.MaxServerBlackoutTime)
                {
                    serverNetworkClient.HeartbeatSent = true;
                    NetworkMessage message = new NetworkMessage(NetworkMessage.MessageType.Heartbeat);
                    message.NeedsAck = true;
                    client.Manager.SendMessage(message);
                }

            }
        }

        #endregion
    }
}
