using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using OnLooker;
using EndevGame;

[Serializable]
    public class SceneInfo
    {
        [SerializeField]
        private string m_Name = string.Empty;
        [SerializeField]
        private float m_LoadDelay = 2.0f; //The time it takes to load this screen

        public string name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        public float loadDelay
        {
            get { return m_LoadDelay; }
            set { m_LoadDelay = value;  }
        }
            
    }


    public delegate void SceneLoadCallback(SceneInfo aInfo);

    //The purpose of Game Manager is to manage where the player is in the game, and manage the state of game between menu, gameplay and gameplay paused.
    //This class is a singleton and is designed to be on the init_scene  _Tools gameobject
    public sealed class GameManager : MonoBehaviour, IDebugWatch
    {
        #region CONSTANTS
        //Layer Constants
        public const int OBJECT_INTERACTION_LAYER = 8;
        public const int SURFACE_LAYER = 10;
        public const int PLANT_LIGHT_LAYER = SURFACE_LAYER;
        public const int CLIMB_LAYER = 11;
        //Scene Name Constants
        public const string SPLASH_SCENE = "splash_scene";
        public const string MAIN_MENU_SCENE = "main_menu_scene";
        public const string LEVEL_01_SCENE = "Level_01_Tutorial";

        //File name
        public const string SAVE_GAMES = "Save_Games";

        #endregion

        #region SINGLETON
        private static GameManager s_Instance = null;

        public static GameManager instance
        {
            get { return s_Instance; }
        }
        #endregion

        
        private enum State
        {
            NOT_LOADING,
            UNLOADING,
            LOADING
        }
        
        private void Start()
        {
            if(s_Instance == null)
            {
                s_Instance = this;
            }
            else
            {
                Debug.LogWarning("Trying to create multiple GameManagers failed.");
                Destroy(this);
            }
            DontDestroyOnLoad(gameObject);
            init();

            
        }
        private void OnDestroy()
        {
            onSaveGame();
            if(s_Instance == this)
            {
                s_Instance = null;
            }
        }


        //A reference to the load screen component kept within the persistant hierarchy
        //And the texture to display for load screens
        //[SerializeField]
        //private LoadScreen m_LoadScreen = null;
        [SerializeField]
        private Texture m_LoadTexture = null;
        
        //The list of scenes and their load delay times
        [SerializeField]//new 
        private List<SceneInfo> m_Scenes = new List<SceneInfo>();

        //The loading state of the GameManager. 
        [HideInInspector]
        [SerializeField]//new
        private State m_State = State.NOT_LOADING;

        //The current time left till the load is done
        [HideInInspector]
        [SerializeField]//new
        private float m_CurrentLoadTime = 0.0f;

        //The scene the Game Manager is trying to load
        [HideInInspector]
        [SerializeField]//new
        private SceneInfo m_TargetScene = null;
        //The scene the Game Manager is currently on
        [HideInInspector]
        [SerializeField]//new
        private SceneInfo m_CurrentScene = null;


        private SceneLoadCallback m_OnLoadBegin = null;
        private SceneLoadCallback m_OnLoadFinished = null;


        //The List of Check Points within the game world
        [HideInInspector]
        [SerializeField]
        private List<CheckPoint> m_CheckPoints = new List<CheckPoint>();

        //The players profile?
        ///[HideInInspector]
        ///[SerializeField]
        ///private PlayerProfile m_PlayerProfile = new PlayerProfile();
        //A reference to the file data class to save player profiles?
        [HideInInspector]
        [SerializeField]
        private FileData m_FileData = null;
        

        //A list of game objects that are Ipauseable that will listen in on pause events
        //private List<IPauseable> m_PausableObjects = new List<IPauseable>();


        //This boolean determines if the gameplay scene is paused or not
        [HideInInspector]
        [SerializeField]
        private bool m_IsPaused = true;

        //This boolean determines if the current scene is a gameplay scene or not
        [HideInInspector]
        [SerializeField]
        private bool m_IsGameplay = false;


        //The prefab of the player to be spawned in each level
        [SerializeField]
        private GameObject m_PlayerPrefab = null;
        //The current or most recent gameplay level
        //private string m_Level = string.Empty;
        //The current or most recent gameplay checkpoint.
        private int m_CheckPoint = 0;

        private bool m_SpawnPlayerRequest = false;

        //[SerializeField]
        //private Camera m_Camera = null;
        //[SerializeField]
        //private CharacterManager m_Player = null;

        [SerializeField]
        private string m_StartScene = "splash_scene";


       
            
        //Gets called after all the singleton business has been done
        private void init()
        {
            //Load Managers here

            
            //Then load the splash scene
            //Application.LoadLevel(m_StartScene);
            loadLevel(m_StartScene);
            //SceneInfo scene = getScene(m_StartScene);
            m_FileData = new FileData(Application.dataPath + "/" + GameManager.SAVE_GAMES + ".bin");

            Debug.Log("Add Watch");
            DebugUtils.addWatch(this);
        }



        //Unity callback to check when a level was loaded EXCEPT the first one
        private void OnLevelWasLoaded(int aLevel)
        {
            if(Application.loadedLevelName == "final_scene")
            {
                //Load the exit scene
                finalSceneLoaded();
                return;
            }
            if(Application.loadedLevelName == "empty_scene")
            {
                m_State = State.LOADING;
                //if(m_LoadScreen != null)
                //{
                //    m_LoadScreen.setTexture(m_LoadTexture);
                //    m_LoadScreen.gameObject.SetActive(true);
                //}
            }
            else
            {
                //On Load Finished
                onLoadFinished();
                m_State = State.NOT_LOADING;
                m_CurrentScene = m_TargetScene;
                m_TargetScene = null;
                //if(m_LoadScreen != null)
                //{
                //    m_LoadScreen.gameObject.SetActive(false);
                //}
            }
        }

        
        private void Update()
        {
            //if(m_IsPaused == true)
            //{
            //    for(int i = 0; i < m_PausableObjects.Count; i++)
            //    {
            //        m_PausableObjects[i].onPause();
            //    }
            //}
            if(m_State == State.LOADING)
            {
                if(m_CurrentLoadTime > m_TargetScene.loadDelay)
                {
                    Application.LoadLevel(m_TargetScene.name);
                    m_CurrentLoadTime = 0.0f;
                    return;
                }
                m_CurrentLoadTime += Time.deltaTime;
            }
            else if(m_State != State.NOT_LOADING)
            {
                if(m_CurrentScene.name != MAIN_MENU_SCENE && isPaused == false)
                {
                    //Debug.Log("Locking Cursor");
                    Screen.lockCursor = true;
                }
                else
                {
                    //Debug.Log("Unlocking cursor");
                    Screen.lockCursor = false;
                }
            }
        }

        public void onReport()
        {
            DebugUtils.drawWatch("GameManager", "Current Scene", m_CurrentScene.name);
            DebugUtils.drawWatch("GameManager", "Paused", isPaused);
        }

        //A controlled load level function to check if were currently unloading / loading and if the requested scene exists
        private void loadLevel(string aSceneName)
        {
            if(m_State != State.NOT_LOADING)
            {
                Debug.LogWarning("Currently Loading a Level. Aborting loadLevel request");
                return;
            }
            if(aSceneName == string.Empty || aSceneName == "empty_scene" || aSceneName == "init_scene" || aSceneName == "final_scene")
            {
                
                return;
            }

            SceneInfo scene = getScene(aSceneName);
            if(scene == null)
            {
                Debug.LogError("A scene with the name " + aSceneName + " does not currently exist.");
                return;
            }

            m_TargetScene = scene;

            onLoadRequested();
            m_State = State.UNLOADING;
            Application.LoadLevel("empty_scene");
        }

        //private void loadPlayer(PlayerProfile aProfile)
        //{
        //    loadLevel(aProfile.level);
        //    m_PlayerProfile = aProfile;
        //}
        

        //Obsolete
        //private void levelLoaded(string aName)
        //{
        //    Debug.Log("Scene " + aName + " was successfully loaded.");
        //    if ((aName == "init_scene" || aName == "splash_scene" || aName == "main_menu_scene") == false)
        //    {
        //        Debug.Log("Spawn Request");
        //        m_Level = aName;
        //        m_CheckPoint = 0;
        //        m_SpawnPlayerRequest = true;
        //
        //        //m_Camera = CameraManager.instance.gameplayCamera;
        //    }
        //    else if(aName == "main_menu_scene")
        //    {
        //        CameraManager.instance.resetState();
        //    }
        //
        //}

        private void registerCheckPoint(CheckPoint aCheckPoint)
        {
            if (aCheckPoint == null)
            {
                return;
            }

            for (int i = 0; i < m_CheckPoints.Count; i++)
            {
                if (m_CheckPoints[i].id == aCheckPoint.id)
                {
                    Debug.LogWarning("Registering a checkpoint with the same id: " + aCheckPoint.id);
                }
            }

            Debug.Log(aCheckPoint.name + " registered.");

            m_CheckPoints.Add(aCheckPoint);
            if(aCheckPoint.id == 0 && m_PlayerPrefab != null && m_SpawnPlayerRequest == true)
            {
                GameObject player = (GameObject)Instantiate(m_PlayerPrefab, aCheckPoint.position, aCheckPoint.rotation);
                m_SpawnPlayerRequest = false;
            }
            

            //if(m_CurrentScene.name == m_PlayerProfile.level && aCheckPoint.id == m_PlayerProfile.checkpointID && m_PlayerPrefab != null && m_SpawnPlayerRequest == true)
            //{
            //    GameObject player = (GameObject)Instantiate(m_PlayerPrefab, aCheckPoint.position, aCheckPoint.rotation);
            //    CharacterManager manager = player.GetComponent<CharacterManager>();
            //
            //    if (manager != null)
            //    {
            //        //manager.characterCamera = m_Camera;
            //        m_Player = manager;
            //        m_Player.characterCamera = CameraManager.instance.gameplayCamera;
            //        //CameraManager.instance.gameplayCamera = m_Camera;
            //        //CameraManager.instance.resetState();
            //        CameraManager.instance.transitionToOrbit(m_Player.transform, CameraMode.INSTANT, 10.0f);
            //    }
            //    m_SpawnPlayerRequest = false;
            //}
            //else if(aCheckPoint.id == 0 && m_PlayerPrefab != null && m_SpawnPlayerRequest == true)
            //{
            //    GameObject player =  (GameObject)Instantiate(m_PlayerPrefab, aCheckPoint.position, aCheckPoint.rotation);
            //    CharacterManager manager = player.GetComponent<CharacterManager>();
            //
            //    if(manager != null)
            //    {
            //        //manager.characterCamera = m_Camera;
            //        m_Player = manager;
            //        m_Player.characterCamera = CameraManager.instance.gameplayCamera;
            //        //CameraManager.instance.gameplayCamera = m_Camera;
            //        //CameraManager.instance.resetState();
            //        CameraManager.instance.transitionToOrbit(m_Player.transform, CameraMode.INSTANT, 10.0f);
            //    }
            //    m_SpawnPlayerRequest = false;
            //}
        }
        private void unregisterCheckPoint(CheckPoint aCheckPoint)
        {
            if (aCheckPoint == null)
            {
                return;
            }
            Debug.Log(aCheckPoint.name + " unregistered.");
            m_CheckPoints.Remove(aCheckPoint);
        }

        //private void registerPausableObject(IPauseable aObject)
        //{
        //    if(aObject == null)
        //    {
        //        return;
        //    }
        //    if(m_PausableObjects.Contains(aObject))
        //    {
        //        Debug.LogError("IPauseable Object already exists within the list");
        //        return;
        //    } 
        //    m_PausableObjects.Add(aObject);
        //}
        //private void unregisterPausableObjects(IPauseable aObject)
        //{
        //    if (aObject == null)
        //    {
        //        return;
        //    }
        //    m_PausableObjects.Remove(aObject);
        //}

        //Pauses the game while in gameplay and not already paused
        //private void pauseGame()
        //{
        //    if(m_IsGameplay == true &&  m_IsPaused == false)
        //    {
        //        m_IsPaused = true;
        //        for(int i = 0; i < m_PausableObjects.Count; i++)
        //        {
        //            m_PausableObjects[i].onPauseBegin();
        //        }
        //    }
        //}
        ////Unpauses the game while in gameplay and not already paused
        //private void unPauseGame()
        //{
        //    if (m_IsGameplay == true && m_IsPaused == true)
        //    {
        //        m_IsPaused = false;
        //        for (int i = 0; i < m_PausableObjects.Count; i++)
        //        {
        //            m_PausableObjects[i].onPauseBegin();
        //        }
        //    }
        //}

        #region CALLBACKS
        private void onCheckPointReached(CheckPoint aCheckPoint)
        {
            if(aCheckPoint == null)
            {
                return;
            }
            if(m_CheckPoints.Contains(aCheckPoint))
            {
                aCheckPoint.checkPointReached();
                m_CheckPoint = aCheckPoint.id;


                //Write Custom Code Here
                //Debug.Log("Check point reached. Loading file data...");
                //m_FileData.load();
                //PlayerProfile[] profiles = m_FileData.get<PlayerProfile>();
                //if(profiles != null)
                //{
                //    Debug.Log("File data loaded. Searching for player...");
                //    for(int i = 0; i < profiles.Length; i++)
                //    {
                //        if(profiles[i].profileName == m_PlayerProfile.profileName)
                //        {
                //            Debug.Log("Player found.");
                //            Debug.Log("Saving current level: " + m_CurrentScene.name);
                //            Debug.Log("Saving current checkPoint: " + m_CheckPoint);
                //            profiles[i].level = instance.m_CurrentScene.name;
                //            profiles[i].checkpointID = instance.m_CheckPoint;
                //            m_FileData.clear();
                //            m_FileData.add(profiles);
                //            Debug.Log("Saving all players");
                //            m_FileData.save();
                //            return;
                //        }
                //    }
                //}
                //Debug.Log("Player Not Found not found.");

            }
        }
        //Gets called to save the game when the user requests a level load
        private void onSaveGame()
        {
            //Save Here
            //CameraManager.instance.gameplayCamera = null;
            //Destroy Checkpoints and player
            //if(m_Player != null)
            //{
            //    Destroy(m_Player.gameObject);
            //}
            
            for(int i = 0; i < m_CheckPoints.Count; i++)
            {
                if (m_CheckPoints[i] != null)
                {
                    Destroy(m_CheckPoints[i]);
                }
            }
        }
        private void finalSceneLoaded()
        {
            Debug.Log("Quitting...");
            Application.Quit();
        }
        private void onLoadRequested()
        {

        }
        
        private void onLoadFinished()
        {
            Debug.Log("Scene " + m_TargetScene.name + " was successfully loaded.");
            if(m_TargetScene.name != "init_scene" && m_TargetScene.name != "splash_Scene" && m_TargetScene.name != "main_menu_scene")
            {
                Debug.Log("Spawn Request");
                m_CheckPoint = 0;
                m_SpawnPlayerRequest = true;
            }
            else if(m_TargetScene.name == "main_menu_scene")
            {
                //CameraManager.instance.resetState();
                CameraManager.instance.disable();
            }
        }

        #endregion

        #region HELPERS

        public void exit()
        {
            Application.LoadLevel("final_scene");
        }
        private SceneInfo getScene(string aName)
        {
            for(int i = 0; i < m_Scenes.Count; i++)
            {
                if(m_Scenes[i] != null && m_Scenes[i].name == aName)
                {
                    return m_Scenes[i];
                }
            }
            return null;
        }

        #endregion


        #region STATIC_FUNCTIONS
        //static wrapper functions to make writing easier
        public static void loadScene(string aSceneName)
        {
            if(instance == null)
            {
                Debug.LogError("Missing Game Manager, start from \'init_scene\'.");
                return;
            }
            instance.loadLevel(aSceneName);
        }
        //public static void loadPlayerProfile(PlayerProfile aPlayer)
        //{
        //    if(instance == null)
        //    {
        //        Debug.LogError("Missing Game Manager, start from \'init_scene\'.");
        //        return;
        //    }
        //    instance.loadPlayer(aPlayer);
        //}
        public static void register(CheckPoint aCheckPoint)
        {
            if (instance == null)
            {
                Debug.LogError("Missing Game Manager, start from \'init_scene\'.");
                return;
            }
            instance.registerCheckPoint(aCheckPoint);
        }
        public static void unregister(CheckPoint aCheckPoint)
        {
            if (instance == null)
            {
                Debug.LogError("Missing Game Manager, start from \'init_scene\'.");
                return;
            }
            instance.unregisterCheckPoint(aCheckPoint);
        }
        //public static void register(IPauseable aObject)
        //{
        //    if(instance == null)
        //    {
        //        Debug.LogError("Missing Game Manager, start from \'init_scene\'.");
        //        return;
        //    }
        //    instance.registerPausableObject(aObject);
        //}
        //public static void unregister(IPauseable aObject)
        //{
        //    if(instance == null)
        //    {
        //        Debug.LogError("Missing Game Manager, start from \'init_scene\'.");
        //        return;
        //    }
        //    instance.unregisterPausableObjects(aObject);
        //}
        public static void pause()
        {
            if(instance == null)
            {
                Debug.LogError("Missing Game Manager, start from \'init_scene\'.");
                return;
            }
            //instance.pauseGame();
        }
        public static void unpause()
        {
            if(instance == null)
            {
                Debug.LogError("Missing Game Manager, start from \'init_scene\'.");
            }
            //instance.unPauseGame();
        }

        //Called when the player reaches a check point..
        public static void checkPointReached(CheckPoint aPoint)
        {
            if(instance == null)
            {
                Debug.LogError("Missing Game Manager, start from \'init_scene\'.");
            }
            else
            {
                
                //instance.m_PlayerProfile.name = Application.loadedLevelName; Debug.Log("Player Profile Name = " + Application.loadedLevelName);
                //instance.m_PlayerProfile.checkpointID = aPoint.id;           Debug.Log("Player Profile ID = " + aPoint.id);
                //instance.m_PlayerProfile.used = false;
                //instance.m_FileData.add(instance.m_PlayerProfile);
                //instance.m_FileData.save();
              
            }   
            instance.onCheckPointReached(aPoint);
        }

        //public static Camera sceneCamera
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            Debug.LogError("Missing Game Manager, start from \'init_scene\'.");
        //            return null;
        //        }
        //        return instance.m_Camera;
        //    }
        //}
        //public static void destroy(UnityEngine.Object aObject)
        //{
        //    if(aObject != null)
        //    {
        //        if(aObject.GetType().IsSubclassOf(typeof(IGameListener)))
        //        {
        //            ((IGameListener)aObject).onDisable();
        //            ((IGameListener)aObject).onDestroy();
        //        }
        //        Destroy(aObject);
        //    }
        //}

        #endregion

        //See field
        public static string requestedLevel
        {
            get { return instance == null ? string.Empty : instance.m_TargetScene.name; }
        }
        //See field
        public static bool loadingRequestedLevel
        {
            get { return instance == null ? false : (instance.m_State == State.LOADING); }
        }
        //See field
        public static bool isUnloading
        {
            get { return instance == null ? false : (instance.m_State == State.UNLOADING); }
        }
        //Returns true if the state of the game is in gameplay and is paused
        public static bool isPaused
        {
            get { return instance == null ? false :  (instance.m_IsPaused && instance.m_IsGameplay == true); }
        }
        //Returns true if the state of the game is in gameplay, false for menu
        public bool isGamePlay
        {
            get { return instance == null ? false : (instance.m_IsGameplay); }
        }
        //See field
        public string level
        {
            get { return instance == null ? string.Empty : m_CurrentScene == null ? m_CurrentScene.name : m_TargetScene.name; }
        }
        //See field
        public int checkPoint
        {
            get { return instance == null ? 0 : instance.m_CheckPoint; }
        }
        //See Field
        //public CharacterManager player
        //{
        //    get { return m_Player; }
        //}

        public static float currentLoadTimePercent
        {
            get
            {
                if (instance == null) { Debug.LogWarning("Missing Game Manager. Please start from \'init_scene\' or attach one to a gameobject."); return 0.0f; }
                if (instance.m_TargetScene == null) { return 100.0f; }
                return instance.m_CurrentLoadTime/instance.m_TargetScene.loadDelay;
            }
        }




        ///Game Code
        private Dictionary<string, object> m_GameEventGlobals = new Dictionary<string, object>();
        private GameEventArgs m_GameEventArgs = new GameEventArgs(GameEventID.NONE);
        

        public event GameEventCallback m_MushroomBounce;
        public event GameEventCallback m_CharacterInteraction;

        /// <summary>
        /// Registers a function to a game event
        /// </summary>
        /// <param name="aCallback">The pointer to the function to call</param>
        /// <param name="aGameEvent">The event you are targeting to register for</param>
        public void registerGameEvent(GameEventCallback aCallback, GameEventID aGameEvent)
        {
            if(aCallback != null)
            {
                switch(aGameEvent)
                {
                    case GameEventID.MUSHROOM_BOUNCE:
                        m_MushroomBounce += aCallback;
                        break;
                    case GameEventID.INTERACTION_PLAYER_ENTER:
                    case GameEventID.INTERACTION_PLAYER_EXIT:
                    case GameEventID.INTERACTION_PLAYER_FOCUS_BEGIN:
                    case GameEventID.INTERACTION_PLAYER_FOCUS_END:
                    case GameEventID.INTERACTION_PLAYER_ON_USE:
                    case GameEventID.INTERACTION_PLAYER_ON_USE_END:
                        m_CharacterInteraction += aCallback;
                        break;
                    default:
                        Debug.LogWarning("Invalid ID given in \'registerGameEvent\'.");
                        break;
                        
                }
            }
        }
        /// <summary>
        /// Register a function to a game event
        /// </summary>
        /// <param name="aCallback">The pointer to the function to call</param>
        /// <param name="aGameEvent">The event you are targeting to register for</param>
        public void unregisterGameEvent(GameEventCallback aCallback, GameEventID aGameEvent)
        {
            if(aCallback != null)
            {
                switch (aGameEvent)
                {
                    case GameEventID.MUSHROOM_BOUNCE:
                        if (m_MushroomBounce != null)
                        {
                            m_MushroomBounce -= aCallback;
                        }
                        break;
                    case GameEventID.INTERACTION_PLAYER_ENTER:
                    case GameEventID.INTERACTION_PLAYER_EXIT:
                    case GameEventID.INTERACTION_PLAYER_FOCUS_BEGIN:
                    case GameEventID.INTERACTION_PLAYER_FOCUS_END:
                    case GameEventID.INTERACTION_PLAYER_ON_USE:
                    case GameEventID.INTERACTION_PLAYER_ON_USE_END:
                        {
                            if(m_CharacterInteraction != null)
                            {
                                m_CharacterInteraction -= aCallback;
                            }
                        }
                        break;
                    default:
                        Debug.LogWarning("Invalid ID given in \'unregisterGameEvent\'.");
                        break;
                }
            }
        }


        public static void triggerEvent(GameEventID aGameEvent, GameEventArgs aArgs)
        {
            if(instance == null)
            {
                Debug.LogError("Missing the game manager. Start from the \'init_scene\'.");
                return;
            }
            instance.invokeEvent(aGameEvent,aArgs);

        }
        public static void registerEvent(GameEventCallback aCallback, GameEventID aGameEventID)
        {
            if(instance == null)
            {
                Debug.LogError("Missing the game manager. Start from the \'init_scene\'.");
                return;
            }
            instance.registerGameEvent(aCallback, aGameEventID);
        }
        public static void unregisterEvent(GameEventCallback aCallback, GameEventID aGameEventID)
        {
            if (instance == null)
            {
                Debug.LogError("Missing the game manager. Start from the \'init_scene\'.");
                return;
            }
            instance.unregisterGameEvent(aCallback, aGameEventID);
        }

        public static GameEventArgs eventArgs
        {
            get
            {
                if (instance == null)
                {
                    Debug.LogError("Missing the game manager. Start from the \'init_scene\'.");
                    return new GameEventArgs();
                }
                return instance.m_GameEventArgs;
            }
        }

        private void invokeEvent(GameEventID aGameEvent, GameEventArgs aArgs)
        {
            if(aGameEvent == GameEventID.NONE)
            {
                return;
            }

            setEventArgs(aArgs);

            invokeEvent(aGameEvent);

            resetEventArgs();

        }
        private void invokeEvent(GameEventID aID)
        {
            switch(aID)
            {
                case GameEventID.MUSHROOM_BOUNCE:
                    if(m_MushroomBounce != null)
                    {
                        m_MushroomBounce.Invoke(m_GameEventArgs.sender, aID);
                    }
                    break;
                case GameEventID.INTERACTION_PLAYER_ENTER:
                case GameEventID.INTERACTION_PLAYER_EXIT:
                case GameEventID.INTERACTION_PLAYER_FOCUS_BEGIN:
                case GameEventID.INTERACTION_PLAYER_FOCUS_END:
                case GameEventID.INTERACTION_PLAYER_ON_USE:
                case GameEventID.INTERACTION_PLAYER_ON_USE_END:
                    if(m_CharacterInteraction != null)
                    {
                        m_CharacterInteraction.Invoke(m_GameEventArgs.sender, aID);
                    }
                    break;
                default:
                    //Unhandled event was sent
                    break;
            }
        }
        private void setEventArgs(GameEventArgs aArgs)
        {
            m_GameEventArgs = aArgs;
        }

        private void resetEventArgs()
        {
            m_GameEventArgs = new GameEventArgs();
        }

        

        
        
        


    }
    public delegate void GameEventCallback(object aSender, GameEventID aEventID);
    public enum GameEventID
    {
        NONE,
        MUSHROOM_BOUNCE,
        INTERACTION_PLAYER_ENTER,
        INTERACTION_PLAYER_EXIT,
        INTERACTION_PLAYER_FOCUS_BEGIN,
        INTERACTION_PLAYER_FOCUS_END,
        INTERACTION_PLAYER_ON_USE,
        INTERACTION_PLAYER_ON_USE_END

    }

    public struct GameEventArgs
    {
        public GameEventArgs(GameEventID aID)
        {
            eventID = aID;
            sender = null;
            triggeringObject = null;
        }
        public GameEventID eventID;
        public object sender;
        public object triggeringObject;
    }
  