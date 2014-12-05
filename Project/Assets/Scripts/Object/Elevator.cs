using UnityEngine;
using System.Collections;

namespace Gem
{

    public enum ElevatorState
    {
        DOWN,
        MOVING_DOWN,
        MOVING_UP,
        UP,
        NONE
    }

    public class Elevator : MonoBehaviour
    {


#if UNITY_EDITOR
        [SerializeField]
        DebugMode m_DebugMode = DebugMode.ON_SELECTED;
#endif
        /// <summary>
        /// The position the door goes when its closed.
        /// </summary>
        [SerializeField]
        private Vector3 m_DownPosition = Vector3.zero;
        /// <summary>
        /// The position the door goes when its open
        /// </summary>
        [SerializeField]
        private Vector3 m_UpPosition = Vector3.zero;
        /// <summary>
        /// The speed at which the door opens at.
        /// </summary>
        [SerializeField]
        private float m_MovementSpeed = 1.0f;
        /// <summary>
        /// A automatic timer which moves the elevator up and down.
        /// </summary>
        [SerializeField]
        private float m_AutoTimer = 10.0f;
        private float m_Timer = 0.0f;

        private Vector3 m_StartPosition = Vector3.zero;
        private ElevatorState m_CurrentState = ElevatorState.DOWN;
        private float m_CurrentTime = 0.0f;
        private PowerReceiver m_Receiver = null;
        // Use this for initialization
        void Start()
        {
            m_Receiver = GetComponent<PowerReceiver>();
            m_StartPosition = transform.position;
            m_Timer = m_AutoTimer;
        }

        // Update is called once per frame
        void Update()
        {
            if(m_AutoTimer > 0.0f)
            {
                m_Timer -= Time.deltaTime;
                if(m_Timer < 0.0f)
                {
                    m_Timer = m_AutoTimer;
                    if(m_CurrentState == ElevatorState.MOVING_UP || m_CurrentState == ElevatorState.UP)
                    {
                        MoveDown();
                    }
                    else if(m_CurrentState == ElevatorState.MOVING_DOWN || m_CurrentState == ElevatorState.DOWN)
                    {
                        MoveUp();
                    }
                }
            }
            switch(m_CurrentState)
            {
                case ElevatorState.DOWN:
                    transform.position = m_StartPosition + m_DownPosition;
                    break;
                case ElevatorState.UP:
                    transform.position =  m_StartPosition + m_UpPosition;
                    break;
                case ElevatorState.MOVING_DOWN:
                    m_CurrentTime -= Time.deltaTime * m_MovementSpeed;
                    if(m_CurrentTime < 0.0f)
                    {
                        m_CurrentState = ElevatorState.DOWN;
                        m_CurrentTime = 0.0f;
                        break;
                    }
                    transform.position = Vector3.Lerp(m_StartPosition + m_DownPosition,m_StartPosition + m_UpPosition , m_CurrentTime);
                    break;
                case ElevatorState.MOVING_UP:
                    m_CurrentTime += Time.deltaTime * m_MovementSpeed;
                    if(m_CurrentTime > 1.0f)
                    {
                        m_CurrentState = ElevatorState.UP;
                        m_CurrentTime = 1.0f;
                        break;
                    }
                    transform.position = Vector3.Lerp(m_StartPosition + m_DownPosition, m_StartPosition + m_UpPosition, m_CurrentTime);
                    break;
            }
        }

        public void MoveUp()
        {
            if (m_Receiver != null && m_Receiver.isPowered == false)
            {
                return;
            }
            if(m_CurrentState != ElevatorState.UP)
            {
                m_CurrentState = ElevatorState.MOVING_UP;
            }
        }
        public void MoveDown()
        {
            if (m_Receiver != null && m_Receiver.isPowered == false)
            {
                return;
            }
            if(m_CurrentState != ElevatorState.DOWN)
            {
                m_CurrentState = ElevatorState.MOVING_DOWN;
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if(m_DebugMode == DebugMode.ON)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position + m_DownPosition, transform.position + m_UpPosition);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position + m_DownPosition, 0.05f);
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(transform.position + m_UpPosition, 0.05f);
            }
            
        }

        void OnDrawGizmosSelected()
        {
            if (m_DebugMode == DebugMode.ON_SELECTED)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position + m_DownPosition, transform.position + m_UpPosition);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position + m_DownPosition, 0.05f);
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(transform.position + m_UpPosition , 0.05f);
            }
        }
#endif


    }
}