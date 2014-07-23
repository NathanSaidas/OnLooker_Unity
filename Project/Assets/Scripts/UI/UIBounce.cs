using UnityEngine;
using System.Collections;

namespace OnLooker
{
    namespace UI
    {
        public class UIBounce : MonoBehaviour
        {
            [SerializeField]
            private UIControl m_UIControl;


            [SerializeField]
            private float m_StartY = 0.0f;
            [SerializeField]
            private float m_Distance = 1.0f;
            [SerializeField]
            private float m_Speed = 1.0f;
         
            // Use this for initialization
            void Start()
            {
                if (m_UIControl == null)
                {
                    m_UIControl = GetComponent<UIControl>();
                }
                if (m_UIControl != null)
                {
                    m_StartY = m_UIControl.offsetPosition.y;
                }
            }

            // Update is called once per frame
            void LateUpdate()
            {
                if (m_UIControl != null)
                {
                    Vector3 pos = m_UIControl.offsetPosition;
                    pos.y += m_Speed * Time.deltaTime;
                    if (pos.y > m_StartY + m_Distance)
                    {
                        pos.y = m_StartY + m_Distance;
                        m_Speed = -m_Speed;
                    }
                    else if (pos.y < m_StartY - m_Distance)
                    {
                        pos.y = m_StartY - m_Distance;
                        m_Speed = -m_Speed;
                    }
                    m_UIControl.offsetPosition = pos;
                }
            }

            
        }
    }
}