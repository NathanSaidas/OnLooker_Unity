using UnityEngine;
using System.Collections;

namespace Gem
{
    [RequireComponent(typeof(NetworkView))]
    public class NetworkManager : MonoBehaviour
    {
        #region SINGLETON
        /// <summary>
        /// A singleton instance of NetworkManager
        /// </summary>
        private static NetworkManager s_Instance = null;
        /// <summary>
        /// An accessor which creates an instance of NetworkManager if it is null
        /// </summary>
        private static NetworkManager instance
        {
            get { if (s_Instance == null) { CreateInstance(); } return s_Instance; }
        }
        /// <summary>
        /// Creates an instance of the NetworkManager if it was missing.
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
            s_Instance = persistant.GetComponent<NetworkManager>();
            if (s_Instance == null)
            {
                s_Instance = persistant.AddComponent<NetworkManager>();
            }

        }
        /// <summary>
        /// Sets the instance of the NetworkManager to the instance given.
        /// </summary>
        /// <param name="aInstance">The instance to make singleton</param>
        /// <returns></returns>
        private static bool SetInstance(NetworkManager aInstance)
        {
            if (s_Instance != null && s_Instance != aInstance)
            {
                return false;
            }
            s_Instance = aInstance;
            return true;
        }
        /// <summary>
        /// Removes the instance of the NetworkManager if the instance being destroyed is the the same as the singleton.
        /// </summary>
        /// <param name="aInstance"></param>
        private static void DestroyInstance(NetworkManager aInstance)
        {
            if (s_Instance == aInstance)
            {
                s_Instance = null;
            }
        }
        #endregion

        #region MEMBERS
        

        #region UNITY_CALLBACKS
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        #endregion


        


        #endregion
        #region STATIC

        #endregion


    }
}