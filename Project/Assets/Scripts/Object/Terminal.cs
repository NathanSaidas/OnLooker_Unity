using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gem
{
    public class Terminal : Interactive
    {
        [SerializeField]
        private Color m_ActiveColor = Color.green;
        [SerializeField]
        private Color m_InactiveColor = Color.gray;

        [SerializeField]
        private List<MeshRenderer> m_Renderers = new List<MeshRenderer>();

        private bool m_IsActive = false;

        /// <summary>
        /// How much energy is sent per second
        /// </summary>
        [SerializeField]
        private float m_Current = 2.0f;
        /// <summary>
        /// How many wires the energy can go before it expires.
        /// </summary>
        [SerializeField]
        private int m_FlowLength = 10;
        [SerializeField]
        private Wire m_Connection = null;
        // Use this for initialization
        void Start()
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                m_Renderers.Add(meshRenderer);
            }
            SetRendererColor();
        }

        void Update()
        {
            if(m_IsActive && m_Connection != null)
            {
                m_Connection.FlowCurrent(m_Current, m_FlowLength);
            }
            
        }


        public override void OnUse()
        {
            if(m_IsActive)
            {
                SetInactive();
            }
            else
            {
                SetActive();
            }
        }
        

        void SetRendererColor()
        {
            IEnumerator<MeshRenderer> renderers = m_Renderers.GetEnumerator();
            if (isActive)
            {
                while (renderers.MoveNext())
                {
                    if (renderers.Current == null)
                    {
                        continue;
                    }
                    renderers.Current.material.color = m_ActiveColor;
                }
            }
            else
            {
                while (renderers.MoveNext())
                {
                    if (renderers.Current == null)
                    {
                        continue;
                    }
                    renderers.Current.material.color = m_InactiveColor;
                }
            }
        }

        public void SetActive()
        {
            m_IsActive = true;
            SetRendererColor();
        }
        public void SetInactive()
        {
            m_IsActive = false;
            SetRendererColor();
            if(m_Connection != null)
            {
                m_Connection.EndFlow(m_FlowLength);
            }
        }

        public bool isActive
        {
            get { return m_IsActive; }
            set { m_IsActive = value; }
        }
        public float current
        {
            get { return m_Current; }
            set { m_Current = value; }
        }
        public int flowLength
        {
            get { return m_FlowLength; }
            set { m_FlowLength = value; }
        }
    }
}