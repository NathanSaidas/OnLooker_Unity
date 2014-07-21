using UnityEngine;
using System.Collections.Generic;


namespace OnLooker
{
    namespace UI
    {
        public class UIManager : MonoBehaviour
        {
            //CONSTANTS
            public const float DOUBLE_CLICK_TIME = 0.5f;
            public const int UI_LAYER = 9;

            [SerializeField()]
            private Font m_DefaultFont = null;
            [SerializeField()]
            private Material m_DefaultFontMaterial = null;
            [SerializeField()]
            private Mesh m_DefaultMesh = null;
            [SerializeField()]
            private Material m_DefaultMaterial = null;
            //The two cameras the UIManager uses
            [SerializeField()]
            private Camera m_MainCamera = null;
            [SerializeField()]
            private Camera m_UserCamera = null;
            [SerializeField()]
            public List<UIToggle> m_Toggles = new List<UIToggle>();
            //Toggle Currently in focus
            [SerializeField]
            private UIToggle m_FocusedToggle = null;

            //Mouse Controls
            [SerializeField]
            private bool m_AllowClickOff = true;
            [SerializeField]
            private float m_MouseDistance = Mathf.Infinity;


            //Keyboard Controls
            [SerializeField]
            private KeyCode m_NextKey = KeyCode.UpArrow;
            [SerializeField]
            private KeyCode m_PreviousKey = KeyCode.DownArrow;
            [SerializeField]
            private KeyCode[] m_ActionKeys;

            

            public void Update()
            {
                if (m_FocusedToggle != null)
                {
                    m_FocusedToggle.processKeyEvents();
                }
            }
            

            public void LateUpdate()
            {
                int layer = 1 << UI_LAYER;
                RaycastHit hit;
                Ray ray = m_MainCamera.ScreenPointToRay(Input.mousePosition);

                //Do a raycast to hit a toggle
                if (Physics.Raycast(ray, out hit, m_MouseDistance, layer))
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
                    go.layer = UI_LAYER;
                    go.AddComponent<BoxCollider>();
                    //uiToggle.interactive = true;
                }
                else
                {
                    
                }
                return uiToggle;
            }


            #region Properties

            public Camera mainCamera
            {
                get{return m_MainCamera;}
                set{m_MainCamera = value;}
            }
            public Camera userCamera
            {
                get{return m_UserCamera;}
                set{m_UserCamera = value;}
            }
            public Camera currentCamera
            {
                get
                {
                    if (m_UserCamera != null)
                    {
                        return m_UserCamera;
                    }
                return m_MainCamera;
                }
                
            }
            public KeyCode nextKey
            {
                get { return m_NextKey; }
                set { m_NextKey = value; }
            }
            public KeyCode previousKey
            {
                get { return m_PreviousKey; }
                set { m_PreviousKey = value; }
            }
            public KeyCode[] actionKeys
            {
                get { return m_ActionKeys; }
            }

            public bool allowClickOff
            {
                get { return m_AllowClickOff; }
                set { m_AllowClickOff = value; }
            }
            public float mouseDistance
            {
                get { return m_MouseDistance; }
                set { m_MouseDistance = value; }
            }

            #endregion

            public UIToggle getToggle(string aName)
            {
                for (int i = 0; i < m_Toggles.Count; i++)
                {
                    if (m_Toggles[i].toggleName == aName)
                    {
                        return m_Toggles[i];
                    }
                }
                return null;
            }
            private void reigsterToggle(UIToggle aToggle)
            {
                if (aToggle != null)
                {
                    m_Toggles.Add(aToggle);
                }
            }
            public void unregisterToggle(UIToggle aToggle)
            {
                m_Toggles.Remove(aToggle);
            }


            //CREATION METHODS

            public UIText createUIText(UIArguments aArgs)
            {
                if (aArgs == null)
                {
                    return null;
                }
                if (aArgs.toggleName == string.Empty)
                {
                    Debug.Log("This toggle has no name");
                    return null;
                }
                if (getToggle(aArgs.toggleName) != null)
                {
                    Debug.Log("Toggle with that name already exists");
                    return null;
                }
                //Create GameObject
                GameObject go = new GameObject("UI Text(" + aArgs.toggleName +  ")");
                go.layer = UI_LAYER;
                //Set Transform
                Transform uiTransform = go.transform;
                uiTransform.parent = transform;
                //Create Required Components
                MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
                TextMesh textMesh = go.AddComponent<TextMesh>();

                textMesh.font = m_DefaultFont;
                textMesh.characterSize = 0.1f;
                textMesh.anchor = TextAnchor.MiddleCenter;
                textMesh.alignment = TextAlignment.Center;

                meshRenderer.material = m_DefaultFontMaterial;
                //Create UIText
                UIText uiText = go.AddComponent<UIText>();
                uiText.setManager(this);
                uiText.toggleName = aArgs.toggleName;
                uiText.isInteractive = aArgs.interactive;
                uiText.trapDoubleClick = aArgs.trapDoubleClick;
                uiText.offsetPosition = aArgs.position;
                uiText.offsetRotation = aArgs.rotation;
                uiText.anchorMode = aArgs.anchorMode;
                uiText.smoothTransform = aArgs.smoothTransform;
                uiText.init();
                uiText.text = aArgs.text;
                uiText.fontSize = aArgs.fontSize;
                uiText.clear();

                if (uiText.isInteractive == true)
                {
                    BoxCollider col = go.AddComponent<BoxCollider>();
                    col.isTrigger = true;
                }
                bool smoothTransform = uiText.smoothTransform;
                uiText.smoothTransform = false;
                uiText.updateTransform();
                uiText.smoothTransform = smoothTransform;

                reigsterToggle(uiText);

                return uiText;
            }
            public UITexture createUITexture(UIArguments aArgs)
            {
                if (aArgs == null)
                {
                    return null;
                }
                if (aArgs.toggleName == string.Empty)
                {
                    Debug.Log("This toggle has no name");
                    return null;
                }
                if (getToggle(aArgs.toggleName) != null)
                {
                    Debug.Log("Toggle with that name already exists");
                    return null;
                }
                //Create GameObject
                GameObject go = new GameObject("UI Texture (" + aArgs.toggleName +  ")");
                go.layer = UI_LAYER;
                //Set Transform
                Transform uiTransform = go.transform;
                uiTransform.parent = transform;
                //Create Required Components
                MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
                MeshFilter meshFilter = go.AddComponent<MeshFilter>();

                meshFilter.mesh = m_DefaultMesh;
                meshRenderer.material = m_DefaultMaterial;
                //Create UIText
                UITexture uiTexture = go.AddComponent<UITexture>();
                uiTexture.setManager(this);
                uiTexture.toggleName = aArgs.toggleName;
                uiTexture.isInteractive = aArgs.interactive;
                uiTexture.trapDoubleClick = aArgs.trapDoubleClick;
                uiTexture.offsetPosition = aArgs.position;
                uiTexture.offsetRotation = aArgs.rotation;
                uiTexture.anchorMode = aArgs.anchorMode;
                uiTexture.smoothTransform = aArgs.smoothTransform;
                uiTexture.init();
                uiTexture.texture = aArgs.texture;

                if (uiTexture.isInteractive == true)
                {
                    BoxCollider col = go.AddComponent<BoxCollider>();
                    col.isTrigger = true;
                }

                reigsterToggle(uiTexture);

                return uiTexture;
            }
        }
    }
}
