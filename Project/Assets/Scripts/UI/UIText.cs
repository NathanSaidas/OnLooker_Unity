using UnityEngine;
using System;
using System.Collections.Generic;

namespace OnLooker
{
    namespace UI
    {
        [Serializable]
        public class UIText : UIToggle
        {
            //Properties from UIToggle
            //debug
            //manager
            //mouseInBounds
            //isFocused
            //isInteractive
            //trapDoubleClick
            //lastClick
            //offsetPosition
            //offsetRotation
            //anchorTarget
            //anchorMode
            //smoothTransform

            [SerializeField]
            private TextMesh m_TextMesh = null;
            [SerializeField]
            private Material m_TextMaterial = null;

            private bool m_UpdateText = false;
            

            // Use this for initialization
            void Start()
            {

                init();
                m_UpdateText = true;
            }

            public void init()
            {
                if (m_TextMesh == null)
                {
                    m_TextMesh = GetComponent<TextMesh>();
                }
                if (m_TextMesh != null && m_TextMaterial == null)
                {
                    MeshRenderer meshRenderer = m_TextMesh.GetComponent<MeshRenderer>();
                    if (meshRenderer != null)
                    {
                        m_TextMaterial = new Material(meshRenderer.sharedMaterial);
                        meshRenderer.material = m_TextMaterial;
                    }
                }
            }

            protected override void gameUpdate()
            {
                updateText();
            }
            protected override void gameFixedUpdate()
            {
                
            }
            public bool shouldUpdateText
            {
                get { return m_UpdateText; }
            }
            public void updateText()
            {

                if (isInteractive == true && m_UpdateText == true)
                {
                    if (m_UpdateText == true)
                    {
                        Debug.Log("Updating Text");
                        m_UpdateText = false;
                    }

                    BoxCollider boxCollider = GetComponent<BoxCollider>();
                    if (boxCollider != null)
                    {
                        if (Application.isPlaying == true)
                        {
                            Destroy(boxCollider);
                        }
                        else
                        {
                            DestroyImmediate(boxCollider);
                        }
                    }
                    boxCollider = gameObject.AddComponent<BoxCollider>();
                    boxCollider.isTrigger = true;
                }
            }
            public void clear()
            {
                m_UpdateText = false;
            }


            public string text
            {
                get { return m_TextMesh.text; }
                set
                {
                    if (value != m_TextMesh.text)
                    {
                        m_UpdateText = true;
                    }
                    m_TextMesh.text = value;
                }
            }
            public Font font
            {
                get { return m_TextMesh.font; }
                set { m_TextMesh.font = value; }
            }
            public FontStyle fontStyle
            {
                get { return m_TextMesh.fontStyle; }
                set { m_TextMesh.fontStyle = value; }
            }
            public int fontSize
            {
                get { return m_TextMesh.fontSize; }
                set { m_TextMesh.fontSize = value; }
            }
            public Color fontColor
            {
                get { return m_TextMaterial.color; }
                set { m_TextMaterial.color = value; }
            }



        }
    }
}