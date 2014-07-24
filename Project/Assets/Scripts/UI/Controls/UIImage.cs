using UnityEngine;
using System;
using System.Collections;


namespace OnLooker
{
    namespace UI
    {
        //UIImage does not make use of UIText

        [Serializable]
        public class UIImage : UIControl
        {
            public override void init()
            {
                m_TextureComponent = GetComponentInChildren<UITexture>();
                if (Application.isPlaying == true)
                {
                    m_TextureComponent.registerEvent(onUIEvent);
                }
            }

            public override void deinit()
            {
                m_TextureComponent.unregisterEvent(onUIEvent);
                base.deinit();
            }

            public void registerEvent(UIEvent aCallback)
            {
                if (m_TextureComponent != null)
                {
                    m_TextureComponent.registerEvent(aCallback);
                }
            }
            public void unregisterEvent(UIEvent aCallback)
            {
                if (m_TextureComponent != null)
                {
                    m_TextureComponent.unregisterEvent(aCallback);
                }
                
            }


            protected override void onUIEvent(UIToggle aSender, UIEventArgs aArgs)
            {
                
            }

            public Texture texture
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
            public Color color
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