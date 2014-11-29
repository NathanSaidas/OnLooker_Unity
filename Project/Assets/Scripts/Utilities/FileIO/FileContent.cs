using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

namespace EndevGame
{
    
    [Serializable]
    public class FileContent : ISerializable
    {
        /// <summary>
        /// The name of the content within the File structure
        /// </summary>
        private string m_ContentName = string.Empty;
        /// <summary>
        /// A structure set and get data from, during saving / loading
        /// </summary>
        private SerializationInfo m_Info = null;

        public FileContent()
        {

        }
        public FileContent(string aContentName)
        {
            m_ContentName = aContentName;
        }
        /// <summary>
        /// A constructor used by .Net Serialization to load data
        /// </summary>
        /// <param name="aInfo"></param>
        /// <param name="aContext"></param>
        public FileContent(SerializationInfo aInfo, StreamingContext aContext)
        {
            m_Info = aInfo;
            m_ContentName = (string)m_Info.GetValue("ContentName", typeof(string));
            OnLoad();
            m_Info = null;
        }
        /// <summary>
        /// A method used by ISerializable to save data with.
        /// </summary>
        /// <param name="aInfo"></param>
        /// <param name="aContext"></param>
        public virtual void GetObjectData(SerializationInfo aInfo, StreamingContext aContext)
        {
            m_Info = aInfo;
            m_Info.AddValue("ContentName", m_ContentName);
            OnSave();
            m_Info = null;
        }


        /// <summary>
        /// Gets invoked when a File is loading data.
        /// </summary>
        protected virtual void OnLoad()
        {

        }
        /// <summary>
        /// Gets invoked when a File is saving data.
        /// </summary>
        protected virtual void OnSave()
        {

        }
        /// <summary>
        /// Adds data into the serialzation stream. Use during saving.
        /// </summary>
        /// <param name="aName">The name of the data within the stream</param>
        /// <param name="aValue">The value of the data within the stream</param>
        protected void AddData(string aName, object aValue)
        {
            if(m_Info != null)
            {
                m_Info.AddValue(aName, aValue);
            }
        }
        /// <summary>
        /// Retrieves data from the serialzation stream. Use during loading
        /// </summary>
        /// <param name="aName">The name of the data to get</param>
        /// <param name="aType">The type of data to constrain to</param>
        /// <returns></returns>
        protected object GetData(string aName, Type aType)
        {
            if(m_Info != null)
            {
                return m_Info.GetValue(aName, aType);
            }
            return default(object);
        }
        /// <summary>
        /// Retrieves data from the serialzation stream. Use during loading
        /// </summary>
        /// <typeparam name="T">The type of data to constrain to</typeparam>
        /// <param name="aName">The name of the data to get</param>
        /// <returns></returns>
        protected T GetData<T>(string aName)
        {
            if(m_Info != null)
            {
                return (T)m_Info.GetValue(aName, typeof(T));
            }
            return default(T);
        }

        /// <summary>
        /// The name of content.
        /// </summary>
        public string contentName
        {
            get { return m_ContentName; }
            set { m_ContentName = value; }
        }
    }
}
