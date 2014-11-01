﻿using UnityEngine;
using System.Reflection;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

#region CHANGE LOG
/* October,31,2014 - Nathan Hanlan, Added Game class
 * November,1,2014 - Nathan Hanlan, Implemented Load Methods
 */
#endregion 

namespace Gem
{

    public class Game : MonoBehaviour
    {
        #region EDITOR MENU
#if UNITY_EDITOR
        /// <summary>
        /// Updates the scene list of the Game Manager
        /// </summary>
        /// <param name="aCommand"></param>
        [MenuItem("CONTEXT/Game/Update Scenes")]
        private static void UpdateScenes(MenuCommand aCommand)
        {
            Game context = aCommand.context as Game;
            IEnumerator iter = EditorBuildSettings.scenes.GetEnumerator();
            while (iter.MoveNext())
            {
                EditorBuildSettingsScene current = iter.Current as EditorBuildSettingsScene;
                if (current == null)
                {
                    continue;
                }
                int lastIndex = current.path.LastIndexOf('/') + 1;
                int endIndex = current.path.LastIndexOf('.');
                int length = endIndex - lastIndex;
                string sceneName = current.path.Substring(lastIndex, length);
                context.m_Scenes.Add(new GameScene(sceneName, current.path));
            }
            EditorUtility.SetDirty(context);
        }

        /// <summary>
        /// Clears the scene list of the game manager.
        /// </summary>
        /// <param name="aCommand"></param>
        [MenuItem("CONTEXT/Game/Clear Scenes")]
        private static void ClearScenes(MenuCommand aCommand)
        {
            Game context = aCommand.context as Game;
            context.m_Scenes.Clear();
            EditorUtility.SetDirty(context);
        }

#endif
        #endregion

        public const string PERSISTANT_GAME_OBJECT_NAME = "_Persistant";

        public const string EMPTY_SCENE = "Empty_Scene";
        public const string INIT_SCENE = "Init_Scene";
        public const string FINAL_SCENE = "Final_Scene";
        #region SINGLETON
        /// <summary>
        /// A singleton instance of Game
        /// </summary>
        private static Game s_Instance = null;
        /// <summary>
        /// An accessor which creates an instance of Game Manager if it is null
        /// </summary>
        private static Game instance
        {
            get { if (s_Instance == null) { CreateInstance(); } return s_Instance; }
        }
        /// <summary>
        /// Creates an instance of the Game if it was missing.
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
            s_Instance = persistant.GetComponent<Game>();
            if (s_Instance == null)
            {
                s_Instance = persistant.AddComponent<Game>();
            }
        }
        /// <summary>
        /// Sets the instance of the game manager to the instance given.
        /// </summary>
        /// <param name="aInstance">The instance to make singleton</param>
        /// <returns></returns>
        private static bool SetInstance(Game aInstance)
        {
            if (s_Instance != null && s_Instance != aInstance)
            {
                return false;
            }
            s_Instance = aInstance;
            return true;
        }
        /// <summary>
        /// Removes the instance of the game manager if the instance being destroyed is the the same as the singleton.
        /// </summary>
        /// <param name="aInstance"></param>
        private static void DestroyInstance(Game aInstance)
        {
            if (s_Instance == aInstance)
            {
                s_Instance = null;
            }
        }
        #endregion


        #region FIELDS
        /// <summary>
        /// The scene to load at start.
        /// </summary>
        [SerializeField]
        private string m_StartScene = string.Empty;
        /// <summary>
        /// A list of all possible game scenes to load.
        /// </summary>
        [SerializeField]
        private List<GameScene> m_Scenes = new List<GameScene>();
        /// <summary>
        /// The current scene loaded.
        /// </summary>
        private GameScene m_LoadedScene = null;
        /// <summary>
        /// The target scene to load
        /// </summary>
        private GameScene m_TargetScene = null;
        /// <summary>
        /// A list of game listeners. 
        /// </summary>
        private HashSet<IGameListener> m_GameListeners = new HashSet<IGameListener>();
        /// <summary>
        /// Determines if the game is paused or not.
        /// </summary>
        private bool m_IsPaused = false;
        /// <summary>
        /// Determines if the game is loading or not
        /// </summary>
        private bool m_IsLoading = false;
        /// <summary>
        /// Determines if the game is loading or not.
        /// </summary>
        private bool m_IsUnloading = false;
        /// <summary>
        /// The load scene target name
        /// </summary>
        private string m_LoadTargetSceneName = string.Empty;
        /// <summary>
        /// The texture to show for loading screen override.
        /// </summary>
        private Texture m_LoadTargetTexture = null;

        #endregion
        /// <summary>
        /// Initialize the game manager.
        /// </summary>
        void Start()
        {
            if(!SetInstance(this))
            {
                Destroy(this);
                return;
            }
            DontDestroyOnLoad(gameObject);
            if (m_StartScene.Length != 0)
            {
                LoadLevel(m_StartScene);
            }
            //StartCoroutine(LateStart());
        }
        /// <summary>
        /// Destroys the game manager.
        /// </summary>
        void OnDestroy()
        {
            DestroyInstance(this);
        }

        void OnLevelWasLoaded(int aIndex)
        {
            string sceneName = Application.loadedLevelName;


            ///Load the scene, finished unloading
            if (sceneName == EMPTY_SCENE)
            {
                if (m_TargetScene != null)
                {
                    Application.LoadLevel(m_TargetScene.sceneName);
                    m_IsUnloading = false;
                    m_IsLoading = true;
                    GameEventManager.InvokeEvent(new GameEventData(Time.time, GameEventID.GAME_LEVEL_UNLOAD_FINISH, GameEventType.GAME, this, loadedScene));
                    GameEventManager.InvokeEvent(new GameEventData(Time.time, GameEventID.GAME_LEVEL_LOAD_BEGIN, GameEventType.GAME, this, targetScene));
                }
            }
            //Finished Loading Target scene
            else if (sceneName == m_TargetScene.sceneName)
            {
                m_LoadedScene = m_TargetScene;
                m_IsLoading = false;
                GameEventManager.InvokeEvent(new GameEventData(Time.time, GameEventID.GAME_LEVEL_LOAD_FINISH, GameEventType.GAME, this, targetScene));
            }
        }
        /// <summary>
        /// A couroutine for a late start to allow all the managers to register.
        /// </summary>
        /// <returns></returns>
        //private IEnumerator LateStart()
        //{
        //    Debug.Log("Late Start");
        //    yield return new WaitForSeconds(1.0f);
        //    
        //}
        /// <summary>
        /// Loads the level by level name
        /// </summary>
        /// <param name="aLevelName">The level name to load.</param>
        public static void LoadLevel(string aLevelName)
        {
            if(aLevelName.Length == 0 || instance.m_IsLoading || instance.m_IsUnloading)
            {
                return;
            }
            instance.m_LoadTargetSceneName = aLevelName;
            instance.m_LoadTargetTexture = null;
            instance.Load();
        }
        /// <summary>
        /// Loads the level by level name and uses the texture as opposed to the default one.
        /// </summary>
        /// <param name="aLevelName">The level name to load.</param>
        /// <param name="aLoadScreen">The texture to use for loading.</param>
        public static void LoadLevel(string aLevelName, Texture aLoadScreen)
        {
            if (aLevelName.Length == 0 || instance.m_IsLoading || instance.m_IsUnloading)
            {
                return;
            }
            instance.m_LoadTargetSceneName = aLevelName;
            instance.m_LoadTargetTexture = aLoadScreen;
            instance.Load();
        }
        /// <summary>
        /// Loads a level by full path name.
        /// </summary>
        /// <param name="aPathName">The path name to load.</param>
        public static void LoadLevelPath(string aPathName)
        {
            if (aPathName.Length == 0 || instance.m_IsLoading || instance.m_IsUnloading)
            {
                return;
            }
            instance.m_LoadTargetSceneName = aPathName;
            instance.m_LoadTargetTexture = null;
            instance.LoadPath();
        }
        /// <summary>
        /// Loads a level by full path name.
        /// </summary>
        /// <param name="aPathName">The path name to load.</param>
        /// <param name="aLoadScreen">The texture to use for loading.</param>
        public static void LoadLevelPath(string aPathName, Texture aLoadScreen)
        {
            if (aPathName.Length == 0 || instance.m_IsLoading || instance.m_IsUnloading)
            {
                return;
            }
            instance.m_LoadTargetSceneName = aPathName;
            instance.m_LoadTargetTexture = aLoadScreen;
            instance.LoadPath();
        }
        /// <summary>
        /// Reloads the current level.
        /// </summary>
        public static void ReloadLevel()
        {
            if(instance.m_IsLoading || instance.m_IsUnloading)
            {
                return;
            }
            instance.Reload();
        }
        /// <summary>
        /// Reloads the current level with the speciied texture.
        /// </summary>
        /// <param name="aLoadScreen"></param>
        public static void ReloadLevel(Texture aLoadScreen)
        {
            if(instance.m_IsLoading || instance.m_IsUnloading)
            {
                return;
            }
            instance.m_LoadTargetTexture = aLoadScreen;
            instance.Reload();
        }
        /// <summary>
        /// Restarts the game from the start scene.
        /// </summary>
        public static void Restart()
        {
            instance.m_LoadTargetSceneName = instance.m_StartScene;
            instance.Load();
        }
        /// <summary>
        /// Pauses the game if the game is not already paused.
        /// </summary>
        public static void PauseGame()
        {
            if(instance.m_IsPaused == true)
            {
                return;
            }
            instance.m_IsPaused = true;
            IEnumerator<IGameListener> iter = instance.m_GameListeners.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current == null)
                {
                    continue;
                }
                iter.Current.OnGamePaused();
            }
            GameEventManager.InvokeEvent(new GameEventData(Time.time, GameEventID.GAME_PAUSED, GameEventType.GAME, instance, null));
        }
        /// <summary>
        /// Unpauses the game if the game is not already paused.
        /// </summary>
        public static void UnpauseGame()
        {
            if(instance.m_IsPaused == false)
            {
                return;
            }
            instance.m_IsPaused = false;
            IEnumerator<IGameListener> iter = instance.m_GameListeners.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current == null)
                {
                    continue;
                }
                iter.Current.OnGameUnpaused();
            }
            GameEventManager.InvokeEvent(new GameEventData(Time.time, GameEventID.GAME_UNPAUSED, GameEventType.GAME, instance, null));
        }
        /// <summary>
        /// Resets the game.
        /// </summary>
        public static void ResetGame()
        {
            IEnumerator<IGameListener> iter = instance.m_GameListeners.GetEnumerator();
            while(iter.MoveNext())
            {
                if(iter.Current == null)
                {
                    continue;
                }
                iter.Current.OnGameReset();
            }
        }
        /// <summary>
        /// Registers a game listener for pause / unpause / reset events.
        /// </summary>
        /// <param name="aListener"></param>
        public static void Register(IGameListener aListener)
        {
            if(aListener != null && !instance.m_GameListeners.Contains(aListener))
            {
                instance.m_GameListeners.Add(aListener);
            }
        }
        /// <summary>
        /// Unregisters a game listener.
        /// </summary>
        /// <param name="aListener"></param>
        public static void Unregister(IGameListener aListener)
        {
            if (aListener != null )
            {
                instance.m_GameListeners.Remove(aListener);
            }
        }
        /// <summary>
        /// Loads a level by scene name
        /// </summary>
        private void Load()
        {
            GameScene scene = GetScene(m_LoadTargetSceneName);
            if(scene == null)
            {
                DebugUtils.LogWarning("Missing scene " + m_LoadTargetSceneName);
                m_LoadTargetSceneName = string.Empty;
                m_LoadTargetTexture = null;
                return;
            }
            m_TargetScene = scene;
            Application.LoadLevel(EMPTY_SCENE);
            m_IsUnloading = true;
            GameEventManager.InvokeEvent(new GameEventData(Time.time, GameEventID.GAME_LEVEL_UNLOAD_BEGIN, GameEventType.GAME, this, loadedScene));
            m_LoadTargetSceneName = string.Empty;
            m_LoadTargetTexture = null;
        }
        /// <summary>
        /// Loads a level by path name
        /// </summary>
        private void LoadPath()
        {
            GameScene scene = GetSceneByPath(m_LoadTargetSceneName);
            if(scene == null)
            {
                DebugUtils.LogWarning("Missing scene " + m_LoadTargetSceneName);
                m_LoadTargetSceneName = string.Empty;
                m_LoadTargetTexture = null;
                return;
            }
            m_TargetScene = scene;
            Application.LoadLevel(EMPTY_SCENE);
            m_IsUnloading = true;
            GameEventManager.InvokeEvent(new GameEventData(Time.time, GameEventID.GAME_LEVEL_UNLOAD_BEGIN, GameEventType.GAME, this, loadedScene));
            m_LoadTargetSceneName = string.Empty;
            m_LoadTargetTexture = null;
        }
        /// <summary>
        /// Reloads the level and sends an event to the game manager.
        /// </summary>
        private void Reload()
        {
            m_TargetScene = m_LoadedScene;
            Application.LoadLevel(EMPTY_SCENE);
            m_IsUnloading = true;
            GameEventManager.InvokeEvent(new GameEventData(Time.time, GameEventID.GAME_LEVEL_UNLOAD_BEGIN, GameEventType.GAME, this, loadedScene));
            m_LoadTargetTexture = null;
        }
        /// <summary>
        /// Gets a scene name by the name of the scene.
        /// </summary>
        /// <param name="aName">The name of the scene</param>
        /// <returns></returns>
        private GameScene GetScene(string aName)
        {
            return m_Scenes.First(Element => Element.sceneName == aName);
        }
        /// <summary>
        /// Gets a scene name by the path name
        /// </summary>
        /// <param name="aPathName">The full file path to search for.</param>
        /// <returns></returns>
        private GameScene GetSceneByPath(string aPathName)
        {
            return m_Scenes.First(Element => Element.scenePath == aPathName);
        }

        public static string startScene
        {
            get { return instance.m_StartScene; }
            set { instance.m_StartScene = value; }
        }
        public static GameScene loadedScene
        {
            get { return instance.m_LoadedScene; }
        }
        public static GameScene targetScene
        {
            get { return instance.m_TargetScene; }
        }
        public static bool isPaused
        {
            get { return instance.m_IsPaused; }
        }
        public static bool isLoading
        {
            get { return instance.m_IsLoading; }
        }
        public static bool isUnloading
        {
            get { return instance.m_IsUnloading; }
        }

    }
}