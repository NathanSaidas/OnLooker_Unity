using UnityEngine;
using System.Collections;

namespace Gem
{

    public class Bounce : MonoBehaviour
    {
        [SerializeField]
        private Vector3 m_PointA = Vector3.zero;
        [SerializeField]
        private Vector3 m_PointB = Vector3.zero;
        [SerializeField]
        private float m_Speed = 1.0f;


        private Vector3 m_StartPoint = Vector3.zero;
        private bool m_DirectionA = true;
        private float m_CurrentTime = 0.0f;
        // Use this for initialization
        void Start()
        {
            m_StartPoint = transform.position;
        }
        void OnDisable()
        {
            transform.position = m_StartPoint;
        }

        // Update is called once per frame
        void Update()
        {
            m_CurrentTime += Time.deltaTime * m_Speed;
            if (m_CurrentTime > 1.0f)
            {
                m_CurrentTime = 0.0f;
                m_DirectionA = !m_DirectionA;
            }
            if (m_DirectionA == true)
            {
                transform.position = Vector3.Lerp(m_StartPoint + m_PointB, m_StartPoint + m_PointA, m_CurrentTime);
            }
            else
            {
                transform.position = Vector3.Lerp(m_StartPoint + m_PointA, m_StartPoint + m_PointB, m_CurrentTime);
            }
        }
        
#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(m_PointA, m_PointB);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(m_PointA, 0.05f);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(m_PointB, 0.05f);
        }
#endif

        public Vector3 pointA
        {
            get { return m_PointA; }
            set { m_PointA = value; }
        }
        public Vector3 pointB
        {
            get { return m_PointB; }
            set { m_PointB = value; }
        }
        public float speed
        {
            get { return m_Speed; }
            set { m_Speed = value; }
        }
    }

}