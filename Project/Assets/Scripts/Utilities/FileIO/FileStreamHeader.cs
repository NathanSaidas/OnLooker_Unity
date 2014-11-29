using System;
using System.Collections.Generic;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace EndevGame
{
    [Serializable]
    public class FileStreamHeader : ISerializable
    {
        private string m_Filename = string.Empty;
        private string m_Extension = string.Empty;
        private int m_FileCount = 0;

        public FileStreamHeader()
        {

        }
        public FileStreamHeader(SerializationInfo aInfo, StreamingContext aContext)
        {
            m_Filename = (string)aInfo.GetValue("Filename", typeof(string));
            m_Extension = (string)aInfo.GetValue("Extension", typeof(string));
            m_FileCount = (int)aInfo.GetValue("FileCount", typeof(int));
        }

        public void GetObjectData(SerializationInfo aInfo, StreamingContext aContext)
        {
            aInfo.AddValue("Filename", m_Filename);
            aInfo.AddValue("Extension", m_Extension);
            aInfo.AddValue("FileCount", m_FileCount);
        }

        public string filename
        {
            get { return m_Filename; }
            set { m_Filename = value; }
        }
        public string extension
        {
            get { return m_Extension; }
            set { m_Extension = value; }
        }
        public int fileCount
        {
            get { return m_FileCount; }
            set { m_FileCount = value; }
        }
    }
}