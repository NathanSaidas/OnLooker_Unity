using UnityEngine;
using System;
using System.Collections.Generic;

namespace OnLooker
{
    namespace UI
    {
        [Serializable]
        public class UITextfield : UIControl
        {
            [SerializeField]
            private bool m_FixedBackground = false;
            [SerializeField]
            private int m_MaxCharacter = UIUtilities.NO_LIMIT;
            [SerializeField]
            private Vector2 m_BoarderSize = Vector2.zero;

            private event TextChanged m_TextChanged;

            private float m_BackspaceKeyTime = 0.0f;
            private float m_SpacebarKeyTime = 0.0f;

            public override void init()
            {
                base.init();
                if (Application.isPlaying)
                {
                    m_TextComponent.registerEvent(onUIEvent);
                    m_TextComponent.setTextChanged(onTextChanged);
                    m_TextComponent.setTextChangedImmediate(onTextChangedImmediate);
                    if (m_MaxCharacter != UIUtilities.NO_LIMIT)
                    {
                        if (m_TextComponent.text.Length > m_MaxCharacter)
                        {
                            m_TextComponent.text = m_TextComponent.text.Substring(0, m_MaxCharacter);
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
                    //Input
                    if (m_TextComponent.isFocused == true)
                    {
                        string inputString = Input.inputString;
                        string currentText = text;
                        string verifiedString = string.Empty;

                        if (inputString.Length > 0)
                        {
                            if (inputString[0] > 32 && inputString[0] < 127)
                            {
                                verifiedString += inputString[0];
                            }
                        }
                        //Backspace
                        if (Input.GetKeyDown(KeyCode.Backspace) && currentText.Length > 0)
                        {
                            currentText = currentText.Substring(0, currentText.Length - 1);
                        }
                        if (Input.GetKey(KeyCode.Backspace) && currentText.Length > 0)
                        {
                            m_BackspaceKeyTime += Time.deltaTime;
                            if (m_BackspaceKeyTime > 0.5f)
                            {
                                currentText = currentText.Substring(0, currentText.Length - 1);
                            }
                        }
                        else if (Input.GetKey(KeyCode.Backspace) == false)
                        {
                            m_BackspaceKeyTime = 0.0f;
                        }

                        //Space
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            currentText += " ";
                        }
                        if (Input.GetKey(KeyCode.Space))
                        {
                            m_SpacebarKeyTime += Time.deltaTime;
                            if (m_BackspaceKeyTime > 0.5f)
                            {
                                currentText += " ";
                            }
                        }
                        else
                        {
                            m_SpacebarKeyTime = 0.0f;
                        }

                        currentText += verifiedString;
                        text = currentText;


                    }

                    //End Input
                    updateBackground();
                }
            }

            public void updateBackground()
            {

                if (m_FixedBackground == true)
                {
                    Vector3 scale = m_TextureComponent.transform.localScale;
                    BoxCollider col = m_TextComponent.GetComponent<BoxCollider>();
                    
                    if (col != null)
                    {
                        scale.x = col.size.x * m_TextComponent.transform.localScale.x + m_BoarderSize.x;
                        scale.y = col.size.y * m_TextComponent.transform.localScale.y + m_BoarderSize.y;
                        m_TextureComponent.transform.localScale = scale;
                    }
                }
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
                    if (m_TextureComponent != null)
                    {
                        m_TextureComponent.color = value;
                    }
                }
            }
            
        }
    }
}