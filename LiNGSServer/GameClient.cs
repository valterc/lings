using LiNGS.Common;
using LiNGS.Common.Network;
using LiNGS.Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LiNGS.Server
{
    /// <summary>
    /// Represent a client.
    /// </summary>
    public class GameClient
    {
        private static Guid sessionGuid;
        private static int userId;
        internal static Guid SessionGUID
        {
            get
            {
                if (sessionGuid == Guid.Empty)
                {
                    sessionGuid = Guid.NewGuid();
                }

                return sessionGuid;
            }
        }

        /// <summary>
        /// The network information of the client.
        /// </summary>
        public NetworkClient NetworkClient { get; internal set; }

        /// <summary>
        /// The client identification within this session of the game.
        /// </summary>
        public String SessionUserId { get; internal set; }

        /// <summary>
        /// The client identification.
        /// </summary>
        public String UserId { get; internal set; }

        /// <summary>
        /// The session identification.
        /// </summary>
        public String SessionId
        {
            get
            {
                return sessionGuid.ToString();
            }
        }

        /// <summary>
        /// The time that this client established a connection.
        /// </summary>
        public DateTime ConnectedAt { get; internal set; }

        /// <summary>
        /// Creates a new instance of this class with the current session identification and generates a new unique session user identification.
        /// </summary>
        public GameClient()
        {
            if (sessionGuid == Guid.Empty)
            {
                sessionGuid = Guid.NewGuid();
            }

            this.UserId = Interlocked.Increment(ref userId).ToString();
            this.SessionUserId = SessionId.ToString() + LiNGSMarkers.Separator + this.UserId;
            this.ConnectedAt = DateTime.Now;
        }

        /// <summary>
        /// Creates a new instance of this class with the information of a user session identification.
        /// </summary>
        /// <param name="sessionString">The identification of the user and session.</param>
        public GameClient(String sessionString)
        {
            this.UserId = sessionString.Split(LiNGSMarkers.Separator[0])[1];
            this.SessionUserId = sessionString;
            this.ConnectedAt = DateTime.Now;
        }

    }
}
