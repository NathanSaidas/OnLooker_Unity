using UnityEngine;
using System;
using System.Collections.Generic;

namespace OnLooker
{
    namespace UI
    {
        [ExecuteInEditMode()]
        [Serializable()]
        public class UIToggle : MonoBehaviour
        {
            //Whether or not to show debug information
            [SerializeField()]
            //[HideInInspector()]
            private bool m_Debug = false;


            //A reference to the UI Manager
            [SerializeField()]
            //[HideInInspector()]
            protected UIManager m_Manager = null;


            //The name of the toggle - For UIManager
            [SerializeField()]
            //[HideInInspector()]
            protected string m_ToggleName = string.Empty;
            protected bool m_MouseInBounds = false;
            protected bool m_IsFocused = false;

            [SerializeField()]
            //[HideInInspector()]
            private bool m_Interactive = false;

            [SerializeField()]
            //[HideInInspector()]
            protected bool m_TrapDoubleClick = false;
            protected float m_LastClick = 0.0f;

            //private UIHandler m_Handler = null;
            private event UIEvent m_UIEvent;

            public bool debug
            {
                get { return m_Debug; }
                set { m_Debug = value; }
            }
            public UIManager manager
            {
                get { return m_Manager; }
            }
            public string toggleName
            {
                get { return m_ToggleName; }
                set { m_ToggleName = value; }
            }
            public bool mouseInBounds
            {
                get { return m_MouseInBounds; }
            }
            public bool isFocused
            {
                get { return m_IsFocused; }
            }
            public bool isInteractive
            {
                get { return m_Interactive; }
                set { m_Interactive = value; }
            }
            public bool trapDoubleClick
            {
                get { return m_TrapDoubleClick; }
                set { m_TrapDoubleClick = value; }
            }
            public float lastClick
            {
                get { return m_LastClick; }
            }

            public void registerEvent(UIEvent aCallback)
            {
                if (aCallback != null)
                {
                    m_UIEvent += aCallback;
                }
            }
            public void unregisterEvent(UIEvent aCallback)
            {
                if (aCallback != null)
                {
                    m_UIEvent -= aCallback;
                }
            }

            public void processEvents()
            {
                bool action = false;
                if (Input.GetMouseButtonDown((int)MouseButton.LEFT))
                {
                    bool doubleClick = false;
                    float delta = Time.time - m_LastClick;
                    if (delta < UIManager.DOUBLE_CLICK_TIME)
                    {
                        doubleClick = true;
                    }

                    if (doubleClick == true)
                    {
                        onMouseDoubleClick(MouseButton.LEFT);
                        if (trapDoubleClick == false)
                        {
                            onMouseClick(MouseButton.LEFT);
                        }
                    }
                    else
                    {
                        onMouseClick(MouseButton.LEFT);
                    }
                    m_LastClick = Time.time;
                    action = true;
                }
                if (Input.GetMouseButtonDown((int)MouseButton.RIGHT))
                {
                    bool doubleClick = false;
                    float delta = Time.time - m_LastClick;
                    if (delta < UIManager.DOUBLE_CLICK_TIME)
                    {
                        doubleClick = true;
                    }

                    if (doubleClick == true)
                    {
                        onMouseDoubleClick(MouseButton.RIGHT);
                        if (trapDoubleClick == false)
                        {
                            onMouseClick(MouseButton.RIGHT);
                        }
                    }
                    else
                    {
                        onMouseClick(MouseButton.RIGHT);
                    }
                    m_LastClick = Time.time;
                    action = true;
                }
                if (Input.GetMouseButtonDown((int)MouseButton.MIDDLE))
                {
                    bool doubleClick = false;
                    float delta = Time.time - m_LastClick;
                    if (delta < UIManager.DOUBLE_CLICK_TIME)
                    {
                        doubleClick = true;
                    }

                    if (doubleClick == true)
                    {
                        onMouseDoubleClick(MouseButton.MIDDLE);
                        if (trapDoubleClick == false)
                        {
                            onMouseClick(MouseButton.MIDDLE);
                        }
                    }
                    else
                    {
                        onMouseClick(MouseButton.MIDDLE);
                    }
                    m_LastClick = Time.time;
                    action = true;
                }

                if (Input.GetMouseButtonUp((int)MouseButton.LEFT))
                {
                    onMouseRelease(MouseButton.LEFT);
                    action = true;
                }
                if (Input.GetMouseButtonUp((int)MouseButton.RIGHT))
                {
                    onMouseRelease(MouseButton.RIGHT);
                    action = true;
                }
                if (Input.GetMouseButtonUp((int)MouseButton.MIDDLE))
                {
                    onMouseRelease(MouseButton.MIDDLE);
                    action = true;
                }

                if (Input.GetMouseButton((int)MouseButton.LEFT))
                {
                    onMouseDown(MouseButton.LEFT);
                    action = true;
                }
                if (Input.GetMouseButton((int)MouseButton.RIGHT))
                {
                    onMouseDown(MouseButton.RIGHT);
                    action = true;
                }
                if (Input.GetMouseButton((int)MouseButton.MIDDLE))
                {
                    onMouseDown(MouseButton.MIDDLE);
                    action = true;
                }

                if (action == false)
                {
                    onMouseHover();
                }

                
            }
            public void processKeyEvents()
            {
                if (m_Manager != null && m_IsFocused == true)
                {
                    KeyCode[] actionKeys = m_Manager.actionKeys;
                    if (actionKeys != null && actionKeys.Length > 0)
                    {
                        for (int i = 0; i < actionKeys.Length; i++)
                        {
                            if (Input.GetKeyDown(actionKeys[i]))
                            {
                                onActionKeyPress(actionKeys[i]);
                            }
                        }
                    }
                }
            }

            public void setFocus(bool aFocus)
            {
                if (m_IsFocused == aFocus)
                {
                    return;
                }
                m_IsFocused = aFocus;
                if (m_IsFocused == true)
                {
                    onFocus();
                }
                else
                {
                    onUnfocus();
                }
            }

            #region EventHelperFuncs
            protected void OnMouseEnter()
            {
                if (m_UIEvent != null && m_Interactive == true)
                {
                    m_UIEvent.Invoke(this, new UIEventArgs(UIEventType.ENTER));
                }
            }
            protected void OnMouseExit()
            {
                if (m_UIEvent != null && m_Interactive == true)
                {
                    m_UIEvent.Invoke(this, new UIEventArgs(UIEventType.EXIT));
                }
            }
            private void onUnfocus()
            {
                if (m_UIEvent != null && m_Interactive == true)
                {
                    m_UIEvent.Invoke(this, new UIEventArgs(UIEventType.UNFOCUS));
                }
            }
            private void onFocus()
            {
                if (m_UIEvent != null && m_Interactive == true)
                {
                    m_UIEvent.Invoke(this, new UIEventArgs(UIEventType.FOCUS));
                }
            }
            private void onMouseClick(MouseButton aButton)
            {
                if (m_UIEvent != null && m_Interactive == true)
                {
                    m_UIEvent.Invoke(this, new UIEventArgs(UIEventType.CLICK, aButton));
                }
            }
            private void onMouseDoubleClick(MouseButton aButton)
            {
                if (m_UIEvent != null && m_Interactive == true)
                {
                    m_UIEvent.Invoke(this, new UIEventArgs(UIEventType.DOUBLE_CLICK, aButton));
                }
            }
            private void onMouseRelease(MouseButton aButton)
            {
                if (m_UIEvent != null && m_Interactive == true)
                {
                    m_UIEvent.Invoke(this, new UIEventArgs(UIEventType.RELEASE, aButton));
                }
            }
            private void onMouseDown(MouseButton aButton)
            {
                if (m_UIEvent != null && m_Interactive == true)
                {
                    m_UIEvent.Invoke(this, new UIEventArgs(UIEventType.DOWN, aButton));
                }
            }
            private void onActionKeyPress(KeyCode aKeyCode)
            {
                if (m_UIEvent != null && m_Interactive == true)
                {
                    m_UIEvent.Invoke(this, new UIEventArgs(UIEventType.KEY_PRESS, MouseButton.NONE,aKeyCode));
                }
            }
            private void onMouseHover()
            {
                if (m_UIEvent != null && m_Interactive == true)
                {
                    m_UIEvent.Invoke(this, new UIEventArgs(UIEventType.HOVER));
                }
            }
            #endregion
        }
    }
}
