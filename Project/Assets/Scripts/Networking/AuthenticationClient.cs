using UnityEngine;
using System.Collections.Generic;

#region CHANGE LOG

#endregion
namespace Gem
{
    
    [RequireComponent(typeof(NetworkView))]
    public class AuthenticationClient : MonoBehaviour
    {
        public delegate void StatusCallback(NetworkPeerInfo aPeer, RequestStatus aStatus);
        
        #region SINGLETON
        /// <summary>
        /// A singleton instance of AuthenticationServer
        /// </summary>
        private static AuthenticationClient s_Instance = null;
        /// <summary>
        /// An accessor which creates an instance of AuthenticationServer if it is null
        /// </summary>
        public static AuthenticationClient instance
        {
            get { if (s_Instance == null) { CreateInstance(); } return s_Instance; }
        }
        /// <summary>
        /// Creates an instance of the AuthenticationServer if it was missing.
        /// </summary>
        private static void CreateInstance()
        {
            GameObject persistant = GameObject.Find(Game.PERSISTANT_GAME_OBJECT_NAME);
            if (persistant == null)
            {
                persistant = new GameObject(Game.PERSISTANT_GAME_OBJECT_NAME);
                persistant.transform.position = Vector3.zero;
                persistant.transform.rotation = Quaternion.identity;
            }
            s_Instance = persistant.GetComponent<AuthenticationClient>();
            if (s_Instance == null)
            {
                s_Instance = persistant.AddComponent<AuthenticationClient>();
            }

        }
        /// <summary>
        /// Sets the instance of the AuthenticationServer to the instance given.
        /// </summary>
        /// <param name="aInstance">The instance to make singleton</param>
        /// <returns></returns>
        private static bool SetInstance(AuthenticationClient aInstance)
        {
            if (s_Instance != null && s_Instance != aInstance)
            {
                return false;
            }
            s_Instance = aInstance;
            return true;
        }
        /// <summary>
        /// Removes the instance of the AuthenticationServer if the instance being destroyed is the the same as the singleton.
        /// </summary>
        /// <param name="aInstance"></param>
        private static void DestroyInstance(AuthenticationClient aInstance)
        {
            if (s_Instance == aInstance)
            {
                s_Instance = null;
            }
        }
        #endregion


        private Queue<NetworkPeerInfo> m_AuthenticationRequests = new Queue<NetworkPeerInfo>();
        private Queue<NetworkPeerInfo> m_RegisterRequests = new Queue<NetworkPeerInfo>();
        private Queue<NetworkPeerInfo> m_UnregisterRequests = new Queue<NetworkPeerInfo>();
        private bool m_AuthenticationPending = false;
        private bool m_RegisterPending = false;
        private bool m_UnregisterPending = false;


        private StatusCallback m_AuthenticationCallback = null;
        private StatusCallback m_RegisterCallback = null;
        private StatusCallback m_UnregisterCallback = null;

        /// <summary>
        /// Updates the request queues
        /// </summary>
        void Update()
        {
            if(m_AuthenticationRequests.Count > 0 && m_AuthenticationPending == false)
            {
                m_AuthenticationPending = true;
                instance.networkView.RPC(NetworkRPC.AUTHS_AUTHENTICATE_REQUEST, RPCMode.Server, NetworkPacket.Serialize(m_AuthenticationRequests.Peek()));
            }
            if (m_RegisterRequests.Count > 0 && m_RegisterPending == false)
            {
                m_RegisterPending = true;
                instance.networkView.RPC(NetworkRPC.AUTHS_REGISTER_REQUEST, RPCMode.Server, NetworkPacket.Serialize(m_AuthenticationRequests.Peek()));
            }
            if (m_UnregisterRequests.Count > 0 && m_UnregisterPending == false)
            {
                m_UnregisterPending = true;
                instance.networkView.RPC(NetworkRPC.AUTHS_UNREGISTER_REQUEST, RPCMode.Server, NetworkPacket.Serialize(m_AuthenticationRequests.Peek()));
            }
        }
        #region SENDERS
        /// <summary>
        /// Adds a request to the queue
        /// </summary>
        /// <param name="aInfo"></param>
        public static void SendAuthenticationRequest(NetworkPeerInfo aInfo)
        {
            instance.m_AuthenticationRequests.Enqueue(aInfo);
        }
        /// <summary>
        /// Adds a request to the queue
        /// </summary>
        /// <param name="aInfo"></param>
        public static void SendRegisterRequest(NetworkPeerInfo aInfo)
        {
            instance.m_RegisterRequests.Enqueue(aInfo);
        }
        /// <summary>
        /// Adds a request to the queue
        /// </summary>
        /// <param name="aInfo"></param>
        public static void SendUnregisterRequest(NetworkPeerInfo aInfo)
        {
            instance.m_UnregisterRequests.Enqueue(aInfo);
        }
        #endregion
        #region RECEIVERS
        /// <summary>
        /// Receives the request from the server and parses the status into a request status
        /// </summary>
        /// <param name="aStatus"></param>
        public static void ReceiveAuthenticationRequest(int aStatus)
        {
            if(aStatus == 0)
            {
                instance.HandleAuthenticationRequest(RequestStatus.BAD);
            }
            else if(aStatus == 1)
            {
                instance.HandleAuthenticationRequest(RequestStatus.GOOD);
            }
            else
            {
                //ERROR
                instance.m_AuthenticationRequests.Dequeue();
            }
            instance.m_AuthenticationPending = false;
        }
        /// <summary>
        /// Receives the request from the server and parses the status into a request status
        /// </summary>
        /// <param name="aStatus"></param>
        public static void ReceiveRegisterRequest(int aStatus)
        {
            if (aStatus == 0)
            {
                instance.HandleRegisterRequest(RequestStatus.BAD);
            }
            else if (aStatus == 1)
            {
                instance.HandleRegisterRequest(RequestStatus.GOOD);
            }
            else
            {
                //ERROR
                instance.m_RegisterRequests.Dequeue();
            }
            instance.m_RegisterPending = false;
        }
        /// <summary>
        /// Receives the request from the server and parses the status into a request status
        /// </summary>
        /// <param name="aStatus"></param>
        public static void ReceiveUnregisterRequest(int aStatus)
        {
            if (aStatus == 0)
            {
                instance.HandleUnregisterRequest(RequestStatus.BAD);
            }
            else if (aStatus == 1)
            {
                instance.HandleUnregisterRequest(RequestStatus.GOOD);
            }
            else
            {
                //ERROR
                instance.m_UnregisterRequests.Dequeue();
            }
            instance.m_UnregisterPending = false;
        }
        #endregion

        /// <summary>
        /// Invokes the callback to the authentication callback letting them know of the event.
        /// </summary>
        /// <param name="aStatus"></param>
        private void HandleAuthenticationRequest(RequestStatus aStatus)
        {
            NetworkPeerInfo peer = m_AuthenticationRequests.Dequeue();
            if(m_AuthenticationCallback != null)
            {
                m_AuthenticationCallback.Invoke(peer, aStatus);
            }
        }
        /// <summary>
        /// Invokes the callback to the register callback letting them know of the event.
        /// </summary>
        /// <param name="aStatus"></param>
        private void HandleRegisterRequest(RequestStatus aStatus)
        {
            NetworkPeerInfo peer = m_RegisterRequests.Dequeue();
            if (m_RegisterCallback != null)
            {
                m_RegisterCallback.Invoke(peer, aStatus);
            }
        }
        /// <summary>
        /// Invokes the callback to the unregister callback letting them know of the event.
        /// </summary>
        /// <param name="aStatus"></param>
        private void HandleUnregisterRequest(RequestStatus aStatus)
        {
            NetworkPeerInfo peer = m_UnregisterRequests.Dequeue();
            if (m_UnregisterCallback != null)
            {
                m_UnregisterCallback.Invoke(peer, aStatus);
            }
        }

        /// Callback Accessors
        public StatusCallback authenticationCallback
        {
            get { return m_AuthenticationCallback; }
            set { m_AuthenticationCallback = value; }
        }
        public StatusCallback registerCallback
        {
            get { return m_RegisterCallback; }
            set { m_RegisterCallback = value; }
        }
        public StatusCallback unregisterCallback
        {
            get { return m_UnregisterCallback; }
            set { m_UnregisterCallback = value; }
        }

    }
}