using UnityEngine;
using System.Collections.Generic;

namespace Gem
{
    public class PowerReceiver : MonoBehaviour
    {
        [SerializeField]
        private List<Wire> m_Wires = new List<Wire>();

        float m_CurrentPower = 0.0f;
        [SerializeField]
        float m_MaxPower = 5.0f;

        [SerializeField]
        float m_PowerUsage = 2.0f;

        private void Update()
        {
            ///Use all the power
            m_CurrentPower -= m_PowerUsage * Time.deltaTime;
            m_CurrentPower = Mathf.Clamp(m_CurrentPower, 0.0f, m_MaxPower);

            ///Get power from wires
            IEnumerator<Wire> enumerator = m_Wires.GetEnumerator();
            while(enumerator.MoveNext())
            {
                if(enumerator.Current == null)
                {
                    continue;
                }
                m_CurrentPower += enumerator.Current.currentFlow * Time.deltaTime;
            }
        }

        public void AddWire(Wire aWire)
        {
            if(aWire != null && !m_Wires.Contains(aWire))
            {
                m_Wires.Add(aWire);
            }
        }
        public void RemoveWire(Wire aWire)
        {
            if(aWire != null)
            {
                m_Wires.Remove(aWire);
            }
        }
        public float currentPower
        {
            get { return m_CurrentPower; }
            set { m_CurrentPower = value; }
        }
        public float maxPower
        {
            get { return m_MaxPower; }
            set { m_MaxPower = value; }
        }
        public float powerUsage
        {
            get { return m_PowerUsage; }
            set { m_PowerUsage = value; }
        }
        public bool isPowered
        {
            get { return m_CurrentPower > 0.0f; }
        }
            
    }
}