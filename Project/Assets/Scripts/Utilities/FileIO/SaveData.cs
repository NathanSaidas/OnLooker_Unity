using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace OnLooker
{
        /// <summary>
        /// This is the base class for all save data for OnLooker.FileIO
        /// </summary>
    [Serializable]
    abstract public class SaveData : NativeObject
    {
        public SaveData()
        {
            name = string.Empty;
        }
        public SaveData(string aName)
        {
            name = aName;
        }
        abstract public void save(Stream aStream, BinaryFormatter aFormatter);
    }
}
