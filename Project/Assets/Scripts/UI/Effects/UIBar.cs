using UnityEngine;
using System.Collections;

namespace Gem
{

    public class UIBar : MonoBehaviour
    {
        private const float SCALE_MODIFIER = 5.0f;
        
      
        /// <summary>
        /// The name of the health bar whos position this uses to auto adjust
        /// </summary>
        [SerializeField]
        private string m_BarName = string.Empty;
        /// <summary>
        /// The mame of the unit whos health to monitor.
        /// </summary>
        [SerializeField]
        private string m_UnitName = string.Empty;
        [SerializeField]
        private UnitResourceType m_ResourceType = UnitResourceType.HEALTH;
        
        
        private Unit m_Unit;
        private Material m_BarMaterial = null;

        // Use this for initialization
        void OnEnable()
        {
            StartCoroutine(LateEnable());
        }
        IEnumerator LateEnable()
        {
            yield return new WaitForEndOfFrame();
            if (m_Unit == null)
            {
                m_Unit = UnitManager.GetUnit(m_UnitName);
            }
            UIImage resourceBar = UIManager.Find<UIImage>(m_BarName);
            if (resourceBar != null)
            {
                m_BarMaterial = resourceBar.renderer.material;
                if (m_BarMaterial != null && m_BarMaterial.shader.name == UIUtilities.SHADER_CUTOFF_TRANSPARENT)
                {
                    m_BarMaterial.SetFloat(UIUtilities.SHADER_CUTOFF_Y, 1.0f);
                }
            }

        }
        // Update is called once per frame
        void Update()
        {
            CalcOffset();
        }

        public void CalcOffset()
        {
            if(m_Unit != null)
            {
                float resource = 1.0f;
                switch(m_ResourceType)
                {
                    case UnitResourceType.HEALTH:
                        resource = m_Unit.health / m_Unit.maxHealth;
                        break;
                    case UnitResourceType.RESOURCE:
                        resource = m_Unit.resource / m_Unit.maxResource;
                        break;
                }
                
                if (m_BarMaterial != null && m_BarMaterial.shader.name == UIUtilities.SHADER_CUTOFF_TRANSPARENT)
                {
                    m_BarMaterial.SetFloat(UIUtilities.SHADER_CUTOFF_X, resource);
                }
            }
        }

        public Unit unit
        {
            get { return m_Unit; }
            set { m_Unit = value; }
        }
        public string unitName
        {
            get { return m_UnitName;}
        }
        public string healthBarName
        {
            get { return m_BarName; }
        }
    }
}