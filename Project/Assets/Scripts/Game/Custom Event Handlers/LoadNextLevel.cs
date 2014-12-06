using UnityEngine;
using System.Collections;

namespace Gem
{


    public class LoadNextLevel : MonoGameEventHandler
    {
        [SerializeField]
        private string m_EventName = string.Empty;
        [SerializeField]
        private string m_LevelName = string.Empty;

        void Start()
        {
            RegisterEvent(GameEventID.TRIGGER_AREA);
        }
        void OnDestroy()
        {
            UnregisterEvent(GameEventID.TRIGGER_AREA);
        }
        protected override void OnGameEvent(GameEventID aEventType)
        {
            if(aEventType == GameEventID.TRIGGER_AREA)
            {
                AreaTrigger trigger = eventData.sender as AreaTrigger;
                if(trigger != null && trigger.triggerName == m_EventName)
                {
                    Game.LoadLevel(m_LevelName);
                }
            }
        }
    }
}