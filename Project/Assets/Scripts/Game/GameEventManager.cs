using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#region CHANGE LOG
/* October,31,2014 - Nathan Hanlan, Added and implemented the GameEvent Manager.
 * 
 */
#endregion

namespace Gem
{

    public class GameEventManager : MonoBehaviour
    {
        #region SINGLETON
        /// <summary>
        /// A field which holds the instance of the game event manager.
        /// </summary>
        private static GameEventManager s_Instance = null;
        /// <summary>
        /// An accessor to the instance of the game event manager.
        /// </summary>
        private static GameEventManager instance
        {
            get { if (s_Instance == null) { CreateInstance(); } return s_Instance; }
        }
        /// <summary>
        /// Creates an instance of the GameEventManager if it was missing.
        /// </summary>
        private static void CreateInstance()
        {
            GameObject persistant = GameObject.Find(Game.PERSISTANT_GAME_OBJECT_NAME);
            if(persistant == null)
            {
                persistant = new GameObject(Game.PERSISTANT_GAME_OBJECT_NAME);
                persistant.transform.position = Vector3.zero;
                persistant.transform.rotation = Quaternion.identity;
            }
            s_Instance = persistant.GetComponent<GameEventManager>();
            if(s_Instance == null)
            {
                s_Instance = persistant.AddComponent<GameEventManager>();
            }
        }
        /// <summary>
        /// Sets the instance of the game event manager to the instance given.
        /// </summary>
        /// <param name="aInstance">The instance to make singleton</param>
        /// <returns></returns>
        private static bool SetInstance(GameEventManager aInstance)
        {
            if(s_Instance != null)
            {
                return false;
            }
            s_Instance = aInstance;
            return true;
        }
        /// <summary>
        /// Removes the instance of the game event manager if the instance being destroyed is the the same as the singleton.
        /// </summary>
        /// <param name="aInstance"></param>
        private static void DestroyInstance(GameEventManager aInstance)
        {
            if(s_Instance == aInstance)
            {
                s_Instance = null;
            }
        }
        #endregion

        #region FIELDS
        /// <summary>
        /// Determines if the game event thread is running or not.
        /// </summary>
        private bool m_EventThreadRunning = false;
        /// <summary>
        /// A list of events to process.
        /// </summary>
        private Queue<GameEventData> m_EventQueue = new Queue<GameEventData>();
        #region Event Listeners
        /// All of the event listener collections
        /// <summary>
        /// Listeners listening on game events.
        /// </summary>
        private HashSet<IGameEventReceiver> m_GameEventListener = new HashSet<IGameEventReceiver>();
        #endregion
        #endregion

        void Start()
        {
            if(!SetInstance(this))
            {
                Destroy(this);
                return;
            }
            DontDestroyOnLoad(gameObject);
            StartEventThread();
        }
        void OnDestroy()
        {
            DestroyInstance(this);
            StopEventThreadImmediate();
        }
        
        public static void InvokeEvent(GameEventData aEvent)
        {
            instance.m_EventQueue.Enqueue(aEvent);  
        }
        public static void RegisterEventListener(GameEventType aType, IGameEventReceiver aListener)
        {
            if(aListener == null)
            {
                return;
            }

            switch(aType)
            {
                case GameEventType.GAME:
                    instance.m_GameEventListener.Add(aListener);
                    break;
            }
        }
        public static void UnregisterEventListener(GameEventType aType, IGameEventReceiver aListener)
        {
            if(aListener == null)
            {
                switch(aType)
                {
                    case GameEventType.GAME:
                        instance.m_GameEventListener.Add(aListener);
                        break;
                }
            }
        }


        #region Event Thread
        /// <summary>
        /// Starts the event thread. An event thread cannot be started if its already running
        /// </summary>
        public static void StartEventThread()
        {
            if(instance.m_EventThreadRunning == true)
            {
                return;
            }
            instance.m_EventThreadRunning = true;
            instance.StartCoroutine(instance.EventThread());
        }
        /// <summary>
        /// Stops the event thread. Waits for it to finish then doesnt run again.
        /// </summary>
        public static void StopEventThread()
        {
            instance.m_EventThreadRunning = false;
        }
        /// <summary>
        /// Forces the event thread to stop immediately.
        /// </summary>
        public static void StopEventThreadImmediate()
        {
            instance.m_EventThreadRunning = false;
            instance.StopCoroutine(instance.EventThread());
        }
        /// <summary>
        /// A coroutine which processes the events and then waits until next frame
        /// </summary>
        /// <returns></returns>
        private IEnumerator EventThread()
        {
            while(m_EventThreadRunning)
            {
                while (m_EventQueue.Count > 0)
                {
                    GameEventData eventData = m_EventQueue.Dequeue();
                    ProcessGameEvent(ref eventData);
                }
                yield return new WaitForEndOfFrame();
            }
        }
        /// <summary>
        /// Process each game event
        /// </summary>
        /// <param name="aEvent"></param>
        private void ProcessGameEvent(ref GameEventData aEvent)
        {
            //Check the event type and iterate through the collection and invoke the ReceiveEvent method.
            switch(aEvent.eventType)
            {
                case GameEventType.GAME:
                    {
                        IEnumerator<IGameEventReceiver> receivers = m_GameEventListener.GetEnumerator();
                        while(receivers.MoveNext())
                        {
                            if(receivers.Current == null)
                            {
                                continue;
                            }
                            receivers.Current.ReceiveEvent(ref aEvent);
                        }
                    }
                    break;
            }

        }
        #endregion
    }
}