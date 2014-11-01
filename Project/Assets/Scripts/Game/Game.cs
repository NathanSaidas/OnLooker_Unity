using UnityEngine;
using System.Reflection;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gem
{

    public class Game : MonoBehaviour
    {
        #region EDITOR MENU
#if UNITY_EDITOR

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
        }

        [MenuItem("CONTEXT/Game/Clear Scenes")]
        private static void ClearScenes(MenuCommand aCommand)
        {
            Game context = aCommand.context as Game;
            context.m_Scenes.Clear();
        }

        [MenuItem("CONTEXT/Game/Hack Unity")]
        private static void Hack(MenuCommand aCommand)
        {
            Type appType = typeof(Shader);
            FieldInfo[] appFields = appType.GetFields(BindingFlags.NonPublic | BindingFlags.Static);
            for(int i = 0; i < appFields.Length; i++)
            {
                Debug.Log(appFields[i].Name);
            }
            Debug.Log("Instances");
            appFields = appType.GetFields(BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance);
            FieldInfo inspectedField = null;
            for (int i = 0; i < appFields.Length; i++)
            {
                if(appFields[i].Name == "m_TerrainData")
                {
                    inspectedField = appFields[i];
                }
                Debug.Log(appFields[i].Name);
            }
            if(inspectedField != null)
            {
                Debug.Log("Inspecting Type");
                Debug.Log(inspectedField.FieldType.Name);
                return;   
            }
            Debug.Log("Methods");
            MethodInfo[] methodInfos = appType.GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo lineIntersection = null;
            for (int i = 0; i < methodInfos.Length; i++)
            {
                Debug.Log(methodInfos[i].Name);
                if(methodInfos[i].Name == "LineIntersection")
                {
                    lineIntersection = methodInfos[i];
                }
            }

            if (lineIntersection != null)
            {
                Debug.Log("Line Intersection");
                Debug.Log(lineIntersection.ReturnType.Name);
                IEnumerator<ParameterInfo> iter = lineIntersection.GetParameters().AsEnumerable<ParameterInfo>().GetEnumerator();
                while(iter.MoveNext())
                {
                    Debug.Log("Arg: " + iter.Current.ParameterType.Name);
                }
                
            }

        }
#endif
        #endregion

        public const string PERSISTANT_GAME_OBJECT_NAME = "_Persistant";

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
            if (s_Instance != null)
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
        //UnityEditor.EditorBuildSettingsScene scene = new UnityEditor.EditorBuildSettingsScene();
        [SerializeField]
        private GameScene m_StartScene = null;
        [SerializeField]
        private List<GameScene> m_Scenes = new List<GameScene>();
        private GameScene m_LoadedScene = null;
        private HashSet<IGameListener> m_GameListeners = new HashSet<IGameListener>();
        private bool m_IsPaused = false;

        //Load inputs
        private string m_LoadTargetSceneName = string.Empty;
        private Texture m_LoadTargetTexture = null;

        #endregion

        
        
        void Start()
        {
            if(!SetInstance(this))
            {
                Destroy(this);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }
        void OnDestroy()
        {
            DestroyInstance(this);
        }

        public static void LoadLevel(string aLevelName)
        {
            instance.m_LoadTargetSceneName = aLevelName;
            instance.m_LoadTargetTexture = null;
            instance.Load();
        }
        public static void LoadLevel(string aLevelName, Texture aLoadScreen)
        {
            instance.m_LoadTargetSceneName = aLevelName;
            instance.m_LoadTargetTexture = aLoadScreen;
            instance.Load();
        }
        public static void LoadLevelPath(string aPathName)
        {
            instance.m_LoadTargetSceneName = aPathName;
            instance.m_LoadTargetTexture = null;
            instance.LoadPath();
        }
        public static void LoadLevelPath(string aPathName, Texture aLoadScreen)
        {
            instance.m_LoadTargetSceneName = aPathName;
            instance.m_LoadTargetTexture = aLoadScreen;
            instance.LoadPath();
        }
        public static void ReloadLevel()
        {
            
            instance.Reload();
        }
        public static void ReloadLevel(Texture aLoadScreen)
        {

        }
        public static void PauseGame()
        {
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
        }
        public static void UnpauseGame()
        {
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
        }
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

        public static void Register(IGameListener aListener)
        {
            if(aListener != null && !instance.m_GameListeners.Contains(aListener))
            {
                instance.m_GameListeners.Add(aListener);
            }
        }
        public static void Unregister(IGameListener aListener)
        {
            if (aListener != null )
            {
                instance.m_GameListeners.Remove(aListener);
            }
        }
        
        private void Load()
        {

        }
        private void LoadPath()
        {

        }
        private void Reload()
        {

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
    }
}