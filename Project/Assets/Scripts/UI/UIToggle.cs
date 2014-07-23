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
            [SerializeField()][HideInInspector()]
            private bool m_Debug = false;


            //A reference to the UI Manager
            [SerializeField()]//[HideInInspector()]
            protected UIManager m_Manager = null;


            //The name of the toggle - For UIManager
            [SerializeField()]//[HideInInspector()]
            protected string m_ToggleName = string.Empty;

            
            //Whether or not the toggle is interactive
            [SerializeField()]//[HideInInspector()]
            private bool m_Interactive = false;

            //Input Control
            [SerializeField()]//[HideInInspector()]
            protected bool m_TrapDoubleClick = false;
            protected float m_LastClick = 0.0f;
            [SerializeField()]
            private UIControl m_ParentControl;
            private event UIEvent m_UIEvent;


            [SerializeField()]
            protected Vector3 m_OffsetPosition = Vector3.zero;
            [SerializeField()]
            protected Vector3 m_OffsetRotation = Vector3.zero;

            [SerializeField()]
            protected Transform m_AnchorTarget = null;
            [SerializeField()]
            protected UIAnchor m_AnchorMode = UIAnchor.CAMERA;
            [SerializeField()]
            protected bool m_FaceCamera = true;
            [SerializeField()]
            protected bool m_SmoothTransform = true;


            


            //User accessible flags
            [SerializeField]
            protected bool m_MouseInBounds = false;
            [SerializeField]
            protected bool m_IsFocused = false;

            private void OnDestroy()
            {
                if (m_Manager != null)
                {
                    m_Manager.unregisterToggle(this);
                }
            }

            private void Update()
            {
                if (Application.isPlaying == true)
                {
                    gameUpdate();
                }
            }
            private void LateUpdate()
            {
                if (Application.isPlaying == true)
                {
                    updateTransform();
                    gameLateUpdate();
                }
            }
            private void FixedUpdate()
            {
                if (Application.isPlaying == true)
                {
                    gameFixedUpdate();
                }
            }

            protected virtual void gameUpdate()
            {

            }
            protected virtual void gameLateUpdate()
            {

            }
            protected virtual void gameFixedUpdate()
            {

            }


            public void updateTransform()
            {
                
                //The manager is required for the camera
                //If there is no manager or camera this function returns and does nothing
                if(m_Manager == null)
                {
                    Debug.Log("UIToggle: No Manager");
                    return;
                }
                Camera currentCamera = m_Manager.currentCamera;
                if(currentCamera == null)
                {
                    Debug.Log("UIToggle: No Camera");
                    return;
                }
                

                switch (m_AnchorMode)
                {
                    case UIAnchor.NONE:
                        {
                            //Position is whereever the user sets the transform
                            if (m_FaceCamera == true)
                            {
                                transform.LookAt(transform.position + currentCamera.transform.rotation * Vector3.forward, currentCamera.transform.rotation * Vector3.up);
                                transform.Rotate(m_OffsetRotation);
                            }
                        }
                        break;
                    case UIAnchor.CAMERA:
                        {
                            //Position is offset from the camera local position / rotation
                            //The camera does not lerp
                            Vector3 position = currentCamera.transform.position + currentCamera.transform.rotation * m_OffsetPosition; 
                            transform.position = position;
                            //Override m_FaceCamera to be true by default when attached to the camera
                            transform.LookAt(transform.position + currentCamera.transform.rotation * Vector3.forward, currentCamera.transform.rotation * Vector3.up);
                            transform.Rotate(m_OffsetRotation);
                        }
                        break;
                    case UIAnchor.OBJECT:
                        {
                            if (m_AnchorTarget == null)
                            {
                                return;
                            }
                            //Set position based on offset
                            Vector3 position = m_AnchorTarget.position + m_AnchorTarget.rotation * m_OffsetPosition;
                            if (m_SmoothTransform == true)
                            {
                                transform.position = Vector3.Lerp(transform.position, position, 5.0f * Time.deltaTime);
                            }
                            else
                            {
                                transform.position = position;
                            }
                            //Set rotation based on offset
                            if (m_FaceCamera == true)
                            {
                                transform.LookAt(transform.position + currentCamera.transform.rotation * Vector3.forward, currentCamera.transform.rotation * Vector3.up);
                                transform.Rotate(m_OffsetRotation);
                            }
                            else
                            {
                                transform.rotation = m_AnchorTarget.rotation;
                                transform.Rotate(m_OffsetRotation);
                            }
                        }
                        break;
                }
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

            //For UIManager
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
            public void setManager(UIManager aManager)
            {
                if (m_Manager != null)
                {
                    Debug.LogWarning("More than one UI Manager in the scene possibly?");
                }
                m_Manager = aManager;
            }

            #region EventHelperFuncs
            public void onMouseEnter()
            {
                if (m_Interactive == true)
                {
                    m_MouseInBounds = true;
                    if (m_UIEvent != null)
                    {
                        m_UIEvent.Invoke(this, new UIEventArgs(UIEventType.ENTER));
                    }
                }
            }
            public void onMouseExit()
            {
                if (m_Interactive == true)
                {
                    m_MouseInBounds = false;
                    if (m_UIEvent != null)
                    {
                        m_UIEvent.Invoke(this, new UIEventArgs(UIEventType.EXIT));
                    }
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
            #region Properties
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
                set
                {
                    m_Interactive = value;
                    if (m_Interactive == true)
                    {
                        gameObject.layer = UIManager.UI_LAYER;
                    }
                }
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

            public Vector3 offsetPosition
            {
                get { return m_OffsetPosition; }
                set { m_OffsetPosition = value; }
            }
            public Vector3 offsetRotation
            {
                get { return m_OffsetRotation; }
                set { m_OffsetRotation = value; }
            }
            public Transform anchorTarget
            {
                get { return m_AnchorTarget; }
                set { m_AnchorTarget = value; }
            }
            public UIAnchor anchorMode
            {
                get { return m_AnchorMode; }
                set { m_AnchorMode = value; }
            }
            public bool faceCamera
            {
                get { return m_FaceCamera; }
                set { m_FaceCamera = value; }
            }
            public bool smoothTransform
            {
                get { return m_SmoothTransform; }
                set { m_SmoothTransform = value; }
            }
            public UIControl parentControl
            {
                get { return m_ParentControl; }
                set { m_ParentControl = value; }
            }

#endregion
        }
    }
}
