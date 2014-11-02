using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using EndevGame;

#region CHANGE LOG
/* November,2,2014 - Nathan Hanlan, Added and implemented the authentication server.
 * 
 */
#endregion

namespace Gem
{
    [RequireComponent(typeof(NetworkView))]
    public class AuthenticationServer : MonoBehaviour
    {
        #region CONSTANTS
        private const int PORT_NUMBER = 24932;
        private const string SERVER_NAME = "OL_Gem_Authentication";
        private const string FILE_PEERS = "Peers";
        private const float SAVE_LOAD_EXECUTION_TIME = 0.75f;
        private const float AUTO_SAVE_TIME = 300.0f;
        #endregion

        #region SINGLETON
        /// <summary>
        /// A singleton instance of AuthenticationServer
        /// </summary>
        private static AuthenticationServer s_Instance = null;
        /// <summary>
        /// An accessor which creates an instance of AuthenticationServer if it is null
        /// </summary>
        public static AuthenticationServer instance
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
            s_Instance = persistant.GetComponent<AuthenticationServer>();
            if (s_Instance == null)
            {
                s_Instance = persistant.AddComponent<AuthenticationServer>();
            }

        }
        /// <summary>
        /// Sets the instance of the AuthenticationServer to the instance given.
        /// </summary>
        /// <param name="aInstance">The instance to make singleton</param>
        /// <returns></returns>
        private static bool SetInstance(AuthenticationServer aInstance)
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
        private static void DestroyInstance(AuthenticationServer aInstance)
        {
            if (s_Instance == aInstance)
            {
                s_Instance = null;
            }
        }
        #endregion
        /// <summary>
        /// If the server should run at the very start or not.
        /// </summary>
        [SerializeField]
        private bool m_RunAtStart = false;
        [SerializeField]
        private bool m_DrawGUI = false;
        /// <summary>
        /// Determines if the server is running or not
        /// </summary>
        private bool m_IsRunning = false;
        /// <summary>
        /// A list of all registered users.
        /// </summary>
        private HashSet<NetworkPeerInfo> m_RegisteredUsers = new HashSet<NetworkPeerInfo>();
        /// <summary>
        /// A file on disc that contains all the information about registered users.
        /// </summary>
        private FileStream m_FileStream = new FileStream("PeerCache", ".GD");
        /// <summary>
        /// A virtual file which contains all the registered peers
        /// </summary>
        private File m_PeerFile = null;
        /// <summary>
        /// The time until the next auto save.
        /// </summary>
        private float m_AutoSaveTimer = 0.0f;

        /// <summary>
        /// Determines if the server is currently saving or not
        /// </summary>
        private bool m_IsSaving = false;
        /// <summary>
        /// Determines if the server is currently loading or not.
        /// </summary>
        private bool m_IsLoading = false;
        
        /// <summary>
        /// Load the server data from disc and set the auto save timer.
        /// </summary>
        void Start()
        {
            LoadServerData();
            m_AutoSaveTimer = AUTO_SAVE_TIME;
            if(m_RunAtStart == true)
            {
                StartServer();
            }
        }

        void Update()
        {
            m_AutoSaveTimer -= Time.deltaTime;
            if(m_AutoSaveTimer < 0.0f)
            {
                if(!m_IsSaving && !m_IsLoading)
                {
                    SaveServerData();
                }
            }
        }

        void OnGUI()
        {
            if(m_DrawGUI == true)
            {
                if(GUI.Button(new Rect(Screen.width * 0.5f - 50.0f,Screen.height * 0.5f - 50.0f,100.0f,100.0f), "Shut Down"))
                {
                    CloseServer();
                }
            }
        }

        private void OnServerInitialized()
        {
            m_IsRunning = true;
            DebugUtils.Log("Server Running");
        }

        private void OnDisconnectedFromServer(NetworkDisconnection aInfo)
        {
            if(Network.isServer)
            {
                m_IsRunning = false;
            }
        }

        /// <summary>
        /// Starts the authentication server.
        /// </summary>
        private void StartServer()
        {
            NetworkConnectionError error = Network.InitializeServer(32, PORT_NUMBER, !Network.HavePublicAddress());
            if(error != NetworkConnectionError.NoError)
            {
                Debug.LogError(error);
                return;
            }
            MasterServer.RegisterHost(NetworkProperties.NETWORK_GAME_TYPE, SERVER_NAME);
        }
        /// <summary>
        /// Closes the server after 5 seconds.
        /// </summary>
        private void CloseServer()
        {
            networkView.RPC(NetworkRPC.AUTHC_SERVER_CLOSE, RPCMode.All);
            StartCoroutine(CloseServerRoutine());
            DebugUtils.Log("Shutting down server...");
        }
        /// <summary>
        /// A routine to close to the server after 5 seconds.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CloseServerRoutine()
        {
            yield return new WaitForSeconds(5.0f);
            DebugUtils.Log("Server Shut Down.");
            Network.Disconnect();
            Application.Quit();
        }
        /// <summary>
        /// Registers a user with the server.
        /// </summary>
        /// <param name="aInfo"></param>
        /// <returns></returns>
        public bool Register(NetworkPeerInfo aInfo)
        {
            if(aInfo == null)
            {
                return false;
            }
            NetworkPeerInfo exists = m_RegisteredUsers.First(Element => Element.username == aInfo.username);
            if(exists != null)
            {
                return false;
            }
            m_RegisteredUsers.Add(aInfo);
            return true;
        }
        /// <summary>
        /// Unregisters a peer from the server. Returns true if the existed and were removed. 
        /// </summary>
        /// <param name="aInfo"></param>
        /// <returns>Returns false if the user was not found or their were multiple users removed</returns>
        public bool Unregister(NetworkPeerInfo aInfo)
        {
            if(aInfo == null)
            {
                return false;
            }
            return m_RegisteredUsers.RemoveWhere(Element => Element.username == aInfo.username) == 1;
        }
        /// <summary>
        /// Determines if a peer exists or not within the server
        /// </summary>
        /// <param name="aInfo"></param>
        /// <returns>Returns true if they do exist, returns false if they do not</returns>
        public bool Authenticate(NetworkPeerInfo aInfo)
        {
            return m_RegisteredUsers.First(Element => Element.username == aInfo.username) != null;
        }
        
        /// <summary>
        /// Invokes the Coroutine OnSaveServer to save all the data in the server as NetworkPeerInfo.PeerSavedInfo
        /// </summary>
        private void SaveServerData()
        {
            if(m_IsSaving == true)
            {
                return;
            }
            m_PeerFile = m_FileStream.Get(FILE_PEERS);
            if (m_PeerFile == null)
            {
                m_PeerFile = m_FileStream.Add(FILE_PEERS);
            }
            m_PeerFile.Clear();
            m_IsSaving = true;
            StartCoroutine(SaveServerDataRoutine());
        }
        /// <summary>
        /// Saves all the data but waits for end of frame if current execution time has exceeded the max.
        /// </summary>
        /// <returns></returns>
        private IEnumerator SaveServerDataRoutine()
        {
            float startExecutionTime = Time.time;
            IEnumerator<NetworkPeerInfo> iter = m_RegisteredUsers.GetEnumerator();
            while(iter.MoveNext())
            {
                if(iter.Current == null)
                {
                    continue;
                }
                m_PeerFile.Add(iter.Current.toSavedInfo);
                if(Time.time - startExecutionTime > SAVE_LOAD_EXECUTION_TIME)
                {
                    startExecutionTime = Time.time;
                    yield return new WaitForEndOfFrame();
                }
            }
            m_FileStream.Save();
            OnSaveFinish();
        }
        /// <summary>
        /// Gets called when saving was finished.
        /// </summary>
        private void OnSaveFinish()
        {
            m_PeerFile.Clear();
            m_IsSaving = false;
            m_AutoSaveTimer = AUTO_SAVE_TIME;
        }
        /// <summary>
        /// Loads all content into the authentication server.
        /// </summary>
        private void LoadServerData()
        {
            if(m_IsLoading == true)
            {
                return;
            }
            m_FileStream.Load(true);
            m_PeerFile = m_FileStream.Get(FILE_PEERS);
            m_RegisteredUsers.Clear();
            m_IsLoading = true;
            StartCoroutine(LoadServerDataRoutine());
        }
        /// <summary>
        /// A courtine for loading server data 
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadServerDataRoutine()
        {
            if (m_PeerFile != null)
            {
                float startExecutionTime = Time.time;
                IEnumerator<NetworkPeerInfo.PeerSavedInfo> iter = m_PeerFile.GetAll<NetworkPeerInfo.PeerSavedInfo>().GetEnumerator();
                while (iter.MoveNext())
                {
                    if (iter.Current == null)
                    {
                        continue;
                    }
                    m_RegisteredUsers.Add(iter.Current.toPeerInfo);
                    if (Time.time - startExecutionTime > SAVE_LOAD_EXECUTION_TIME)
                    {
                        startExecutionTime = Time.time;
                        yield return new WaitForEndOfFrame();
                    }
                }
            }
            OnLoadFinish();
        }
        /// <summary>
        /// Gets called when the server finishes loading.
        /// </summary>
        private void OnLoadFinish()
        {
            if (m_PeerFile != null)
            {
                m_PeerFile.Clear();
            }
            m_IsLoading = false;
        }


        public bool isRunning
        {
            get { return m_IsRunning; }
        }
        public bool isSaving
        {
            get { return m_IsSaving; }
        }
        public bool isLoading
        {
            get { return m_IsLoading; }
        }
    }
}