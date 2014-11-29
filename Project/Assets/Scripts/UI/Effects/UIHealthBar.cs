using UnityEngine;
using System.Collections;

namespace Gem
{

    public class UIHealthBar : MonoBehaviour
    {
        private const float SCALE_MODIFIER = 5.0f;
        
        [SerializeField]
        private UIToggle m_HealthBarToggle = null;
        [SerializeField]
        private string m_HealthBarToggleSearchName = string.Empty;
        [SerializeField]
        private float m_XPosition = 0.0f;
        [SerializeField]
        private float m_XOffset = 0.0f;
        [SerializeField]
        private Unit m_Unit;
        [SerializeField]
        private string m_UnitName = string.Empty;
        // Use this for initialization
        void OnEnable()
        {
            StartCoroutine(LateEnable());
        }
        IEnumerator LateEnable()
        {
            yield return Game.WAIT_QUATER;
            if (m_Unit == null)
            {
                m_Unit = UnitManager.GetUnit(m_UnitName);
            }
            if(m_HealthBarToggle == null)
            {
                m_HealthBarToggle = UIManager.Find(m_HealthBarToggleSearchName);
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
                float health = m_Unit.health / m_Unit.maxHealth;
                m_XOffset = (1 - health) * SCALE_MODIFIER;
                if (m_HealthBarToggle != null)
                {
                    Vector3 position = m_HealthBarToggle.transform.position;
                    position.x = m_XPosition - m_XOffset;
                    m_HealthBarToggle.transform.position = position;
                    m_HealthBarToggle.transform.localScale = new Vector3(health, 1.0f, 1.0f);
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
            set { m_UnitName = value; }
        }
        public float xPosition
        {
            get { return m_XPosition; }
            set { m_XPosition = value; }
        }
    }
}