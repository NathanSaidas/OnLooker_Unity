using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using EndevGame;

#region CHANGE LOG
/* November,2,2014 - Nathan Hanlan, Added and implemented Game Options class with file saving / loading capability.
 * 
 */
#endregion 
namespace Gem
{
    public class GameOptions : MonoBehaviour
    {
        /// <summary>
        /// This class represents the saved data of the options class.
        /// </summary>
        [Serializable]
        private class OptionsSavedData : FileContent
        {
            #region CONSTANTS
            private const string VOLUME = "Volume";
            private const string BRIGHTNESS = "Brightness";
            private const string MOUSE_SENSITIVITY = "Mouse Sensitivity";
            private const string INVERT_MOUSE = "Invert Mouse";
            #endregion
            public OptionsSavedData()
            {

            }
            /// <summary>
            /// A constructor used by .Net Serialization to load data
            /// </summary>
            /// <param name="aInfo"></param>
            /// <param name="aContext"></param>
            public OptionsSavedData(SerializationInfo aInfo, StreamingContext aContext) : base (aInfo,aContext)
            {
            }
            /// <summary>
            /// A method used by ISerializable to save data with.
            /// </summary>
            /// <param name="aInfo"></param>
            /// <param name="aContext"></param>
            public override void GetObjectData(SerializationInfo aInfo, StreamingContext aContext)
            {
                base.GetObjectData(aInfo, aContext);
            }
            protected override void OnLoad()
            {
                volume = GetData<float>(VOLUME);
                brightness = GetData<float>(BRIGHTNESS);
                mouseSensitivity = GetData<float>(MOUSE_SENSITIVITY);
                invertMouse = GetData<bool>(INVERT_MOUSE);
            }
            protected override void OnSave()
            {
                AddData(VOLUME, volume);
                AddData(BRIGHTNESS, brightness);
                AddData(MOUSE_SENSITIVITY, mouseSensitivity);
                AddData(INVERT_MOUSE, invertMouse);
            }
        }

        #region SINGLETON
        /// <summary>
        /// A singleton instance of GameOptions
        /// </summary>
        private static GameOptions s_Instance = null;
        /// <summary>
        /// An accessor which creates an instance of GameOptions if it is null
        /// </summary>
        private static GameOptions instance
        {
            get { if (s_Instance == null) { CreateInstance(); } return s_Instance; }
        }
        /// <summary>
        /// Creates an instance of the GameOptions if it was missing.
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
            s_Instance = persistant.GetComponent<GameOptions>();
            if (s_Instance == null)
            {
                s_Instance = persistant.AddComponent<GameOptions>();
            }

        }
        /// <summary>
        /// Sets the instance of the GameOptions to the instance given.
        /// </summary>
        /// <param name="aInstance">The instance to make singleton</param>
        /// <returns></returns>
        private static bool SetInstance(GameOptions aInstance)
        {
            if (s_Instance != null && s_Instance != aInstance)
            {
                return false;
            }
            s_Instance = aInstance;
            return true;
        }
        /// <summary>
        /// Removes the instance of the GameOptions if the instance being destroyed is the the same as the singleton.
        /// </summary>
        /// <param name="aInstance"></param>
        private static void DestroyInstance(GameOptions aInstance)
        {
            if (s_Instance == aInstance)
            {
                s_Instance = null;
            }
        }
        #endregion
        [SerializeField]
        private float m_Brightness = 70.0f;
        [SerializeField]
        private float m_Volume = 80.0f;
        [SerializeField]
        private float m_MouseSensitivity = 1.0f;
        [SerializeField]
        private bool m_InvertMouse = false;
        /// <summary>
        /// Initialze the singleton
        /// </summary>
        private void Start()
        {
            if(!SetInstance(this))
            {
                Destroy(this);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }
        /// <summary>
        /// Destroy the singleton
        /// </summary>
        private void OnDestroy()
        {
            DestroyInstance(this);
        }
        /// <summary>
        /// Verifies the options were loaded in, otherwise defaults are used.
        /// </summary>
        /// <param name="aStream"></param>
        public static void LoadOptions(EndevGame.FileStream aStream)
        {
            EndevGame.File loadFile = aStream.Get(Game.FILE_OPTIONS);
            if(loadFile != null)
            {
                OptionsSavedData loadedData = loadFile.GetFirst<OptionsSavedData>();
                if(loadedData != null)
                {
                    return;
                }
            }
            SetDefaults();
        }
        /// <summary>
        /// Adds a options file as well as a options saved data to get the data.
        /// </summary>
        /// <param name="aStream"></param>
        public static void SaveOptions(EndevGame.FileStream aStream)
        {
            EndevGame.File loadFile = aStream.Get(Game.FILE_OPTIONS);
            if(loadFile == null)
            {
                loadFile = aStream.Add(Game.FILE_OPTIONS);
            }
            loadFile.Clear();
            loadFile.Add(new OptionsSavedData());

        }
        /// <summary>
        /// Sets the defaults of the options.
        /// </summary>
        public static void SetDefaults()
        {
            brightness = 70.0f;
            volume = 80.0f;
            mouseSensitivity = 1.0f;
            invertMouse = false;
        }
        /// <summary>
        /// An accessor for brightness. Expects a 0-100 value.
        /// </summary>
        public static float brightness
        {
            get { return instance.m_Brightness; }
            set { if (value != instance.m_Brightness) { RenderSettings.ambientLight = new Color(value / 100.0f, value / 100.0f, value / 100.0f); } instance.m_Brightness = value; }
        }
        /// <summary>
        /// An accessor for volume. Expects a 0-100 value
        /// </summary>
        public static float volume
        {
            get { return instance.m_Volume; }
            set { if (value != instance.m_Volume) { AudioListener.volume = value / 100.0f; } instance.m_Volume = value; }
        }
        /// <summary>
        /// An accessor for mouse sensitivity. Expects a 0-1 value.
        /// </summary>
        public static float mouseSensitivity
        {
            get { return s_Instance.m_MouseSensitivity; }
            set { s_Instance.m_MouseSensitivity = value; }
        }
        /// <summary>
        /// An accessor for inverting the mouse. 
        /// </summary>
        public static bool invertMouse
        {
            get { return s_Instance.m_InvertMouse; }
            set { s_Instance.m_InvertMouse = value; }
        }

    }
}