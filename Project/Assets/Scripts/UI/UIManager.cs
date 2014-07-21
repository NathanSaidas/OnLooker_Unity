using UnityEngine;
using System.Collections.Generic;


namespace OnLooker
{
    namespace UI
    {
        public class UIManager : MonoBehaviour
        {

            public const float DOUBLE_CLICK_TIME = 0.5f;

            private static int m_UILayer = 9;

            public static int uiLayer
            {
                get { return m_UILayer; }
                set { m_UILayer = value; }
            }

            public Camera m_Camera;
            public UIToggle[] m_Toggles;

            [SerializeField]
            private UIToggle m_FocusedToggle = null;
            [SerializeField]
            private bool m_AllowClickOff = true;
            [SerializeField]
            private float m_GUIDistance = Mathf.Infinity;
            [SerializeField]
            private KeyCode m_NextKey = KeyCode.UpArrow;
            [SerializeField]
            private KeyCode m_PreviousKey = KeyCode.DownArrow;

            [SerializeField]
            private KeyCode[] m_ActionKeys;

            public KeyCode[] actionKeys
            {
                get { return m_ActionKeys; }
            }

            public bool allowClickOff
            {
                get{return m_AllowClickOff;}
                set{m_AllowClickOff = value;}
            }

            public void Update()
            {
                if (m_FocusedToggle != null)
                {
                    m_FocusedToggle.processKeyEvents();
                }
            }
            

            public void FixedUpdate()
            {
                int layer = 1 << m_UILayer;
                RaycastHit hit;
                Ray ray = m_Camera.ScreenPointToRay(Input.mousePosition);

                //Do a raycast to hit a toggle
                if (Physics.Raycast(ray, out hit, m_GUIDistance, layer))
                {
                    //Does this collider have a toggle?
                    UIToggle toggle = hit.collider.GetComponent<UIToggle>();
                    if (toggle != null)
                    {
                        //Check to see if any mouse button was pressed
                        //If so were going to need to focus / unfocus the toggle depending on the following conditions
                        //Condition A - Toggle is not interactive and the manager allows clicking off into space
                        //Condition B - Toggle is interactive
                        if (OnLookerUtils.anyMouseButtonDown() == true)
                        {
                            if (toggle.isInteractive == false && allowClickOff == true)
                            {
                                Debug.Log("Unfocus Toggle");
                                unfocusToggle();
                            }
                            else if (toggle.isInteractive == true)
                            {
                                Debug.Log("Focus Toggle");
                                focusToggle(toggle);
                            }
                            else
                            {
                                //Do nozing
                            }
                        }

                        //If the toggle is interactive process its events
                        if (toggle.isInteractive == true)
                        {
                            Debug.Log("Toggle Process Events");

                            toggle.processEvents();
                        }
                    }
                }
                else
                {
                    if (m_AllowClickOff == true && OnLookerUtils.anyMouseButtonDown() == true)
                    {
                        unfocusToggle();
                    }
                }

                

                
            }

            public void focusToggle(UIToggle aToggle)
            {
                if (aToggle == m_FocusedToggle)
                {
                    return;
                }
                if (aToggle != null)
                {
                    unfocusToggle();
                    m_FocusedToggle = aToggle;
                    m_FocusedToggle.setFocus(true);
                }
            }
            public void unfocusToggle()
            {
                if (m_FocusedToggle != null)
                {
                    m_FocusedToggle.setFocus(false);
                    m_FocusedToggle = null;
                }
            }




            public UIToggle createToggle(bool aInteractive)
            {
                GameObject go = new GameObject("Toggle");
               

                Transform uiTransform = go.transform;
                uiTransform.parent = transform;

                UIToggle uiToggle = go.AddComponent<UIToggle>();
                

                if (aInteractive)
                {
                    go.layer = uiLayer;
                    go.AddComponent<BoxCollider>();
                    //uiToggle.interactive = true;
                }
                else
                {
                    
                }
                return uiToggle;
            }
        }
    }
}
