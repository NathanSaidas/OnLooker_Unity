using UnityEngine;
using System;
using System.Collections;
using System.Runtime.Serialization;
using OnLooker;


namespace Gem
{
    #region
    /* October,7,2014 Nathan Hanlan, Adding player prefs support.
    */
    #endregion

    /// <summary>
    /// This class represents the an input axis which holds a value from -1 to 1. This is used to determine input states.
    /// </summary>
    [Serializable]
    public class InputAxis : CustomSaveData, IDebugWatch
    {
        #region Constructor
        public InputAxis()
        {
            m_PositiveKey = new InputKey(this, true);
            m_NegativeKey = new InputKey(this, false);
        }
        public InputAxis(string aName)
        {
            m_PositiveKey = new InputKey(this, true);
            m_NegativeKey = new InputKey(this, false);
            axisName = aName;
        }
        public InputAxis(SerializationInfo aInfo, StreamingContext aContext)
            : base(aInfo, aContext)
        {

        }
        #endregion

        #region Fields
        /// <summary>
        /// The Last value of the input axis
        /// </summary>
        private float m_LastValue = 0.0f;

        /// <summary>
        /// The Current Value of the input axis
        /// </summary>
        private float m_CurrentValue = 0.0f;

        /// <summary>
        /// The speed at which the input axis increases / decreases 
        /// </summary>
        [SerializeField]
        private float m_Speed = 3.0f;

        /// <summary>
        /// Whether or not to snap back 0 upon positive / negative key state switch. This should be set to true if the axis is intended to be used as like a button.
        /// </summary>
        [SerializeField]
        private bool m_ResetOnRelease = false;

        /// <summary>
        /// The device type this input axis is targeting
        /// </summary>
        [SerializeField]
        private InputDevice m_DeviceType;

        /// <summary>
        /// The player this input axis is targeting
        /// </summary>
        [SerializeField]
        private InputPlayer m_Player;

        /// <summary>
        /// The positive key of the input axis
        /// </summary>
        //[HideInInspector]
        [SerializeField]
        InputKey m_PositiveKey = null;

        /// <summary>
        /// The negative key of the input axis
        /// </summary>
        //[HideInInspector]
        [SerializeField]
        InputKey m_NegativeKey = null;

        /// <summary>
        /// A variable for the editor to use to show / hide the input axis variables inside the inspector or window
        /// </summary>
        //[HideInInspector]
        [SerializeField]
        private bool m_FoldOut = true;
        #endregion

        #region Methods
        public void start()
        {
            m_PositiveKey.owner = this;
            m_NegativeKey.owner = this;
            validatePositiveKey();
            validateNegativeKey();

            m_PositiveKey.start();
            m_NegativeKey.start();
        }

        public void onReport()
        {
            //DebugUtils.drawWatch("InputAxis", "Current Value", m_CurrentValue);
        }

        /// <summary>
        /// Updates the InputAxis changing the current value based on the positive and negative input key states.
        /// </summary>
        public void update()
        {
            bool isPos = false;
            bool isNeg = false;

            InputKey inputKey = null;
            //Check the positive key state
            if (m_PositiveKey != null && m_PositiveKey.state > 0.0f)
            {
                isPos = true;
                //Get the actual axis state if the key is an axis
                if (m_PositiveKey.isAxis && inputKey == null)
                {
                    inputKey = m_PositiveKey;
                }
            }
            if (m_NegativeKey != null && m_NegativeKey.state < 0.0f)
            {
                isNeg = true;
                //Get the actual axis state if the key is an axis
                if (m_NegativeKey.isAxis && inputKey == null)
                {
                    inputKey = m_NegativeKey;
                }
            }

            m_LastValue = m_CurrentValue;

            //Move to zero without input
            if (isPos == false && isNeg == false)
            {
                moveToZero(false);
            }
            //Move to zero with input from both sides
            else if (isPos == true && isNeg == true)
            {
                moveToZero(true);
            }
            //Set Actual value
            else if (inputKey != null)
            {
                m_CurrentValue = inputKey.state;
            }
            else
            {
                //Move to left or right depending on pos / neg
                if (isPos)
                {
                    m_CurrentValue = Mathf.Clamp(m_CurrentValue + m_Speed * Time.deltaTime, -1.0f, 1.0f);
                }
                else if (isNeg)
                {
                    m_CurrentValue = Mathf.Clamp(m_CurrentValue - m_Speed * Time.deltaTime, -1.0f, 1.0f);
                }
            }
        }
        
        /// <summary>
        /// Moves the current value to 0
        /// </summary>
        /// <param name="aHasInput">Sets the current value to 0 where there is no input and is reseting on release</param>
        public void moveToZero(bool aHasInput)
        {
            if (m_CurrentValue < 0.0f)
            {
                m_CurrentValue = Mathf.Clamp(m_CurrentValue + m_Speed * Time.deltaTime, -1.0f, 0.0f);
            }
            else if (m_CurrentValue > 0.0f)
            {
                m_CurrentValue = Mathf.Clamp(m_CurrentValue - m_Speed * Time.deltaTime, 0.0f, 1.0f);
            }

            if (aHasInput == false && m_ResetOnRelease)
            {
                m_CurrentValue = 0.0f;
                return;
            }
        }

        /// <summary>
        /// Set the positive key variables
        /// </summary>
        /// <param name="aInput"></param>
        /// <param name="aModifier"></param>
        public void setPositiveKey(string aInput, KeyCode aModifier)
        {
            m_PositiveKey.input = aInput;
            m_PositiveKey.modifier = aModifier;
        }

        /// <summary>
        /// Set the negative key variables
        /// </summary>
        /// <param name="aInput"></param>
        /// <param name="aModifier"></param>
        public void setNegativeKey(string aInput, KeyCode aModifier)
        {
            m_NegativeKey.input = aInput;
            m_NegativeKey.modifier = aModifier;
        }

        /// <summary>
        /// Validate the positive key
        /// </summary>
        /// <returns></returns>
        public bool validatePositiveKey()
        {
            return m_PositiveKey.validateInput();
        }

        /// <summary>
        /// Validate the negative key
        /// </summary>
        /// <returns></returns>
        public bool validateNegativeKey()
        {
            return m_NegativeKey.validateInput();
        }

        /// <summary>
        /// Get the positive key variables
        /// </summary>
        /// <param name="aInput"></param>
        /// <param name="aModifier"></param>
        public void getPositiveKey(out string aInput, out KeyCode aModifier)
        {
            aInput = m_PositiveKey.input;
            aModifier = m_PositiveKey.modifier;
        }

        /// <summary>
        /// Get the negative key variables.
        /// </summary>
        /// <param name="aInput"></param>
        /// <param name="aModifier"></param>
        public void getNegativeKey(out string aInput, out KeyCode aModifier)
        {
            aInput = m_NegativeKey.input;
            aModifier = m_NegativeKey.modifier;
        }

        protected override void onSave()
        {
            
#if UNITY_WEBPLAYER
            PlayerPrefs.SetString(axisName + "_AxisName", axisName);
            PlayerPrefs.SetFloat(axisName + "_Speed", m_Speed);
            PlayerPrefs.SetInt(axisName + "_Reset", Convert.ToInt32(m_ResetOnRelease));
            PlayerPrefs.SetInt(axisName + "_DeviceType", (int)m_DeviceType);
            PlayerPrefs.SetInt(axisName + "_Player", (int)m_Player);
            PlayerPrefs.SetString(axisName + "_p_input", m_PositiveKey.input);
            PlayerPrefs.SetInt(axisName + "_p_modifier", (int)m_PositiveKey.modifier);
            PlayerPrefs.SetString(axisName + "n_input", m_NegativeKey.input);
            PlayerPrefs.SetInt(axisName + "_n_modifier", (int)m_NegativeKey.modifier);
#else
            addData("AxisName", axisName);
            addData("Speed", m_Speed);
            addData("Reset", m_ResetOnRelease);
            addData("DeviceType", (int)m_DeviceType);
            addData("Player", (int)m_Player);

            addData("p_input", m_PositiveKey.input);
            addData("p_modifier", (int)m_PositiveKey.modifier);

            addData("n_input", m_NegativeKey.input);
            addData("n_modifier", (int)m_NegativeKey.modifier);
#endif
        }
        protected override void onLoad()
        {
            m_PositiveKey = new InputKey(this, true);
            m_NegativeKey = new InputKey(this, false);
#if UNITY_WEBPLAYER
#if UNITY_EDITOR
            if (PlayerPrefs.HasKey(axisName + "_AxisName") == false)
            {
                Debug.LogWarning("Missing key " + axisName + "_AxisName");
            }
            if (PlayerPrefs.HasKey(axisName + "_Speed") == false)
            {
                Debug.LogWarning("Missing key " + axisName + "_Speed");
            }
            if (PlayerPrefs.HasKey(axisName + "_Reset") == false)
            {
                Debug.LogWarning("Missing key " + axisName + "_Reset");
            }
            if (PlayerPrefs.HasKey(axisName + "_DeviceType") == false)
            {
                Debug.LogWarning("Missing key " + axisName + "_DeviceType");
            }
            if (PlayerPrefs.HasKey(axisName + "_Player") == false)
            {
                Debug.LogWarning("Missing key " + axisName + "_Player");
            }
            if (PlayerPrefs.HasKey(axisName + "_p_input") == false)
            {
                Debug.LogWarning("Missing key " + axisName + "_p_input");
            }
            if (PlayerPrefs.HasKey(axisName + "_p_modifier") == false)
            {
                Debug.LogWarning("Missing key " + axisName + "_p_modifier");
            }
            if (PlayerPrefs.HasKey(axisName + "_n_input") == false)
            {
                Debug.LogWarning("Missing key " + axisName + "_n_input");
            }
            if (PlayerPrefs.HasKey(axisName + "_n_modifier") == false)
            {
                Debug.LogWarning("Missing key " + axisName + "_n_modifier");
            }
#endif
            axisName = PlayerPrefs.GetString(axisName + "_AxisName");
            m_Speed = PlayerPrefs.GetFloat(axisName + "_Speed");
            m_ResetOnRelease = Convert.ToBoolean(PlayerPrefs.GetInt(axisName + "_Reset"));
            m_DeviceType = (InputDevice)PlayerPrefs.GetInt(axisName + "_DeviceType");
            m_Player = (InputPlayer)PlayerPrefs.GetInt(axisName + "_Player");

            m_PositiveKey.input = PlayerPrefs.GetString(axisName + "_p_input");
            m_PositiveKey.modifier = (KeyCode)PlayerPrefs.GetInt(axisName + "_p_modifier");

            m_NegativeKey.input = PlayerPrefs.GetString(axisName + "_n_input");
            m_NegativeKey.modifier = (KeyCode)PlayerPrefs.GetInt(axisName + "_n_modifier");
#else
            axisName = getData<string>("AxisName");
            m_Speed = getData<float>("Speed");
            m_ResetOnRelease = getData<bool>("Reset");
            m_DeviceType = (InputDevice)getData<int>("DeviceType");
            m_Player = (InputPlayer)getData<int>("Player");

            m_PositiveKey.input = getData<string>("p_input");
            m_PositiveKey.modifier = (KeyCode)getData<int>("p_modifier");

            m_NegativeKey.input = getData<string>("n_input");
            m_NegativeKey.modifier = (KeyCode)getData<int>("n_modifier");
#endif
        }
        #endregion

        #region Properties

        /// <summary>
        /// Returns true if the button is down, false if up.
        /// </summary>
        public bool button
        {
            get { return m_CurrentValue != 0.0f; }
        }

        /// <summary>
        /// Returns true if the button is down (First Frame).
        /// </summary>
        public bool buttonDown
        {
            get { return m_LastValue == 0.0f && m_CurrentValue != 0.0f; }
        }

        /// <summary>
        /// Returns true if the button is up (First Frame).
        /// </summary>
        public bool buttonUp
        {
            get { return m_LastValue != 0.0f && m_CurrentValue == 0.0f; }
        }

        /// <summary>
        /// The current value of the axis.
        /// </summary>
        public float currentValue
        {
            get { return m_CurrentValue; }
        }

        /// <summary>
        /// The last value of the axis.
        /// </summary>
        public float lastValue
        {
            get { return m_LastValue; }
        }

        /// <summary>
        /// The delta value of the axis (always positive).
        /// </summary>
        public float delta
        {
            get { return Mathf.Abs(m_CurrentValue - m_LastValue); }
        }

        /// <summary>
        /// Returns the name of the axis
        /// </summary>
        public string axisName
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Whether or not to snap back 0 upon positive / negative key state switch. This should be set to true if the axis is intended to be used as like a button.
        /// </summary>
        public bool resetOnRelease
        {
            get { return m_ResetOnRelease; }
            set { m_ResetOnRelease = value; }
        }

        /// <summary>
        /// The speed to move from 0 to 1 or -1.
        /// </summary>
        public float speed
        {
            get { return m_Speed; }
            set { m_Speed = value; }
        }

        /// <summary>
        /// A boolean for the editor to fold out and show the properties of the input axis or not.
        /// </summary>
        public bool foldOut
        {
            get { return m_FoldOut; }
            set { m_FoldOut = value; }
        }

        /// <summary>
        /// Returns the target device type this axis searches for.
        /// </summary>
        public InputDevice deviceType
        {
            get { return m_DeviceType; }
            set { m_DeviceType = value; }
        }

        /// <summary>
        /// Returns the player this axis searches for.
        /// </summary>
        public InputPlayer player
        {
            get { return m_Player; }
            set { m_Player = value; }
        }
        #endregion
    }
}