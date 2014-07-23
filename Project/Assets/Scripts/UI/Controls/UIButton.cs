using UnityEngine;
using System.Collections;

namespace OnLooker
{
    namespace UI
    {

        //UIButton consists of the UIText and UITexture
        //UIButton uses UITexture for it's events
        public class UIButton : UIControl
        {

            public enum UIButtonState
            {
                DOWN,
                HIGHLIGHTED,
                FOCUSED,
                NORMAL
            }

            private UIButtonState m_ButtonState;

            
            [SerializeField]
            private Texture m_DownTexture;
            [SerializeField]
            private Texture m_HighlightedTexture;
            [SerializeField]
            private Texture m_FocusedTexture;
            [SerializeField]
            private Texture m_NormalTexture;


            public override void init()
            {
                base.init();
            }
            public override void deinit()
            {
                base.deinit();
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