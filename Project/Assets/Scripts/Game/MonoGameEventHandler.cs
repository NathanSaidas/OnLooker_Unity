using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

#region CHANGE LOG
/* October,31,2014 - Nathan Hanlan, Added and implemented the MonoGameEventHandler Class. Missing Game Register / Unregister Link
 * 
 */
#endregion
namespace Gem
{

    public class MonoGameEventHandler : MonoBehaviour , IGameEventReceiver
    {
        /// <summary>
        /// A list of currently registered events
        /// </summary>
        private List<GameEventID> m_RegisteredEvents = new List<GameEventID>();
        private int m_GameEventsRegistered = 0;
        private int m_UnitEventsRegistered = 0;
        private int m_TriggerEventsRegistered = 0;
        /// <summary>
        /// An event data structure.
        /// </summary>
        private GameEventData m_EventData = GameEventData.empty;

        /// <summary>
        /// Register an event with the Game Event Manager
        /// </summary>
        /// <param name="aEventID"></param>
        protected void RegisterEvent(GameEventID aEventID)
        {
            if(m_RegisteredEvents.Contains(aEventID))
            {
                return;
            }
            ///Add the event
            m_RegisteredEvents.Add(aEventID);
            switch(aEventID)
            {
                case GameEventID.GAME_LEVEL_LOAD_BEGIN:
                case GameEventID.GAME_LEVEL_LOAD_FINISH:
                case GameEventID.GAME_LEVEL_UNLOAD_BEGIN:
                case GameEventID.GAME_LEVEL_UNLOAD_FINISH:
                case GameEventID.GAME_LOAD:
                case GameEventID.GAME_PAUSED:
                case GameEventID.GAME_SAVE:
                case GameEventID.GAME_UNPAUSED:
                    //Register a game event type
                    if(m_GameEventsRegistered == 0)
                    {
                        GameEventManager.RegisterEventListener(GameEventType.GAME, this);
                    }
                    m_GameEventsRegistered++;
                    break;
                case GameEventID.UNIT_KILLED:
                case GameEventID.UNIT_REVIVED:
                case GameEventID.UNIT_SPAWNED:
                    if (m_UnitEventsRegistered == 0)
                    {
                        GameEventManager.RegisterEventListener(GameEventType.UNIT, this);
                    }
                    m_UnitEventsRegistered++;
                    break;
                case GameEventID.TRIGGER_AREA:
                case GameEventID.TRIGGER_AREA_EXIT:
                    if(m_TriggerEventsRegistered == 0)
                    {
                        GameEventManager.RegisterEventListener(GameEventType.TRIGGER, this);
                    }
                    m_TriggerEventsRegistered++;
                    break;
            }
            
        }
        /// <summary>
        /// Unregister an event with the Game Event Manager
        /// </summary>
        /// <param name="aEventID"></param>
        protected void UnregisterEvent(GameEventID aEventID)
        {
            if(!m_RegisteredEvents.Contains(aEventID))
            {
                return;
            }
            m_RegisteredEvents.Remove(aEventID);
            switch (aEventID)
            {
                case GameEventID.GAME_LEVEL_LOAD_BEGIN:
                case GameEventID.GAME_LEVEL_LOAD_FINISH:
                case GameEventID.GAME_LEVEL_UNLOAD_BEGIN:
                case GameEventID.GAME_LEVEL_UNLOAD_FINISH:
                case GameEventID.GAME_LOAD:
                case GameEventID.GAME_PAUSED:
                case GameEventID.GAME_SAVE:
                case GameEventID.GAME_UNPAUSED:
                    //Register a game event type
                    if (m_GameEventsRegistered == 1)
                    {
                        GameEventManager.UnregisterEventListener(GameEventType.GAME, this);
                    }
                    m_GameEventsRegistered--;
                    break;
                case GameEventID.UNIT_KILLED:
                case GameEventID.UNIT_REVIVED:
                case GameEventID.UNIT_SPAWNED:
                    if(m_GameEventsRegistered == 1)
                    {
                        GameEventManager.UnregisterEventListener(GameEventType.UNIT, this);
                    }
                    m_GameEventsRegistered--;
                    break;
                case GameEventID.TRIGGER_AREA:
                case GameEventID.TRIGGER_AREA_EXIT:
                    {
                        GameEventManager.UnregisterEventListener(GameEventType.TRIGGER, this);
                    }
                    m_TriggerEventsRegistered--;
                    break;
            }
        }
        /// <summary>
        /// Filters the event and calls OnGameEvent if the event was registered.
        /// </summary>
        /// <param name="aType"></param>
        protected void FilterEvent(GameEventType aType)
        {
            if(m_RegisteredEvents.Contains(eventData.eventSubType))
            {
                OnGameEvent(eventData.eventSubType);
            }
        }
        
        /// <summary>
        /// Game event manager invokes this event to send event data.
        /// Override this method to intercept the call and ignore filtering.
        /// </summary>
        /// <param name="aData"></param>
        public void ReceiveEvent(ref GameEventData aData)
        {
            eventData = aData;
            FilterEvent(aData.eventType);
            eventData = GameEventData.empty;
        }

        /// <summary>
        /// This method gets called to notify the EventHandler of the event type.
        /// </summary>
        /// <param name="aEventType"></param>
        protected virtual void OnGameEvent(GameEventID aEventType)
        {

        }

        /// <summary>
        /// Get the event data sent in from the event.
        /// </summary>
        protected GameEventData eventData
        {
            get { return m_EventData; }
            set { m_EventData = value; }
        }
    }

}