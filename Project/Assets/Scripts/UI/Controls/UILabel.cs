using UnityEngine;
using System;
using System.Collections;

namespace OnLooker
{
    namespace UI
    {
        [Serializable]
        public class UILabel : UIControl
        {
            //public const int NO_LIMIT = -1;

            [SerializeField]
            private bool m_FixedBackground = false;
            [SerializeField]
            private int m_MaxCharacter = UIUtilities.NO_LIMIT;
            [SerializeField]
            private Vector2 m_BoarderSize = Vector2.zero;


            private event TextChanged m_TextChanged;

            //The purpose of this function is to initialize this class
            //Get references to the required components
            //And set the default state of them
            public override void init()
            {
                base.init();
                if (Application.isPlaying)
                {
                    m_TextComponent.registerEvent(onUIEvent);
                    m_TextComponent.setTextChanged(onTextChanged);
                    m_TextComponent.setTextChangedImmediate(onTextChangedImmediate);
                    //After that we can safely check for max characters
                    if (m_MaxCharacter != UIUtilities.NO_LIMIT)
                    {
                        if (m_TextComponent.text.Length > m_MaxCharacter)
                        {
                            m_TextComponent.text = m_TextComponent.text.Substring(0, m_MaxCharacter - 1);
                        }
                    }
                }
            }
            public override void deinit()
            {
                m_TextComponent.unregisterEvent(onUIEvent);
                m_TextComponent.setTextChanged(null);
                m_TextComponent.setTextChangedImmediate(null);
                base.deinit();
            }

            private void LateUpdate()
            {
                if (Application.isPlaying == true)
                {
                    updateBackground();
                }
            }

            public void updateBackground()
            {
                //If this UILabel has a fixed background that means its size is related to the size of the bounds for the collider
                //which encapsulates the text
                if (m_FixedBackground == true)
                {
                    //Get the scale and collider
                    Vector3 scale = m_TextureComponent.transform.localScale;
                    BoxCollider col = m_TextComponent.GetComponent<BoxCollider>();
                    //Set the scale accordingly
                    if (col != null)
                    {
                        scale.x = col.size.x * m_TextComponent.transform.localScale.x + m_BoarderSize.x;
                        scale.y = col.size.y * m_TextComponent.transform.localScale.y + m_BoarderSize.y;
                        m_TextureComponent.transform.localScale = scale;
                    }
                }

                //Otherwise if there is no fixed background the user is free to set the background to whatever size they want
            }
            public void registerUIEvent(UIEvent aCallback)
            {
                if (m_TextComponent != null)
                {
                    m_TextComponent.registerEvent(aCallback);
                }
            }
            public void unregisterUIEvent(UIEvent aCallback)
            {
                if (m_TextComponent != null)
                {
                    m_TextComponent.unregisterEvent(aCallback);
                }
            }

            public void registerTextChangedEvent(TextChanged aCallback)
            {
                if (aCallback != null)
                {
                    m_TextChanged += aCallback;
                }
            }
            public void unregisterTextChangedEvent(TextChanged aCallback)
            {
                if (m_TextChanged != null && aCallback != null)
                {
                    m_TextChanged -= aCallback;
                }
            }

            protected override void onUIEvent(UIToggle aSender, UIEventArgs aArgs)
            {

            }

            protected virtual string onTextChanged(UIText aSender, string aText)
            {
                return aText;
            }
            //Everytime the text changes check to see if we need to refine it
            protected virtual string onTextChangedImmediate(UIText aSender, string aText)
            {
                string returnText = aText;
                if (m_TextComponent == null)
                {
                    if (m_TextChanged != null)
                    {
                        m_TextChanged.Invoke(aSender, aText);
                    }
                    return aText;
                }
                //If there is a limit on the characters reduce the characters to a substring of the limit
                if (m_MaxCharacter != UIUtilities.NO_LIMIT)
                {
                    if (aText.Length > m_MaxCharacter)
                    {
                        returnText = aText.Substring(0, m_MaxCharacter);
                    }
                }
                if (m_TextChanged != null)
                {
                    m_TextChanged.Invoke(aSender, returnText);
                }
                return returnText;
            }




            //This property represents the string of text from the text component
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
            //These properties refer to the texture component
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
                    if (m_TextureComponent != null )
                    {
                        m_TextureComponent.color = value;
                    }
                }
            }
            //End Properties
        }
    }
}