using UnityEngine;
using System.Collections;

namespace OnLooker
{
    namespace UI
    {

        public class UILabel : UIEventHandler
        {
            [SerializeField]
            private UIText m_Text;
            [SerializeField]
            private UITexture m_Texture;

            [SerializeField]
            private Vector2 m_BoarderSize;

            private void Start()
            {
                init();
                m_Text.setTextChanged(onTextChanged);
                m_Text.setTextChangedImmediate(onTextChangedImmediate);
            }

            public void init()
            {
                m_Text = GetComponentInChildren<UIText>();
                m_Texture = GetComponentInChildren<UITexture>();
                TextMesh textmesh = m_Text.GetComponent<TextMesh>();
                if (textmesh != null)
                {
                    textmesh.offsetZ = -0.1f;
                }
            }
            private void LateUpdate()
            {
                
            }
            public void updateBackground()
            {
                Vector3 scale = m_Texture.transform.localScale;
                BoxCollider col = m_Text.GetComponent<BoxCollider>();
                if (col != null)
                {
                    scale.x = col.size.x * 1.0f + m_BoarderSize.x;
                    scale.y = col.size.y * 1.0f + m_BoarderSize.y;
                    m_Texture.transform.localScale = scale;
                }
            }
            //For editor use only really.
            public void updateTransform()
            {
                if (m_Text != null)
                {
                    m_Text.updateTransform();
                }
                if (m_Texture != null)
                {
                    m_Texture.updateTransform();
                }
            }

            protected override void onUIEvent(UIToggle aSender, UIEventArgs aArgs)
            {
                
            }
            protected virtual void onTextChanged(UIText aSender, string aText)
            {
                if (m_Texture == null)
                {
                    return;
                }
                Debug.Log("Text Changed: " + aText);
                //Update Texture Mesh Vertices
                //BoxCollider boxCol = aSender.GetComponent<BoxCollider>();
                //m_Center = boxCol.bounds.center;
                //m_Extents = boxCol.bounds.extents;
                //
                //Vector3 scale = new Vector3(m_Extents.x * 2.5f, m_Extents.y * 2.5f, 1.0f);
                //m_Texture.transform.localScale = scale;

            }
            protected virtual void onTextChangedImmediate(UIText aSender, string aText)
            {

            }

            public UIText textComponent
            {
                get { return m_Text; }
                set { m_Text = value; }
            }
            public UITexture textureComponent
            {
                get { return m_Texture; }
                set { m_Texture = value; }
            }

            public Vector3 offsetPosition
            {
                get
                {
                    if (m_Text != null)
                    {
                        return m_Text.offsetPosition;
                    }
                    return Vector3.zero;
                }
                set
                {
                    if (m_Text != null)
                    {
                        m_Text.offsetPosition = value;
                    }
                    if (m_Texture != null)
                    {
                        m_Texture.offsetPosition = value;
                    }
                }
            }
            public Vector3 offsetRotation
            {
                get
                {
                    if (m_Text != null)
                    {
                        return m_Text.offsetRotation;
                    }
                    return Vector3.zero;
                }
                set
                {
                    if (m_Text != null)
                    {
                        m_Text.offsetRotation = value;
                    }
                    if (m_Texture != null)
                    {
                        m_Texture.offsetRotation = value;
                    }
                }
            }
            public Transform anchorTarget
            {
                get
                {
                    if (m_Text != null)
                    {
                        return m_Text.anchorTarget;
                    }
                    return null;
                }
                set
                {
                    if (m_Text != null)
                    {
                        m_Text.anchorTarget = value;
                    }
                    if (m_Texture != null)
                    {
                        m_Texture.anchorTarget = value;
                    }
                }
            }
            public UIAnchor anchorMode
            {
                get
                {
                    if (m_Text != null)
                    {
                        return m_Text.anchorMode;
                    }
                    return UIAnchor.NONE;
                }
                set
                {
                    if (m_Text != null)
                    {
                        m_Text.anchorMode = value;
                    }
                    if (m_Texture != null)
                    {
                        m_Texture.anchorMode = value;
                    }
                }
            }
            public bool faceCamera
            {
                get
                {
                    if (m_Text != null)
                    {
                        return m_Text.faceCamera;
                    }
                    return false;
                }
                set
                {
                    if (m_Text != null)
                    {
                        m_Text.faceCamera = value;
                    }
                    if (m_Texture != null)
                    {
                        m_Texture.faceCamera = value;
                    }
                }
            }
            public bool smoothTransform
            {
                get
                {
                    if (m_Text != null)
                    {
                        return m_Text.smoothTransform;
                    }
                    return false;
                }
                set
                {
                    if (m_Text != null)
                    {
                        m_Text.smoothTransform = value;
                    }
                    if (m_Texture != null)
                    {
                        m_Texture.smoothTransform = value;
                    }
                }
            }

            public string text
            {
                get
                {
                    if (m_Text != null)
                    {
                        return m_Text.text;
                    }
                    return string.Empty;
                }
                set
                {
                    if (m_Text != null)
                    {
                        m_Text.text = value;
                    }
                }
            }
            public Texture backgroundTexture
            {
                get
                {
                    if (m_Texture != null)
                    {
                        return m_Texture.texture;
                    }
                    return null;
                }

                set
                {
                    if (m_Texture != null)
                    {
                        m_Texture.texture = value;
                    }
                }
            }

            //End Properties
        }
    }
}