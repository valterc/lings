using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace LiNGS.Common.Network
{
    /// <summary>
    /// Represents a message to be transmitted between two systems via a network.
    /// </summary>
    public class NetworkMessage
    {
        /// <summary>
        /// Type of the <see cref="NetworkMessage"/>.
        /// </summary>
        public enum MessageType
        {
            /// <summary>
            /// Used to establish a connection.
            /// </summary>
            Connect, 

            /// <summary>
            /// Used to terminate a connection.
            /// </summary>
            Disconnect, 

            /// <summary>
            /// Knowledges the reception of a message.
            /// </summary>
            Ack, 

            /// <summary>
            /// Message with game data.
            /// </summary>
            Data,

            /// <summary>
            /// Message sent by the game implementation.
            /// </summary>
            Game,

            /// <summary>
            /// Message sent by the game implementation.
            /// </summary>
            Event, 

            /// <summary>
            /// Message sent by the game implementation.
            /// </summary>
            Error, 

            /// <summary>
            /// Indicates that an error ocurred when establishing a connection.
            /// </summary>
            ErrorConnect, 

            /// <summary>
            /// Check if a client is still online.
            /// </summary>
            Heartbeat
        }

        /// <summary>
        /// The type of the message.
        /// </summary>
        public MessageType Type { get; set; }

        /// <summary>
        /// The contents of the message.
        /// </summary>
        public List<MessageData> Data { get; private set; }

        /// <summary>
        /// The endpoint of the sender of the message.
        /// </summary>
        public EndPoint From { get; set; }

        /// <summary>
        /// Size of the message.
        /// </summary>
        public int Length
        {
            get
            {
                return 13 + Data.Sum(md => md.Length + 4);
            }
        }

        /// <summary>
        /// Indicates if the message needs to be Acknowledged.
        /// </summary>
        public bool NeedsAck { get; set; }

        /// <summary>
        /// The id of the message.
        /// </summary>
        public int MessageId { get; set; }

        /// <summary>
        /// The method to invoke when the message reception is confirmed.
        /// </summary>
        public Action<NetworkMessage> OnReceived { get; set; }

        /// <summary>
        /// Creates a empty instance of this class.
        /// </summary>
        public NetworkMessage()
        {
            Data = new List<MessageData>();
        }

        /// <summary>
        /// Creates a instance of this class with the specified type and contents.
        /// </summary>
        /// <param name="type">The type of the message.</param>
        /// <param name="data">The contents of the message.</param>
        public NetworkMessage(MessageType type, params MessageData[] data)
        {
            this.Data = new List<MessageData>(data);
            this.Type = type;
        }

        /// <summary>
        /// Deserializes a message
        /// </summary>
        /// <param name="serializedMessage">Serialized message</param>
        public NetworkMessage(byte[] serializedMessage)
        {
            int index = 0;
            int messageLength = 0;

            this.Data = new List<MessageData>();
            this.Type = (MessageType)BitConverter.ToInt32(serializedMessage, index);
            index += 4;

            this.MessageId = BitConverter.ToInt32(serializedMessage, index);
            index += 4;

            this.NeedsAck = serializedMessage[index] == (byte)1;
            index += 1;

            messageLength = BitConverter.ToInt32(serializedMessage, index);
            index += 4;

            while (index < messageLength + 8)
            {
                int dataLength = BitConverter.ToInt32(serializedMessage, index);
                index += 4;

                Data.Add(new MessageData(serializedMessage, index, dataLength));
                index += dataLength;
            }

        }

        /// <summary>
        /// Deserializes a message
        /// </summary>
        /// <param name="serializedMessage">Serialized message</param>
        /// <param name="senderEndPoint">Message origin</param>
        public NetworkMessage(byte[] serializedMessage, EndPoint senderEndPoint) : this(serializedMessage)
        {
            this.From = senderEndPoint;
        }

        /// <summary>
        /// Serializes the message into a network friendly data format
        /// MESSAGE => TYPE|ID|NEEDS_ACK|DATA_LENGTH|{[DATA_LENGTH|DATA] 0..N}
        /// </summary>
        /// <returns>Serialized message</returns>
        public byte[] Serialize()
        {
            List<byte> messageBytes = new List<byte>();

            messageBytes.AddRange(BitConverter.GetBytes((int)Type));
            messageBytes.AddRange(BitConverter.GetBytes(MessageId));
            messageBytes.Add(NeedsAck ? (byte)1 : (byte)0);

            if (Data.Count > 0)
            {
                int size = 0;

                foreach (var item in Data)
                {
                    byte[] data = item.Serialize();
                    if (data == null)
                    {
                        continue;
                    }
                    size += data.Length + 4;
                    messageBytes.AddRange(BitConverter.GetBytes(data.Length));
                    messageBytes.AddRange(data);
                }

                messageBytes.InsertRange(9, BitConverter.GetBytes(size));
            }
            else
            {
                messageBytes.AddRange(BitConverter.GetBytes(0));
            }

            return messageBytes.ToArray();
        }

        /// <summary>
        /// Converts the message to a <see cref="String"/> representation.
        /// </summary>
        /// <returns>String representation of the object.</returns>
        public override string ToString()
        {
            return String.Format("NetworkMessage: Type=\"{0}\", From=\"{1}\", Id={2}, NeedsAck={3}, Length={4}, Data Count={5}", Type, From, MessageId, NeedsAck, Length, Data.Count);
        }

        /// <summary>
        /// Confirms the reception of this message.
        /// </summary>
        public void ConfirmReception()
        {
            if (OnReceived != null)
            {
                OnReceived(this);
            }
        }

    }
}
