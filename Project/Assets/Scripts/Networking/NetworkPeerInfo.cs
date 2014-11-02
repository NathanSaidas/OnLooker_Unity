using UnityEngine;
using System;
using System.Collections;
using System.Runtime.Serialization;
using EndevGame;

namespace Gem
{

    [Serializable]
    public class NetworkPeerInfo : NetworkPacket
    {
        public class PeerSavedInfo : FileContent
        {
            
            public PeerSavedInfo() : base()
            {

            }
            public PeerSavedInfo(SerializationInfo aInfo, StreamingContext aContext) : base(aInfo, aContext)
            {

            }
            public override void GetObjectData(SerializationInfo aInfo, StreamingContext aContext)
            {
                base.GetObjectData(aInfo, aContext);   
            }
            protected override void OnLoad()
            {
                
            }
            protected override void OnSave()
            {
                
            }

            public string username
            {
                get;
                set;
            }
            public string password
            {
                get;
                set;
            }

            public NetworkPeerInfo toPeerInfo
            {
                get
                {
                    NetworkPeerInfo info = new NetworkPeerInfo();
                    info.username = username;
                    info.password = password;
                    return info;
                }
            }
        }

        private string m_Username = string.Empty;
        private string m_Password = string.Empty;
        private int m_UserID = -1;
        private string m_NetworkKey = string.Empty;


        public void AssignKey()
        {
            m_NetworkKey = NetworkProperties.NETWORK_KEY;
        }
        public string username
        {
            get { return m_Username; }
            set { m_Username = value; }
        }
        public string password
        {
            get { return m_Password; }
            set { m_Password = value; }
        }
        public int userID
        {
            get { return m_UserID; }
            set { m_UserID = value; }
        }
        public string networkKey
        {
            get { return m_NetworkKey; }
        }
        public override bool serializeSafe
        {
            get { return true; }
        }

        public PeerSavedInfo toSavedInfo
        {
            get
            {
                PeerSavedInfo info = new PeerSavedInfo();
                info.username = username;
                info.password = password;
                info.contentName = username;
                return info;
            }
        }
        

        
    }
}