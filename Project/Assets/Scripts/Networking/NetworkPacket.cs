using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

namespace Gem
{
    /// <summary>
    /// This is a base class for all packets. It provides functionality to serialize to bytes / deserialize from bytes for networking.
    /// </summary>
    [Serializable]
    public abstract class NetworkPacket
    {
        public NetworkPacket()
        {

        }
        /// <summary>
        /// Serializes the data to bytes.
        /// </summary>
        /// <param name="aPacket">The data to serialize</param>
        /// <returns></returns>
        public static byte[] Serialize(NetworkPacket aPacket)
        {
            if(aPacket == null || aPacket.serializeSafe == false)
            {
                return null;
            }
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, aPacket);
            return stream.ToArray();
        }
        /// <summary>
        /// Deserializes the data to an object.
        /// </summary>
        /// <typeparam name="T">The type to deserilzie to</typeparam>
        /// <param name="aBytes">The data being deserialized</param>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] aBytes) where T : NetworkPacket
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(aBytes);
            return (T)formatter.Deserialize(stream);
        }

        /// <summary>
        /// Implement this to determine if the object is ready for serialization
        /// </summary>
        public abstract bool serializeSafe { get; }
        
    }
}