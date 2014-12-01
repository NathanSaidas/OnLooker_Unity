using UnityEngine;
using System.Collections;
using System.Collections.Generic;   
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Gem
{

    public class Wire : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("CONTEXT/Wire/Create Wire")]
        private static void OnCreateWire(MenuCommand aCommand)
        {
            Wire wire = aCommand.context as Wire;
            if(wire == null || wire.next != null)
            {
                return;
            }
            Vector3 position = wire.transform.position + wire.transform.forward * wire.transform.localScale.z;
            GameObject newWireGO = (GameObject)Instantiate(wire.gameObject, position, wire.transform.rotation);
            newWireGO.name = newWireGO.name.Remove(newWireGO.name.Length - 7);
            Wire newWire = newWireGO.GetComponent<Wire>();
            if(newWire != null)
            {
                wire.next = newWire;
                newWire.previous = wire;
                EditorUtility.SetDirty(newWire);
                EditorUtility.SetDirty(wire);
            }
        }
        [MenuItem("CONTEXT/Wire/Select All")]
        private static void OnSelectAll(MenuCommand aCommand)
        {
            Wire wire = aCommand.context as Wire;
            if(wire == null)
            {
                return;
            }
            List<GameObject> wires = new List<GameObject>();
            wires.Add(wire.gameObject);
            Wire currentWire = wire.next;
            while(currentWire != null)
            {
                wires.Add(currentWire.gameObject);
                currentWire = currentWire.next;
            }
            Selection.objects = wires.ToArray();
        }
#endif

        [SerializeField]
        private Color m_ActiveColor = Color.green;
        [SerializeField]
        private Color m_InactiveColor = Color.gray;
        [SerializeField]
        private Wire m_Previous = null;
        [SerializeField]
        private Wire m_Next = null;

        [SerializeField]
        private float m_MaxPower = 100.0f;
        [SerializeField]
        private float m_CurrentPower = 0.0f;
        [SerializeField]
        private float m_DecayPower = 0.05f;

        private float m_CurrentFlow = 0.0f;

        private MeshRenderer m_MeshRenderer = null;
        // Use this for initialization
        void Start()
        {
            m_MeshRenderer = GetComponent<MeshRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            if(Game.isPaused)
            {
                return;
            }
            m_CurrentPower -= m_DecayPower;
            if (m_CurrentPower < 0.0f)
            {
                m_CurrentPower = 0.0f;
            }

            if(isAlive && m_MeshRenderer != null)
            {
                m_MeshRenderer.material.color = m_ActiveColor;
            }
            else if(!isAlive && m_MeshRenderer != null)
            {
                m_MeshRenderer.material.color = m_InactiveColor;
            }
        }

        public void FlowCurrent(float aCurrent, int aLength)
        {
            if(aLength <= 0)
            {
                return;
            }
            m_CurrentPower += aCurrent * Time.deltaTime;
            m_CurrentFlow = aCurrent;
            if(m_CurrentPower > m_MaxPower)
            {
                m_CurrentPower = m_MaxPower;
            }
            aLength--;
            if(next != null && next != this)
            {
                next.FlowCurrent(aCurrent * 0.95f,aLength);
            }
        }
        public void EndFlow(int aLength)
        {
            if(aLength <= 0)
            {
                return;
            }
            m_CurrentFlow = 0.0f;
            aLength--;
            if(next != null && next != this)
            {
                next.EndFlow(aLength);
            }
        }

        public Wire next
        {
            get { return m_Next; }
            set { m_Next = value; }
        }
        public Wire previous
        {
            get { return m_Previous; }
            set { m_Previous = value; }
        }
        public float maxPower
        {
            get { return m_MaxPower; }
            set { m_MaxPower = value; }
        }
        public float currentPower
        {
            get { return m_CurrentPower; }
            set { m_CurrentPower = value; }
        }
        public float currentFlow
        {
            get { return m_CurrentFlow; }
            set { m_CurrentFlow = value; }
        }
        public bool isAlive
        {
            get { return m_CurrentPower > 0.0f; }
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if(next != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(next.transform.position, 0.25f);
            }
            if(previous != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(previous.transform.position, 0.25f);
            }
        }
#endif
    }
}