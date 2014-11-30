using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gem
{
    public enum DoorState
    {
        OPEN,
        CLOSED,
        OPENING,
        CLOSING,
        NONE
    }

    public class OpenDoor : MonoGameEventHandler, IGameListener
    {
        
        /// <summary>
        /// The position the door goes when its closed.
        /// </summary>
        [SerializeField]
        private Vector3 m_ClosedPosition = Vector3.zero;
        /// <summary>
        /// The position the door goes when its open
        /// </summary>
        [SerializeField]
        private Vector3 m_OpenPosition = Vector3.zero;
        /// <summary>
        /// The speed at which the door opens at.
        /// </summary>
        [SerializeField]
        private float m_OpenSpeed = 1.0f;
        /// <summary>
        /// The name of the door.
        /// </summary>
        [SerializeField]
        string m_DoorName = string.Empty;
        /// <summary>
        /// The item needed to open the door. This can be empty to not require any items.
        /// </summary>
        [SerializeField]
        string m_RequiredItemName = string.Empty;

        [SerializeField]
        bool m_RequirePower = false;



        private DoorState m_State = DoorState.CLOSED;
        private float m_CurrentTime = 0.0f;
        private Vector3 m_StartPosition = Vector3.zero;
        private PowerReceiver m_Receiver = null;

        /// <summary>
        /// The time to wait before closing the door.
        /// </summary>
        [SerializeField]
        private float m_MaxTime = 1.0f;

        private List<Unit> m_TriggeringUnits = new List<Unit>();
        private float m_CloseTime = 0.0f;

        // Use this for initialization
        void Start()
        {
            m_Receiver = GetComponent<PowerReceiver>();
            m_StartPosition = transform.position;
            RegisterEvent(GameEventID.TRIGGER_AREA);
            RegisterEvent(GameEventID.TRIGGER_AREA_EXIT);
            Game.Register(this);
        }
        void OnDestroy()
        {
            UnregisterEvent(GameEventID.TRIGGER_AREA);
            UnregisterEvent(GameEventID.TRIGGER_AREA_EXIT);
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
                m_TriggeringUnits.Add(unit);
                UnitInventory inventory = unit.inventory;
                if(trigger.triggerName == m_DoorName)
                {
                    if(m_RequiredItemName.Length > 0)
                    {
                        if(inventory != null && inventory.GetItem(m_RequiredItemName) != null)
                        {
                            Open();
                        }
                    }
                    else
                    {
                        Open();
                    }
                }
            }
            else if(aEventType == GameEventID.TRIGGER_AREA_EXIT)
            {
                AreaTrigger trigger = eventData.sender as AreaTrigger;
                Unit unit = eventData.triggeringObject as Unit;
                if (unit == null || trigger == null)
                {
                    return;
                }
                m_TriggeringUnits.Remove(unit);
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
                        m_CurrentTime = 1.0f;
                        break;
                    }
                    transform.position = Vector3.Lerp(m_StartPosition + m_ClosedPosition,m_StartPosition + m_OpenPosition, m_CurrentTime);
                    break;
                case DoorState.CLOSING:
                    m_CurrentTime -= Time.deltaTime * m_OpenSpeed;
                    if(m_CurrentTime < 0.0f)
                    {
                        m_State = DoorState.CLOSED;
                        m_CurrentTime = 0.0f;
                        break;
                    }
                    transform.position = Vector3.Lerp(m_StartPosition + m_ClosedPosition, m_StartPosition + m_OpenPosition, m_CurrentTime);
                    break;
            }

            if (m_TriggeringUnits.Count > 0)
            {
                m_CloseTime = m_MaxTime;
            }
            else
            {
                m_CloseTime -= Time.deltaTime;
            }
            m_CloseTime = Mathf.Clamp(m_CloseTime, 0.0f, m_MaxTime);

            if (m_CloseTime <= 0.0f && m_State != DoorState.OPENING)
            {
                Close();
            }

        }

        public void Open()
        {
            if(m_RequirePower == true)
            {
                if (m_Receiver != null && m_Receiver.isPowered == false)
                {
                    return;
                }
            }
            if (m_State != DoorState.OPEN)
            {
                m_State = DoorState.OPENING;
            }
        }
        public void Close()
        {
            if (m_RequirePower == true)
            {
                if (m_Receiver != null && m_Receiver.isPowered == false)
                {
                    return;
                }
            }
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

