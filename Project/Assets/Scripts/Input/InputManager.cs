using UnityEngine;
using System;
using System.Collections.Generic;
using OnLooker;

namespace Gem
{

    //[ExecuteInEditMode]
    public class InputManager : MonoBehaviour
    {
        #region SINGLETON


        private static InputManager s_Instance = null;
        public static InputManager instance
        {
            get { return s_Instance; }
        }
        #endregion

        #region StaticHelpers

        private static void missingInputManager()
        {
            Debug.LogError("Missing \'InputManager\' start from init_scene or attach one to a gameobject.");
        }

        public static float GetAxis(string aName)
        {
            return getAxis(aName, InputPlayer.ANY);
        }
        public static float getAxis(string aName, InputPlayer aPlayer)
        {
            if (instance == null)
            {
                missingInputManager();
                return 0.0f;
            }
            return instance.internal_GetAxis(aName, aPlayer);
        }
        public static bool GetButton(string aName)
        {
            return getButton(aName, InputPlayer.ANY);
        }
        public static bool getButton(string aName, InputPlayer aPlayer)
        {
            if (instance == null)
            {
                missingInputManager();
                return false;
            }
            return instance.internal_GetButton(aName, aPlayer);
        }
        public static bool getButtonDown(string aName)
        {
            return getButtonDown(aName, InputPlayer.ANY);
        }
        public static bool getButtonDown(string aName, InputPlayer aPlayer)
        {
            if (instance == null)
            {
                missingInputManager();
                return false;
            }
            return instance.internal_GetButtonDown(aName, aPlayer);
        }
        public static bool getButtonUp(string aName)
        {
            return getButtonUp(aName, InputPlayer.ANY);
        }
        public static bool getButtonUp(string aName, InputPlayer aPlayer)
        {
            if (instance == null)
            {
                missingInputManager();
                return false;
            }
            return instance.internal_GetButtonUp(aName, aPlayer);
        }
        public static void save(string aUser)
        {
            if (instance == null)
            {
                missingInputManager();
                return;
            }
            instance.internal_SaveUser(aUser);
        }
        public static void load(string aUser)
        {
            if (instance == null)
            {
                missingInputManager();
                return;
            }
            instance.internal_LoadUser(aUser);
        }
        public static void save(FileData aFile)
        {
            if (instance == null)
            {
                missingInputManager();
                return;
            }
            instance.internal_SaveUser(aFile);
        }
        public static void load(FileData aFile)
        {
            if (instance == null)
            {
                missingInputManager();
                return;
            }
            instance.internal_LoadUser(aFile);
        }
        #endregion


        #region Fields


#if UNITY_EDITOR && (UNITY_4_5 || UNITY_5_0)
        [Tooltip("")]
#endif
        [SerializeField]
        private List<InputAxis> m_Axis = new List<InputAxis>();


        #endregion

        // Use this for initialization
        void Start()
        {
            if (s_Instance == null)
            {
                s_Instance = this;
            }
            else
            {
                Debug.LogWarning("Attempting to create more than one \'Input Manager\'");
                Debug.LogWarning("Floating GameObject " + gameObject.name);
                if (Application.isPlaying)
                {
                    Destroy(this);
                }
                else
                {
                    DestroyImmediate(this);
                }
                return;
            }
            DontDestroyOnLoad(gameObject);
            //if (Application.isPlaying == false)
            //{
            //    setDefault();
            //    internal_SaveEditor();
            //}
            if (Application.isPlaying)
            {
                
                for (int i = 0; i < m_Axis.Count; i++)
                {
                    if (m_Axis[i] != null)
                    {
                        m_Axis[i].start();
                    }
                }
            }
        }



        void OnDestroy()
        {
            if (s_Instance == this)
            {
                s_Instance = null;
            }
        }



        // Update is called once per frame
        void Update()
        {
            if (Application.isPlaying)
            {
                for (int i = 0; i < m_Axis.Count; i++)
                {
                    if (m_Axis != null)
                    {
                        m_Axis[i].update();
                    }
                }
            }
        }


        //Set the defaults for input manager.
        public void setDefault()
        {
            clear();
            createInputAxis("Vertical", InputDevice.KEYBOARD, "w", "s");
            createInputAxis("Horizontal", InputDevice.KEYBOARD, "d", "a");
            createInputAxis("Vertical", InputDevice.KEYBOARD, InputUtilities.UP_ARROW, InputUtilities.DOWN_ARROW);
            createInputAxis("Horizontal", InputDevice.KEYBOARD, InputUtilities.RIGHT_ARROW, InputUtilities.LEFT_ARROW);

            createButton("Jump", InputDevice.KEYBOARD, InputUtilities.SPACE);

            createInputAxis("Mouse X", InputDevice.MOUSE, InputUtilities.MOUSE_X, InputUtilities.MOUSE_X);
            createInputAxis("Mouse Y", InputDevice.MOUSE, InputUtilities.MOUSE_Y, InputUtilities.MOUSE_Y);
            createInputAxis("Mouse ScrollWheel", InputDevice.MOUSE, InputUtilities.MOUSE_SCROLL_Y, InputUtilities.MOUSE_SCROLL_Y);
        }
        public void setVerdantStoryDefault()
        {
            
            clear();

            //**********Keyboard and mouse**********
            //**************************************
            createInputAxis("Forward", InputDevice.KEYBOARD, "w", "s");
            createInputAxis("Side", InputDevice.KEYBOARD, "d", "a");
            createInputAxis("Forward", InputDevice.KEYBOARD, InputUtilities.UP_ARROW, InputUtilities.DOWN_ARROW);
            createInputAxis("Side", InputDevice.KEYBOARD, InputUtilities.RIGHT_ARROW, InputUtilities.LEFT_ARROW);

            createButton("Jump", InputDevice.KEYBOARD, InputUtilities.SPACE);

            createInputAxis("Mouse X", InputDevice.MOUSE, InputUtilities.MOUSE_X, InputUtilities.MOUSE_X);
            createInputAxis("Mouse Y", InputDevice.MOUSE, InputUtilities.MOUSE_Y, InputUtilities.MOUSE_Y);
            createInputAxis("Mouse ScrollWheel", InputDevice.MOUSE, InputUtilities.MOUSE_SCROLL_Y, InputUtilities.MOUSE_SCROLL_Y);

            createInputAxis("AmmoSwitch", InputDevice.MOUSE, InputUtilities.MOUSE_SCROLL_Y, InputUtilities.MOUSE_SCROLL_Y);
            
            createButton("Grow", InputDevice.MOUSE, InputUtilities.MOUSE_RIGHT_BUTTON);

            createButton("Shrink", InputDevice.MOUSE, InputUtilities.MOUSE_RIGHT_BUTTON);
            //add modifier

            createButton("Sprint", InputDevice.KEYBOARD, "leftshift");

            createButton("Use", InputDevice.KEYBOARD, "e");

            createButton("Crouch", InputDevice.KEYBOARD, "c");

            createButton("Shoot", InputDevice.MOUSE, InputUtilities.MOUSE_LEFT_BUTTON);

            createButton("ShootMode", InputDevice.KEYBOARD, "tab");

            createButton("AimMode", InputDevice.MOUSE, InputUtilities.MOUSE_RIGHT_BUTTON);
            //**************************************
            //**************************************

            //**********  360 Controller  **********
            //**************************************
            createInputAxis("Forward", InputDevice.XBOX_CONTROLLER, InputUtilities.LEFT_STICK_Y, InputUtilities.LEFT_STICK_Y);
            createInputAxis("Side", InputDevice.XBOX_CONTROLLER, InputUtilities.LEFT_STICK_X, InputUtilities.LEFT_STICK_X);

            createButton("Jump", InputDevice.XBOX_CONTROLLER, InputUtilities.A);

            createInputAxis("Mouse X", InputDevice.XBOX_CONTROLLER, InputUtilities.RIGHT_STICK_X, InputUtilities.RIGHT_STICK_X);
            createInputAxis("Mouse Y", InputDevice.XBOX_CONTROLLER, InputUtilities.RIGHT_STICK_Y, InputUtilities.RIGHT_STICK_Y);
            createInputAxis("Mouse ScrollWheel", InputDevice.XBOX_CONTROLLER, InputUtilities.D_PAD_Y, InputUtilities.D_PAD_Y);

            createInputAxis("AmmoSwitch", InputDevice.XBOX_CONTROLLER, InputUtilities.D_PAD_X, InputUtilities.D_PAD_X);

            createButton("Grow", InputDevice.XBOX_CONTROLLER, InputUtilities.RIGHT_SHOULDER);

            createButton("Shrink", InputDevice.XBOX_CONTROLLER, InputUtilities.LEFT_SHOULDER);

            createButton("Sprint", InputDevice.XBOX_CONTROLLER, InputUtilities.B);

            createButton("Use", InputDevice.XBOX_CONTROLLER, InputUtilities.X);

            createButton("Crouch", InputDevice.XBOX_CONTROLLER, InputUtilities.LEFT_STICK_IN);

            createInputAxis("Shoot", InputDevice.XBOX_CONTROLLER, InputUtilities.RIGHT_TRIGGER, InputUtilities.RIGHT_TRIGGER);

            createButton("ShootMode", InputDevice.XBOX_CONTROLLER, InputUtilities.Y);

            createInputAxis("AimMode", InputDevice.XBOX_CONTROLLER, InputUtilities.LEFT_TRIGGER, InputUtilities.LEFT_TRIGGER);
            //**************************************
            //**************************************

            //See create functions below but also note these two variables not shown below that are useful for changing input axis value.
            //InputAxis.speed - Sets the speed at which the input axis value changes at. 
            //InputAxis.resetOnRelease - Resets the axis value to 0 on release of input.

        }

        public void createInputAxis(string aName)
        {
            InputAxis axis = new InputAxis(aName);
            m_Axis.Add(axis);
        }
        private void createInputAxis(string aName, InputDevice aDevice, string aPositiveInput)
        {
            InputAxis axis = new InputAxis(aName);
            axis.deviceType = aDevice;
            axis.player = InputPlayer.ANY;
            axis.setPositiveKey(aPositiveInput, KeyCode.None);
            if (axis.validatePositiveKey() == false)
            {
                Debug.LogWarning("Failed to validate InputKey. InputDevice: " + aDevice + "Positive Input:" + aPositiveInput);
            }
            m_Axis.Add(axis);
        }
        private void createInputAxis(string aName, InputDevice aDevice, string aPositiveInput, string aNegativeInput)
        {
            InputAxis axis = new InputAxis(aName);
            axis.deviceType = aDevice;
            axis.player = InputPlayer.ANY;
            axis.setPositiveKey(aPositiveInput, KeyCode.None);
            axis.setNegativeKey(aNegativeInput, KeyCode.None);
            axis.resetOnRelease = true;
            if (axis.validatePositiveKey() == false)
            {
                Debug.LogWarning("Failed to validate InputKey. InputDevice: " + aDevice + ". Positive Input:" + aPositiveInput + ". Negative Input:" + aNegativeInput);
            }
            m_Axis.Add(axis);
        }
        private void createButton(string aName, InputDevice aDevice, string aPositiveInput)
        {
            InputAxis axis = new InputAxis(aName);
            axis.deviceType = aDevice;
            axis.player = InputPlayer.ANY;
            axis.setPositiveKey(aPositiveInput, KeyCode.None);
            axis.resetOnRelease = true;
            if (axis.validatePositiveKey() == false)
            {
                Debug.LogWarning("Failed to validate InputKey. InputDevice: " + aDevice + "Positive Input:" + aPositiveInput);
            }
            m_Axis.Add(axis);
        }

        public void deleteInputAxis(InputAxis aAxis)
        {
            if (aAxis != null)
            {
                m_Axis.Remove(aAxis);
            }
        }
        public void clear()
        {
            m_Axis.Clear();
        }
        public void hideAll()
        {
            for (int i = 0; i < m_Axis.Count; i++)
            {
                m_Axis[i].foldOut = false;
            }
        }
        public void showAll()
        {
            for (int i = 0; i < m_Axis.Count; i++)
            {
                m_Axis[i].foldOut = true;
            }
        }

        private float internal_GetAxis(string aName, InputPlayer aPlayer)
        {
            float value = 0.0f;
            int hits = 0;

            for (int i = 0; i < m_Axis.Count; i++)
            {
                if (m_Axis[i] == null)
                {
                    continue;
                }


                if (m_Axis[i].name == aName && (aPlayer == InputPlayer.ANY || aPlayer == m_Axis[i].player))
                {
                    value += m_Axis[i].currentValue;
                    hits++;
                }
            }

            if (hits == 0)
            {
                Debug.LogWarning("Axis \'" + aName + "\' not setup");
                return 0.0f;
            }
            return Mathf.Clamp(value, -1.0f, 1.0f);
        }

        private bool internal_GetButton(string aName, InputPlayer aPlayer)
        {
            int hits = 0;
            bool exists = false;
            for (int i = 0; i < m_Axis.Count; i++)
            {
                if (m_Axis[i] == null)
                {
                    continue;
                }
                if (m_Axis[i].name == aName && (aPlayer == InputPlayer.ANY || aPlayer == m_Axis[i].player))
                {
                    if (m_Axis[i].button)
                    {
                        hits++;
                    }
                    exists = true;
                }
            }

            if (exists == false)
            {
                Debug.LogWarning("Axis \'" + aName + "\' not setup");
                return false;
            }
            return hits > 0;
        }
        private bool internal_GetButtonDown(string aName, InputPlayer aPlayer)
        {
            int hits = 0;
            bool exists = false;
            for (int i = 0; i < m_Axis.Count; i++)
            {
                if (m_Axis[i] == null)
                {
                    continue;
                }
                if (m_Axis[i].name == aName && (aPlayer == InputPlayer.ANY || aPlayer == m_Axis[i].player))
                {
                    if (m_Axis[i].buttonDown)
                    {
                        hits++;
                    }
                    exists = true;
                }
            }

            if (exists == false)
            {
                Debug.LogWarning("Axis \'" + aName + "\' not setup");
                return false;
            }
            return hits > 0;
        }
        private bool internal_GetButtonUp(string aName, InputPlayer aPlayer)
        {
            int hits = 0;
            bool exists = false;
            for (int i = 0; i < m_Axis.Count; i++)
            {
                if (m_Axis[i] == null)
                {
                    continue;
                }
                if (m_Axis[i].name == aName && (aPlayer == InputPlayer.ANY || aPlayer == m_Axis[i].player))
                {
                    if (m_Axis[i].buttonUp)
                    {
                        hits++;
                    }
                    exists = true;
                }
            }

            if (exists == false)
            {
                Debug.LogWarning("Axis \'" + aName + "\' not setup");
                return false;
            }
            return hits > 0;
        }

        public void saveEditor()
        {
            internal_SaveEditor();
        }
        public void loadEditor()
        {
            internal_LoadEditor();
        }

        private void internal_SaveEditor()
        {
            
#if UNITY_WEBPLAYER
            List<InputAxis>.Enumerator iterator = m_Axis.GetEnumerator();
            while(iterator.MoveNext())
            {
                if(iterator.Current != null)
                {
                    iterator.Current.save();
                }
            }
#else
            FileData file = new FileData("Input_Editor");
            for (int i = 0; i < m_Axis.Count; i++)
            {
                file.add(m_Axis[i], m_Axis[i].name);
            }
            file.save();
#endif
        }
        private void internal_LoadEditor()
        {
            
#if UNITY_WEBPLAYER
            setVerdantStoryDefault();
            List<InputAxis>.Enumerator iterator = m_Axis.GetEnumerator();
            while (iterator.MoveNext())
            {
                if (iterator.Current != null)
                {
                    iterator.Current.load();
                }
            }
#else
            FileData file = new FileData("Input_Editor");
            file.load();
            InputAxis[] loadedAxis = file.get<InputAxis>();
            if (loadedAxis != null)
            {
                Debug.Log("Clearing to load editor");
                clear();
                for (int i = 0; i < loadedAxis.Length; i++)
                {
                    m_Axis.Add(loadedAxis[i]);
                }
            }
#endif
        }
        private void internal_SaveUser(string aUser)
        {
#if UNITY_WEBPLAYER
            List<InputAxis>.Enumerator iterator = m_Axis.GetEnumerator();
            while(iterator.MoveNext())
            {
                if(iterator.Current != null)
                {
                    iterator.Current.save();
                }
            }
#else
            FileData file = new FileData("Input_" + aUser);
            for (int i = 0; i < m_Axis.Count; i++)
            {
                file.add(m_Axis[i], m_Axis[i].name);
            }
            file.save();
#endif
        }
        private void internal_LoadUser(string aUser)
        {
#if UNITY_WEBPLAYER
            setVerdantStoryDefault();
            List<InputAxis>.Enumerator iterator = m_Axis.GetEnumerator();
            while (iterator.MoveNext())
            {
                if (iterator.Current != null)
                {
                    iterator.Current.load();
                }
            }
#else
            FileData file = new FileData("Input_" + aUser);
            file.load();
            InputAxis[] loadedAxis = file.get<InputAxis>();
            if (loadedAxis != null)
            {
                clear();
                for (int i = 0; i < loadedAxis.Length; i++)
                {
                    m_Axis.Add(loadedAxis[i]);
                }
            }
#endif
        }
        private void internal_SaveUser(FileData aData)
        {
#if UNITY_WEBPLAYER
            List<InputAxis>.Enumerator iterator = m_Axis.GetEnumerator();
            while(iterator.MoveNext())
            {
                if(iterator.Current != null)
                {
                    iterator.Current.save();
                }
            }
#else
            if (aData != null)
            {
                for (int i = 0; i < m_Axis.Count; i++)
                {
                    aData.add(m_Axis[i], m_Axis[i].name);
                }
            }
#endif
        }
        private void internal_LoadUser(FileData aData)
        {
#if UNITY_WEBPLAYER
            setVerdantStoryDefault();
            List<InputAxis>.Enumerator iterator = m_Axis.GetEnumerator();
            while (iterator.MoveNext())
            {
                if (iterator.Current != null)
                {
                    iterator.Current.load();
                }
            }
#else
            if (aData == null)
            {
                return;
            }
            InputAxis[] loadedAxis = aData.get<InputAxis>();
            if (loadedAxis != null)
            {
                clear();
                for (int i = 0; i < loadedAxis.Length; i++)
                {
                    m_Axis.Add(loadedAxis[i]);
                }
            }
#endif
        }

        public List<InputAxis> axisList
        {
            get { return m_Axis; }
        }
    }

}