using UnityEngine;

namespace OnLooker
{
    namespace UI
    {


        public class UIControl : UIEventHandler
        {
            [SerializeField]
            protected UIText m_TextComponent;
            [SerializeField]
            protected UITexture m_TextureComponent;
            [SerializeField]
            protected string m_ControlName = string.Empty;

            public void Start()
            {
                if (Application.isPlaying)
                {
                    init();
                }
            }
            public void OnDestroy()
            {
                deinit();
            }

            public virtual void init()
            {
                m_TextComponent = GetComponentInChildren<UIText>();
                m_TextureComponent = GetComponentInChildren<UITexture>();
                if (m_TextComponent == null || m_TextureComponent == null)
                {
                    Debug.LogWarning("Failed Initialization");
                    return;
                }
                TextMesh textMesh = m_TextComponent.GetComponent<TextMesh>();
                if (textMesh != null)
                {
                    textMesh.offsetZ = -0.1f;
                }
            }
            public virtual void deinit()
            {
                m_TextComponent.manager.unregisterControl(this);
            }

            public void updateTransform()
            {
                if(m_TextComponent != null)
                {
                    m_TextComponent.updateTransform();
                }
                if (m_TextureComponent != null)
                {
                    m_TextureComponent.updateTransform();
                }
            }

            //Required Components / GameObject References
            public UIText textComponent
            {
                get { return m_TextComponent; }
                set { m_TextComponent = value; }
            }
            public UITexture textureComponent
            {
                get { return m_TextureComponent; }
                set { m_TextureComponent = value; }
            }

            //Transform properties that are shared between the two Required Components / Game Objects
            public Vector3 offsetPosition
            {
                get
                {
                    if (m_TextComponent != null)
                    {
                        return m_TextComponent.offsetPosition;
                    }
                    return Vector3.zero;
                }
                set
                {
                    if (m_TextComponent != null)
                    {
                        m_TextComponent.offsetPosition = value;
                    }
                    if (m_TextureComponent != null)
                    {
                        value.y = value.y * 1.05f;
                        m_TextureComponent.offsetPosition = value;
                    }
                }
            }
            public Vector3 offsetRotation
            {
                get
                {
                    if (m_TextComponent != null)
                    {
                        return m_TextComponent.offsetRotation;
                    }
                    return Vector3.zero;
                }
                set
                {
                    if (m_TextComponent != null)
                    {
                        m_TextComponent.offsetRotation = value;
                    }
                    if (m_TextureComponent != null)
                    {
                        m_TextureComponent.offsetRotation = value;
                    }
                }
            }
            public Transform anchorTarget
            {
                get
                {
                    if (m_TextComponent != null)
                    {
                        return m_TextComponent.anchorTarget;
                    }
                    return null;
                }
                set
                {
                    if (m_TextComponent != null)
                    {
                        m_TextComponent.anchorTarget = value;
                    }
                    if (m_TextureComponent != null)
                    {
                        m_TextureComponent.anchorTarget = value;
                    }
                }
            }
            public UIAnchor anchorMode
            {
                get
                {
                    if (m_TextComponent != null)
                    {
                        return m_TextComponent.anchorMode;
                    }
                    return UIAnchor.NONE;
                }
                set
                {
                    if (m_TextComponent != null)
                    {
                        m_TextComponent.anchorMode = value;
                    }
                    if (m_TextureComponent != null)
                    {
                        m_TextureComponent.anchorMode = value;
                    }
                }
            }
            public bool faceCamera
            {
                get
                {
                    if (m_TextComponent != null)
                    {
                        return m_TextComponent.faceCamera;
                    }
                    return false;
                }
                set
                {
                    if (m_TextComponent != null)
                    {
                        m_TextComponent.faceCamera = value;
                    }
                    if (m_TextureComponent != null)
                    {
                        m_TextureComponent.faceCamera = value;
                    }
                }
            }
            public bool smoothTransform
            {
                get
                {
                    if (m_TextComponent != null)
                    {
                        return m_TextComponent.smoothTransform;
                    }
                    return false;
                }
                set
                {
                    if (m_TextComponent != null)
                    {
                        m_TextComponent.smoothTransform = value;
                    }
                    if (m_TextureComponent != null)
                    {
                        m_TextureComponent.smoothTransform = value;
                    }
                }
            }
            public string controlName
            {
                get { return m_ControlName; }
                set { m_ControlName = value; }
            }
        }
    }
}
