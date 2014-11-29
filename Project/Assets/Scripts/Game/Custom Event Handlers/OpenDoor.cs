using UnityEngine;
using System.Collections;

namespace Gem
{

    public class OpenDoor : MonoGameEventHandler, IGameListener
    {
        private enum DoorState
        {
            OPEN,
            CLOSED,
            OPENING,
            CLOSING
        }
        [SerializeField]
        private Vector3 m_ClosedPosition = Vector3.zero;
        [SerializeField]
        private Vector3 m_OpenPosition = Vector3.zero;
        [SerializeField]
        private float m_OpenSpeed = 1.0f;

        [SerializeField]
        string m_DoorName = string.Empty;
        [SerializeField]
        string m_RequiredItemName = string.Empty;

        private DoorState m_State = DoorState.CLOSED;
        private float m_CurrentTime = 0.0f;
        private Vector3 m_StartPosition = Vector3.zero;
        // Use this for initialization
        void Start()
        {
            m_StartPosition = transform.position;
            RegisterEvent(GameEventID.TRIGGER_AREA);
            Game.Register(this);
        }
        void OnDestroy()
        {
            UnregisterEvent(GameEventID.TRIGGER_AREA);
            Game.Unregister(this);
        }

        protected override void OnGameEvent(GameEventID aEventType)
        {
            if(aEventType == GameEventID.TRIGGER_AREA)
            {
                AreaTrigger trigger = eventData.sender as AreaTrigger;
                Unit unit = eventData.triggeringObject as Unit;
                if(unit == null || trigger == null)
                {
                    return;
                }
                UnitInventory inventory = unit.inventory;
                if(trigger.triggerName == m_DoorName && inventory != null && inventory.GetItem(m_RequiredItemName) != null)
                {
                    Open();
                }
            }
        }

        private void Update()
        {
            switch(m_State)
            {
                case DoorState.CLOSED:
                    transform.position = m_StartPosition + m_ClosedPosition;
                    break;
                case DoorState.OPEN:
                    transform.position = m_StartPosition + m_OpenPosition;
                    break;
                case DoorState.OPENING:
                    m_CurrentTime += Time.deltaTime * m_OpenSpeed;
                    if(m_CurrentTime > 1.0f)
                    {
                        m_State = DoorState.OPEN;
                        m_CurrentTime = 0.0f;
                    }
                    transform.position = Vector3.Lerp(m_StartPosition + m_ClosedPosition,m_StartPosition + m_OpenPosition, m_CurrentTime);
                    break;
                case DoorState.CLOSING:
                    m_CurrentTime += Time.deltaTime * m_OpenSpeed;
                    if(m_CurrentTime > 0.0f)
                    {
                        m_State = DoorState.CLOSED;
                        m_CurrentTime = 0.0f;
                    }
                    transform.position = Vector3.Lerp(m_StartPosition + m_OpenPosition, m_StartPosition + m_ClosedPosition, m_CurrentTime);
                    break;

            }
        }

        public void Open()
        {
            if (m_State != DoorState.OPEN)
            {
                m_State = DoorState.OPENING;
            }
        }
        public void Close()
        {
            if(m_State != DoorState.CLOSED)
            {
                m_State = DoorState.CLOSING;
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(m_OpenPosition, m_ClosedPosition);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(m_OpenPosition, 0.05f);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(m_ClosedPosition, 0.05f);
        }
#endif

        public void OnGamePaused()
        {
            
        }

        public void OnGameUnpaused()
        {
            
        }

        public void OnGameReset()
        {
            m_State = DoorState.CLOSED;
        }
    }
}

