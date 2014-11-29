using UnityEngine;
using System.Collections;

namespace Gem
{

    public class GameWinEvent : MonoGameEventHandler
    {
        [SerializeField]
        private string m_WinTrigger = "Win_Trigger";

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
            if (aEventType == GameEventID.TRIGGER_AREA)
            {
                AreaTrigger trigger = eventData.sender as AreaTrigger;
                if(trigger != null && trigger.triggerName == m_WinTrigger)
                {
                    Game.LoadLevel(Game.MENU_SCENE);
                    DebugUtils.Log("Winner");
                }
            }
        }
    }
}