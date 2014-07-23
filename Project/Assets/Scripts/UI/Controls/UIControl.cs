using UnityEngine;

namespace OnLooker
{
    namespace UI
    {

        [ExecuteInEditMode()]
        public class UIControl : UIEventHandler
        {
            [SerializeField][HideInInspector()]
            protected UIText m_TextComponent;
            [SerializeField][HideInInspector()]
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
                if (m_TextComponent != null)
                {
                    m_TextComponent.manager.unregisterControl(this);
                }
                else if (m_TextureComponent != null)
                {
                    m_TextureComponent.manager.unregisterControl(this);
                }
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
            public virtual Vector3 offsetPosition
            {
                get
                {
                    if (m_TextComponent != null)
                    {
                        return m_TextComponent.offsetPosition;
                    }
                    if (m_TextureComponent != null)
                    {
                        return m_TextureComponent.offsetPosition;
                    }
                    return Vector3.zero;
                }
                set
                {
                    if (m_TextComponent == null && m_TextureComponent != null)
                    {
                        m_TextureComponent.offsetPosition = value;
                        return;
                    }
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
            public virtual Vector3 offsetRotation
            {
                get
                {
                    if (m_TextComponent != null)
                    {
                        return m_TextComponent.offsetRotation;
                    }
                    if (m_TextureComponent != null)
                    {
                        return m_TextureComponent.offsetRotation;
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
            public virtual Transform anchorTarget
            {
                get
                {
                    if (m_TextComponent != null)
                    {
                        return m_TextComponent.anchorTarget;
                    }
                    if (m_TextureComponent != null)
                    {
                        return m_TextureComponent.anchorTarget;
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
            public virtual UIAnchor anchorMode
            {
                get
                {
                    if (m_TextComponent != null)
                    {
                        return m_TextComponent.anchorMode;
                    }
                    if (m_TextureComponent != null)
                    {
                        return m_TextureComponent.anchorMode;
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
            public virtual bool faceCamera
            {
                get
                {
                    if (m_TextComponent != null)
                    {
                        return m_TextComponent.faceCamera;
                    }
                    if (m_TextureComponent != null)
                    {
                        return m_TextureComponent.faceCamera;
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
            public virtual bool smoothTransform
            {
                get
                {
                    if (m_TextComponent != null)
                    {
                        return m_TextComponent.smoothTransform;
                    }
                    if (m_TextureComponent != null)
                    {
                        return m_TextureComponent.smoothTransform;
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
