using UnityEngine;
using System;
using System.Collections.Generic;

namespace Gem
{
    #region Change Log
    /* October,13,2014 - Nathan Hanlan, Added support for left and right triggers in the readonly state property.
     * 
     */
    #endregion
    [Serializable]
    public class InputKey : EndevGame.Object, IDebugWatch
    {
        /// <summary>
        /// Defines the three types of input.
        /// </summary>
        private enum InputType
        {
            AXIS,
            KEYCODE,
            MOUSE_BUTTON
        }
        #region Constructors
        public InputKey()
        {

        }
        public InputKey(InputAxis aOwner)
        {
            owner = aOwner;
        }
        public InputKey(InputAxis aOwner, bool aPositive)
        {
            owner = aOwner;
            m_PositiveKey = aPositive;
        }
        #endregion

        #region Fields
        /// <summary>
        /// Defines the InputType in a string format for this InputKey
        /// </summary>

        [SerializeField]
        private string m_Input = string.Empty;


        /// <summary>
        /// Defines the input type for this InputKey
        /// </summary>
        [SerializeField]

        private InputType m_InputType = InputType.KEYCODE;

        /// <summary>
        /// The keycode value stored in an integer format to be used with MouseButton / KeyCodes enums
        /// </summary>
        [SerializeField]
        private int m_KeyCode = 0;


        /// <summary>
        /// The name of the axis to be looked up
        /// </summary>
        [SerializeField]
        private string m_AxisName = string.Empty;

        /// <summary>
        /// If positive key is true the InputKey increments on updates and decrements on updates where it is false
        /// </summary>
        [SerializeField]
        private bool m_PositiveKey = true;

        /// <summary>
        /// The InputAxis responsible for the InputKey
        /// </summary>
        
        //private InputAxis m_Owner = null;


        /// <summary>
        /// The modifier keycode to be used.
        /// </summary>
        [SerializeField]
        private KeyCode m_Modifier = KeyCode.None;

        [SerializeField]
        private bool m_IsValid = false;

        #endregion

        #region Methods

        public override  void start()
        {
            if(owner != null)
            {
                //if(owner.axisName == "Grow")
                //{
                //    DebugUtils.addWatch(this);
                //}
            }
        }


        public void onReport()
        {
            //DebugUtils.drawWatch("InputKey", "InputType", m_InputType.ToString());
            //DebugUtils.drawWatch("InputKey", "Keycode", m_KeyCode);
        }

        /// <summary>
        /// Invoked to set the mouse button and specify the type of input this InputKey keeps track of
        /// </summary>
        /// <param name="aButton"></param>
        public void setAsMouseButton(MouseButton aButton)
        {
            m_InputType = InputType.MOUSE_BUTTON;
            m_KeyCode = (int)aButton;
        }
        /// <summary>
        /// Invoked to set the keycode and specify the type of input this InputKey keeps track of
        /// </summary>
        /// <param name="aKeyCode"></param>
        public void setAsKeyCode(KeyCode aKeyCode)
        {
            m_InputType = InputType.KEYCODE;
            m_KeyCode = (int)aKeyCode;
        }
        /// <summary>
        /// Invoked to set the axis and specify the type of input this InputKey keeps track of
        /// </summary>
        /// <param name="aAxis"></param>
        public void setAsAxis(string aAxis)
        {
            m_InputType = InputType.AXIS;
            m_AxisName = aAxis;
        }

        public bool validateInput()
        {
            if (InputUtilities.parseInputString(this))
            {
                m_IsValid = true;
                return true;
            }
            m_IsValid = false;
            return false;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Get the state of InputKey. 
        /// Returns 1 for positive state, -1 for negativeState where keycode/mousebutton is triggered
        /// Returns -1 to 1 for axis is triggered
        /// Returns 0 for keyCode/mouseButton/axis is not triggered or there was an error.
        /// </summary>
        public float state
        {
            get
            {
                if (owner == null)
                {
                    Debug.LogError("InputKey is missing an \'InputAxis\'");
                    return 0.0f;
                }
                if (m_IsValid == false)
                {
                    //Debug.LogWarning("InputKey " + name + " does not have a valid input setup.");
                    return 0.0f;
                }
                //Check for a modifier if there is one. If its not pressed might as well exit early.
                if (m_Modifier != KeyCode.None && Input.GetKey(m_Modifier) == false)
                {
                    return 0.0f;
                }
                //Check Mouse Button
                if (m_InputType == InputType.MOUSE_BUTTON)
                {
                    if (Input.GetMouseButton(m_KeyCode))
                    {
                        //Debug.Log("Mouse Button Pressed");
                        if (positiveKey == true)
                        {
                            return 1.0f;
                        }
                        else
                        {
                            return -1.0f;
                        }
                    }
                    else
                    {
                        return 0.0f;
                    }
                }
                //Check KeyCode
                else if (m_InputType == InputType.KEYCODE)
                {

                    if (Input.GetKey((KeyCode)m_KeyCode))
                    {
                        if (positiveKey == true)
                        {
                            return 1.0f;
                        }
                        else
                        {
                            return -1.0f;
                        }
                    }
                    else
                    {
                        return 0.0f;
                    }
                }
                //Check Axis
                else if (m_InputType == InputType.AXIS)
                {
                    float inputValue = 0.0f;

                    switch (owner.player)
                    {
                            
                        case InputPlayer.ANY:
                            inputValue = Input.GetAxis(m_AxisName + "_0");
                            break;
                        case InputPlayer.PLAYER_1:
                            inputValue = Input.GetAxis(m_AxisName + "_1");
                            break;
                        case InputPlayer.PLAYER_2:
                            inputValue = Input.GetAxis(m_AxisName + "_2");
                            break;
                        case InputPlayer.PLAYER_3:
                            inputValue = Input.GetAxis(m_AxisName + "_3");
                            break;
                        case InputPlayer.PLAYER_4:
                            inputValue = Input.GetAxis(m_AxisName + "_4");
                            break;
                        default:

                            break;
                    }

                    if(m_AxisName == InputUtilities.LEFT_TRIGGER)
                    {
                        return Mathf.Clamp(inputValue, 0.0f, 1.0f);
                    }
                    else if (m_AxisName == InputUtilities.RIGHT_TRIGGER)
                    {
                        return Mathf.Abs(Mathf.Clamp(inputValue, -1.0f, 0.0f));
                    }

                    return inputValue;

                }
                return 0.0f;
            }
        }

        public override string name
        {
            get { return owner.name; }
            set { ;}
        }
        /// <summary>
        /// Returns true where the input type is axis
        /// </summary>
        public bool isAxis
        {
            get { return m_InputType == InputType.AXIS; }
        }
        /// <summary>
        /// Returns true where the input type is a keycode
        /// </summary>
        public bool isKeyCode
        {
            get { return m_InputType == InputType.KEYCODE; }
        }
        /// <summary>
        /// Returns true where the input type is a mouseButton
        /// </summary>
        public bool isMouseButton
        {
            get { return m_InputType == InputType.MOUSE_BUTTON; }
        }

        /// <summary>
        /// Get and Set the input string which is then processed 
        /// </summary>
        public string input
        {
            get { return m_Input; }
            set { m_Input = value; }
        }
        /// <summary>
        /// Returns the name of the axis, should this be an axis code.
        /// </summary>
        public string axisName
        {
            get { return m_AxisName; }
        }
        /// <summary>
        /// Returns the KeyCode this Inputkey tracks. Should this not be a keycode the value returned is KeyCode.None
        /// </summary>
        public KeyCode keyCode
        {
            get { return isKeyCode ? (KeyCode)m_KeyCode : KeyCode.None; }
        }
        /// <summary>
        /// Returns the MouseButton this InputKey tracks. Should this not be a mouse button the value returned is MouseButton.NONE
        /// </summary>
        public MouseButton mouseButton
        {
            get { return isMouseButton ? (MouseButton)m_KeyCode : MouseButton.NONE; }
        }
        /// <summary>
        /// Get and Set the modifier key used to check the state of the InputKey
        /// </summary>
        public KeyCode modifier
        {
            get { return m_Modifier; }
            set { m_Modifier = value; }
        }
        /// <summary>
        /// Get and Set the positiveKey flag to determine increment(positive) or decrement(negative)
        /// </summary>
        public bool positiveKey
        {
            get { return m_PositiveKey; }
            set { m_PositiveKey = value; }
        }
        public InputAxis owner
        {
            get; //{ return m_Owner; }
            set;//{ if (m_Owner == null) { m_Owner = value; } }
        }
        #endregion


    }
}