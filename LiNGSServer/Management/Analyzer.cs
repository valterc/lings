using LiNGS.Common.GameCycle;
using LiNGS.Common.Network;
using LiNGS.Server.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LiNGS.Server.Management
{
    internal class Analyzer : IUpdatable
    {
        private LiNGSServer server;
        private Manager manager;

        public Analyzer(LiNGSServer server)
        {
            this.server = server;
            this.manager = server.Manager;
        }

        #region IUpdatable Members

        public void Update(TimeSpan timeSinceLastUpdate)
        {
            List<GameClient> clients = manager.ConnectedClients.ToList();

            foreach (var item in clients)
            {
                InternalNetworkClient client = item.NetworkClient as InternalNetworkClient;
                TimeSpan time = DateTime.Now - client.LastReceivedConnectionTime;
                if (!client.HeartbeatSent && time.TotalMilliseconds > server.ServerProperties.MaxClientBlackoutTime)
                {
                    //Sent a heartbeat to the client and set a flag indicating that a network heartbeat was sent
                    client.HeartbeatSent = true;
                    NetworkMessage message = new NetworkMessage(NetworkMessage.MessageType.Heartbeat);
                    message.NeedsAck = true;
                    server.Manager.SendMessage(client, message);
                }
            }

        }

        #endregion

    }
}
