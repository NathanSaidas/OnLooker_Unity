using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace OnLooker
{
        [Serializable]
        public abstract class CustomSaveData : SaveData, ISerializable
        {

            SerializationInfo m_Info = null;

            public override void save(Stream aStream, BinaryFormatter aFormatter)
            {
                if (aStream != null && aFormatter != null)
                {
                    aFormatter.Serialize(aStream, this);
                }
            }
            
            public CustomSaveData()
            {

            }
            /// <summary>
            /// Implement this constructor and call this one. Otherwise a serialization exception will be thrown.
            /// </summary>
            /// <param name="aInfo"></param>
            /// <param name="aContext"></param>
            public CustomSaveData(SerializationInfo aInfo, StreamingContext aContext)
            {
                m_Info = aInfo;
                name = (string)aInfo.GetValue("Name", typeof(string));
                onLoad();
            }

            
            public void GetObjectData(SerializationInfo aInfo, StreamingContext aContext)
            {
                m_Info = aInfo;
                aInfo.AddValue("Name", name);
                onSave();
            }

            
            /// <summary>
            /// On save gets invoked when this piece of data is being saved to the file
            /// </summary>
            protected virtual void onSave()
            {
                
            }
            /// <summary>
            /// On Load gets invoked when this piece of data is being loaded from the file.
            /// </summary>
            protected virtual void onLoad()
            {
                
            }

            //Helper Function to add data in by name and value
            protected void addData(string aName, object aValue)
            {
                //UnityEngine.Debug.Log(aName);
                if(aName != "Name" && aValue != null && m_Info != null)
                {
                    m_Info.AddValue(aName, aValue);
                }
            }
            //Helper Function to get data out by name and value
            protected T getData<T>(string aName)
            {
                //UnityEngine.Debug.Log(UnityEngine.StackTraceUtility.ExtractStackTrace());
                Type type = typeof(T);
                if(m_Info != null)
                {
                    return (T)m_Info.GetValue(aName,type);
                }
                return default(T);
            }

            protected object getData(string aName, Type aType)
            {
                if(m_Info != null)
                {
                    return m_Info.GetValue(aName, aType);
                }
                return null;
            }
        }
}
