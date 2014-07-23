using UnityEngine;
using System;
using System.Collections;

namespace OnLooker
{
    namespace UI
    {

        //UIButton consists of the UIText and UITexture
        //UIButton uses UITexture for it's events
        [Serializable]
        public class UIButton : UIControl
        {

            public enum UIButtonState
            {
                DOWN,
                HIGHLIGHTED,
                FOCUSED,
                NORMAL
            }

            

            
            [SerializeField]
            protected Texture m_DownTexture;
            [SerializeField]
            protected Texture m_HighlightedTexture;
            [SerializeField]
            protected Texture m_FocusedTexture;
            [SerializeField]
            protected Texture m_NormalTexture;
            [SerializeField]
            private UIButtonState m_ButtonState;


            public override void init()
            {
                base.init();
            }
            public override void deinit()
            {
                base.deinit();
            }

            public void registerUIEvent(UIEvent aCallback)
            {
                if (m_TextureComponent != null)
                {
                    m_TextureComponent.registerEvent(aCallback);
                }
            }
            public void unregisterUIEvent(UIEvent aCallback)
            {
                if (m_TextureComponent != null)
                {
                    m_TextureComponent.registerEvent(aCallback);
                }
            }
            private void transparent()
            {
                Color color = backgroundColor;
                color.a = 0.0f;
                backgroundColor = color;
            }
            private void opaque()
            {
                Color color = backgroundColor;
                color.a = 1.0f;
                backgroundColor = color;
            }

            private void LateUpdate()
            {
                if (textureComponent.mouseInBounds == true && OnLookerUtils.anyMouseButtonDown(true) == true)
                {
                    m_ButtonState = UIButtonState.DOWN;

                    if (downTexture != null)
                    {
                        backgroundTexture = downTexture;
                        opaque();
                    }
                    else if (normalTexture != null)
                    {
                        backgroundTexture = normalTexture;
                        opaque();
                    }
                    else if (normalTexture == null)
                    {
                        backgroundTexture = normalTexture;
                        transparent();
                    }
                }
                else if (textureComponent.mouseInBounds == true && OnLookerUtils.anyMouseButtonDown(true) == false)
                {
                    m_ButtonState = UIButtonState.HIGHLIGHTED;
                    if (highlightedTexture != null)
                    {
                        backgroundTexture = highlightedTexture;
                        opaque();
                    }
                    else if (normalTexture != null)
                    {
                        backgroundTexture = normalTexture;
                        opaque();
                    }
                    else if (normalTexture == null)
                    {
                        backgroundTexture = normalTexture;
                        transparent();
                    }
                }
                else if (textureComponent.isFocused == true)
                {
                    m_ButtonState = UIButtonState.FOCUSED;
                    if (focusedTexture != null)
                    {
                        backgroundTexture = focusedTexture;
                        opaque();
                    }
                    else if (normalTexture != null)
                    {
                        backgroundTexture = normalTexture;
                        opaque();
                    }
                    else if (normalTexture == null)
                    {
                        backgroundTexture = normalTexture;
                        transparent();
                    }
                }
                else
                {
                    m_ButtonState = UIButtonState.NORMAL;
                    if (normalTexture != null)
                    {
                        backgroundTexture = normalTexture;
                        opaque();
                    }
                    else if (normalTexture == null)
                    {
                        backgroundTexture = normalTexture;
                        transparent();
                    }
                }
            }

            protected override void onUIEvent(UIToggle aSender, UIEventArgs aArgs)
            {
                
            }

            public string text
            {
                get
                {
                    if (m_TextComponent != null)
                    {
                        return m_TextComponent.text;
                    }
                    return string.Empty;
                }
                set
                {
                    if (m_TextComponent != null)
                    {
                        m_TextComponent.text = value;
                    }
                }
            
            }


            public Texture highlightedTexture
            {
                get { return m_HighlightedTexture; }
                set { m_HighlightedTexture = value; }
            }
            public Texture downTexture
            {
                get { return m_DownTexture; }
                set { m_DownTexture = value; }
            }
            public Texture focusedTexture
            {
                get { return m_FocusedTexture; }
                set { m_FocusedTexture = value; }
            }
            public Texture normalTexture
            {
                get { return m_NormalTexture; }
                set { m_NormalTexture = value; }
            }

            public UIButtonState buttonState
            {
                get { return m_ButtonState; }
                set { m_ButtonState = value; }
            }

            public Texture backgroundTexture
            {
                get
                {
                    if (m_TextureComponent != null)
                    {
                        return m_TextureComponent.texture;
                    }
                    return null;
                }

                set
                {
                    if (m_TextureComponent != null)
                    {
                        m_TextureComponent.texture = value;
                    }
                }
            }

            public Color backgroundColor
            {
                get
                {
                    if (m_TextureComponent != null)
                    {
                        return m_TextureComponent.color;
                    }
                    return Color.white;
                }
                set
                {
                    if (m_TextureComponent != null)
                    {
                        m_TextureComponent.color = value;
                    }
                }
            }

        }
    }
}