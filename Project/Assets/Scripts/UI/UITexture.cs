using UnityEngine;
using System;
using System.Collections;

namespace OnLooker
{
    namespace UI
    {
        [Serializable]
        public class UITexture : UIToggle
        {
            [SerializeField()]
            private Material m_TextureMaterial;

            // Use this for initialization
            void Start()
            {
                init();
            }

            public void init()
            {
                MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
                if (meshRenderer != null && m_TextureMaterial == null)
                {
                    m_TextureMaterial = new Material(meshRenderer.sharedMaterial);
                    meshRenderer.material = m_TextureMaterial;
                }
            }
            public Texture texture
            {
                get { return m_TextureMaterial.mainTexture; }
                set { m_TextureMaterial.mainTexture = value; }
            }
            public Color color
            {
                get { return m_TextureMaterial.color; }
                set { m_TextureMaterial.color = value; }
            }



        }
    }
}