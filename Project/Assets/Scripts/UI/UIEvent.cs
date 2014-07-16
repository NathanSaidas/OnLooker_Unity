using UnityEngine;
using System.Collections;

namespace OnLooker
{
    namespace UI
    {
        //public delegate void UIEvent(UIToggle aSender, UIEventArgs aArgs);

        public enum UIEventType
        {
            ENTER,
            EXIT,
            CLICK,
            DOUBLE_CLICK,
            DOWN,
            RELEASE,
            HOVER,
            FOCUS,
            UNFOCUS,
            KEY_PRESS
        }

        public class UIEventArgs
        {
            //Fields
            private UIEventType m_EventType = UIEventType.CLICK;
            private MouseButton m_MouseButton = MouseButton.NONE;
            private KeyCode m_KeyCode = KeyCode.None;


            //Constructors
            public UIEventArgs(UIEventType aEventType)
            {
                m_EventType = aEventType;
            }

            public UIEventArgs(UIEventType aEventType, MouseButton aMouseButton)
            {
                m_EventType = aEventType;
                m_MouseButton = aMouseButton;
            }

            public UIEventArgs(UIEventType aEventType, MouseButton aMouseButton, KeyCode aKeyCode)
            {
                m_EventType = aEventType;
                m_MouseButton = aMouseButton;
                m_KeyCode = aKeyCode;
            }

            //Properties
            public UIEventType eventType { get { return m_EventType; } }
            public MouseButton mouseButton { get { return m_MouseButton; } }
            public KeyCode keyCode { get { return m_KeyCode; } }
        }

    }
}
