using UnityEngine;
using System.Collections;

namespace Gem
{
    [RequireComponent(typeof(NetworkView))]
    public class NetworkRPC : MonoBehaviour
    {
        #region RPC NAME CONSTANTS
        public const string AUTHC_SERVER_CLOSE = "AuthC_ServerClose";
        public const string AUTHS_REGISTER_REQUEST = "AuthS_RegisterRequest";
        public const string AUTHC_REGISTER_REQUEST = "AuthC_RegisterRequest";
        public const string AUTHS_UNREGISTER_REQUEST = "AuthS_UnregisterRequest";
        public const string AUTHC_UNREGISTER_REQUEST = "AuthC_UnregisterRequest";
        public const string AUTHS_AUTHENTICATE_REQUEST = "AuthS_AuthenticateRequest";
        public const string AUTHC_AUTHENTICATE_REQUEST = "AuthC_AuthenticateRequest";
        #endregion


        #region RPC Calls
        #region AUTHENTICATION SERVER
        /// <summary>
        /// An RPC call to Clients from the Authentication server to let them know the server is closing
        /// </summary>
        [RPC]
        private void AuthC_ServerClose()
        {

        }
        /// <summary>
        /// An RPC call to the server to request registering the player to the server.
        /// </summary>
        /// <param name="aNetworkPeerInfo"></param>
        [RPC]
        private void AuthS_RegisterRequest(byte[] aNetworkPeerInfo, NetworkMessageInfo aInfo)
        {
            NetworkPeerInfo info = NetworkPacket.Deserialize<NetworkPeerInfo>(aNetworkPeerInfo);
            if(info == null)
            {
                networkView.RPC(AUTHC_REGISTER_REQUEST,aInfo.sender,0);
                return;
            }
            if(AuthenticationServer.instance.Register(info))
            {
                networkView.RPC(AUTHC_REGISTER_REQUEST, aInfo.sender, 1);
            }
            else
            {
                networkView.RPC(AUTHC_REGISTER_REQUEST, aInfo.sender, 0);
            }
        }
        /// <summary>
        /// An RPC call to the client from the server to let the client know of their register request.
        /// </summary>
        /// <param name="aSuccess">0 == false, 1 == true</param>
        [RPC]
        private void AuthC_RegisterRequest(int aSuccess)
        {
            AuthenticationClient.ReceiveRegisterRequest(aSuccess);
        }
        /// <summary>
        /// An RPC call to the server to request unregistering a player from the server
        /// </summary>
        /// <param name="aNetworkPeerInfo"></param>
        [RPC]
        private void AuthS_UnregisterRequest(byte[] aNetworkPeerInfo, NetworkMessageInfo aInfo)
        {
            NetworkPeerInfo info = NetworkPacket.Deserialize<NetworkPeerInfo>(aNetworkPeerInfo);
            if (info == null)
            {
                networkView.RPC(AUTHC_UNREGISTER_REQUEST, aInfo.sender, 0);
                return;
            }
            if (AuthenticationServer.instance.Unregister(info))
            {
                networkView.RPC(AUTHC_UNREGISTER_REQUEST, aInfo.sender, 1);
            }
            else
            {
                networkView.RPC(AUTHC_UNREGISTER_REQUEST, aInfo.sender, 0);
            }
        }
        /// <summary>
        /// A RPC call to the client to return the result of their request
        /// </summary>
        /// <param name="aSuccess">0 == false, 1 == true</param>
        [RPC]
        private void AuthC_UnregisterRequest(int aSuccess)
        {
            AuthenticationClient.ReceiveUnregisterRequest(aSuccess);
        }
        /// <summary>
        /// A RPC call to the server to authenicate a peer type
        /// </summary>
        /// <param name="aNetworkPeerInfo"></param>
        [RPC]
        private void AuthS_AuthenticateRequest(byte[] aNetworkPeerInfo, NetworkMessageInfo aInfo)
        {
            NetworkPeerInfo info = NetworkPacket.Deserialize<NetworkPeerInfo>(aNetworkPeerInfo);
            if (info == null)
            {
                networkView.RPC(AUTHC_AUTHENTICATE_REQUEST, aInfo.sender, 0);
                return;
            }
            if (AuthenticationServer.instance.Authenticate(info))
            {
                networkView.RPC(AUTHC_AUTHENTICATE_REQUEST, aInfo.sender, 1);
            }
            else
            {
                networkView.RPC(AUTHC_AUTHENTICATE_REQUEST, aInfo.sender, 0);
            }
        }
        /// <summary>
        /// A RPC call to the client to return the result of their request
        /// </summary>
        /// <param name="aSuccess"></param>
        [RPC]
        private void AuthC_AuthenticateRequest(int aSuccess)
        {
            AuthenticationClient.ReceiveAuthenticationRequest(aSuccess);
        }
        #endregion
        #endregion

    }
}