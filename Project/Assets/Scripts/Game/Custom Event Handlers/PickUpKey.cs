using UnityEngine;
using System.Collections;

namespace Gem
{

    public class PickUpKey : MonoGameEventHandler, IGameListener
    {
        [SerializeField]
        private string m_KeyPickUpString = "Key_PickUp";
        private AreaTrigger m_Trigger = null;
        // Use this for initialization
        void Start()
        {
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
            DebugUtils.Log(aEventType);
            if(aEventType == GameEventID.TRIGGER_AREA)
            {
                AreaTrigger areaTrigger = eventData.sender as AreaTrigger;
                Unit unit = eventData.triggeringObject as Unit;
                if(unit == null)
                {
                    return;
                }
                UnitInventory inventory = unit.inventory;
                if(areaTrigger != null && areaTrigger.triggerName == m_KeyPickUpString && inventory != null )
                {
                    areaTrigger.gameObject.SetActive(false);
                    m_Trigger = areaTrigger;
                    PlayerPickUpKey(inventory);
                }
            }
        }

        void PlayerPickUpKey(UnitInventory aInventory)
        {
            aInventory.AddItem(ItemDatabase.QueryItem(ItemType.SILVER_ACCESS_CARD));
        }

        public void OnGamePaused()
        {
            
        }

        public void OnGameUnpaused()
        {
            
        }

        public void OnGameReset()
        {
            if(m_Trigger != null)
            {
                m_Trigger.gameObject.SetActive(true);
            }
        }
    }


}