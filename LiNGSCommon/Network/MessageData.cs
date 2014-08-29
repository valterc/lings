using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Common.Network
{
    /// <summary>
    /// Represent a piece of information transmitted in a <see cref="NetworkMessage"/>.
    /// This information is composed of 3 strings: Object, Property, Value
    /// </summary>
    public class MessageData
    {
        /// <summary>
        /// The size of the message (in bytes)
        /// </summary>
        public int Length
        {
            get
            {
                return (Object == null ? 0 : Object.Length) + 
                    (Property == null ? 0 : Property.Length) + 
                    (Value == null ? 0 : Value.Length) + 2;
            }
        }

        /// <summary>
        /// The object name
        /// </summary>
        public String Object { get; set; }

        /// <summary>
        /// The property name
        /// </summary>
        public String Property { get; set; }

        /// <summary>
        /// The property value
        /// </summary>
        public String Value { get; set; }

        /// <summary>
        /// Creates a new empty instance of this class.
        /// </summary>
        public MessageData()
        {

        }

        /// <summary>
        /// Creates a new instance of this class with the deserialized data.
        /// </summary>
        /// <param name="serializedMessageData">Serialized data.</param>
        /// <param name="index">The index of the data.</param>
        /// <param name="count">The size of the data.</param>
        public MessageData(byte[] serializedMessageData, int index, int count)
        {
            String s = Encoding.UTF8.GetString(serializedMessageData, index, count);
            String[] parts = s.Split('\0');

            this.Object = parts[0];
            this.Property = parts[1];
            this.Value = parts[2];
        }

        /// <summary>
        /// Serializes the data.
        /// </summary>
        /// <returns>Serialized data.</returns>
        public byte[] Serialize()
        {
            try
            {
                StringBuilder s = new StringBuilder(Object.Length + (Property == null ? 0 : Property.Length) + (Value == null ? 0 : Value.Length) + 2);
                s.Append(Object);
                s.Append('\0');
                s.Append(Property ?? "");
                s.Append('\0');
                s.Append(Value ?? "");

                return Encoding.UTF8.GetBytes(s.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Represents this object in a <see cref="String"/>.
        /// </summary>
        /// <returns>String representation of the object.</returns>
        public override string ToString()
        {
            return String.Format("MessageData: Object=\"{0}\", Property=\"{1}\", Value=\"{2}\"", Object, Property, Value);
        }

    }
}
