using UnityEngine;
using System.Collections.Generic;

namespace EndevGame
{
    //Attach this to the player
    /// <summary>
    /// Character interaction drives the interation between the player and Interactive Objects.
    /// </summary>
    public class CharacterInteraction : CharacterComponent
    {
        /// <summary>
        /// This represents the list of interactive objects the Character is within
        /// </summary>
        [SerializeField]
        private List<Interactive> m_TriggeringObjects = new List<Interactive>();

        /// <summary>
        /// This represents the object the character is looking at.
        /// </summary>
        [SerializeField]
        private Interactive m_ObjectInFocus = null;
        /// <summary>
        /// This represents the object the character is using.
        /// </summary>
        [SerializeField]
        private Interactive m_ObjectInUse = null;

        /// <summary>
        /// The distance to search for an interaction infront of the player.
        /// </summary>
        [SerializeField]
        private float m_SearchDistance = 5.0f;



        void Start()
        {
            base.init();
        }

        /// <summary>
        /// Detects a trigger event and stores the object into the interactive component list.
        /// </summary>
        /// <param name="aCollider"></param>
        private void OnTriggerEnter(Collider aCollider)
        {
            Interactive triggeringObject = aCollider.GetComponent<Interactive>();
            if(triggeringObject != null)
            {
                GameEventArgs eventArgs = new GameEventArgs(GameEventID.INTERACTION_PLAYER_ENTER);
                eventArgs.sender = this;
                eventArgs.triggeringObject = triggeringObject;
                GameManager.triggerEvent(eventArgs.eventID, eventArgs);
                triggeringObject.onPlayerEnter(this);
                m_TriggeringObjects.Add(triggeringObject);
            }
        }

        /// <summary>
        /// Detects a trigger event and removes the object from the interactive component list
        /// </summary>
        /// <param name="aCollider"></param>
        private void OnTriggerExit(Collider aCollider)
        {
            Interactive triggeringObject = aCollider.GetComponent<Interactive>();
            if(triggeringObject != null)
            {
                GameEventArgs eventArgs = new GameEventArgs(GameEventID.INTERACTION_PLAYER_EXIT);
                eventArgs.sender = this;
                eventArgs.triggeringObject = triggeringObject;
                GameManager.triggerEvent(eventArgs.eventID, eventArgs);
                triggeringObject.onPlayerExit(this);
                m_TriggeringObjects.Remove(triggeringObject);
            }
        }

        /// <summary>
        /// Detects a continuous trigger event.
        /// </summary>
        /// <param name="aCollider"></param>
        private void OnTriggerStay(Collider aCollider)
        {
            Interactive triggeringObject = aCollider.GetComponent<Interactive>();
            if(triggeringObject != null)
            {
                triggeringObject.onPlayerStay(this);
            }
        }


        /// <summary>
        /// Processes the character action button.
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if(action == true)
            {
                processActionButton();
            }

        }
        protected override void SlowUpdate()
        {
            base.SlowUpdate();
        }


        /// <summary>
        /// Constantly check to see if an object is in focus.
        /// </summary>
        private void FixedUpdate()
        {
            if(characterCamera == null)
            {
                return;
            }

            Vector3 origin = characterCamera.transform.position;
            Ray ray = characterCamera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.0f));
            Vector3 direction = ray.direction;
            direction.Normalize();

            int layerMask = 1 << GameManager.OBJECT_INTERACTION_LAYER;
            RaycastHit hit;

            if(Physics.Raycast(origin,direction,out hit, m_SearchDistance, layerMask))
            {
                //First do a check on the object itself
                Interactive interactiveObject = hit.transform.GetComponent<Interactive>();
                if(interactiveObject != null)
                {
                    checkForObject(interactiveObject);
                }
                else //Do a second check on the child of the object in case the collider was put on the child instead.
                {
                    interactiveObject = hit.transform.GetComponentInChildren<Interactive>();
                    if(interactiveObject != null)
                    {
                        checkForObject(interactiveObject);
                    }
                }
            }
            else
            {
                if(m_ObjectInFocus != null)
                {
                    GameEventArgs eventArgs = new GameEventArgs(GameEventID.INTERACTION_PLAYER_FOCUS_END);
                    eventArgs.sender = this;
                    eventArgs.triggeringObject = m_ObjectInFocus;
                    GameManager.triggerEvent(eventArgs.eventID, eventArgs);

                    m_ObjectInFocus.onPlayerFocusExit(this);
                    m_ObjectInFocus = null;
                }
            }
        }

        /// <summary>
        /// This method quickly checks to see which calls it should make to the object in focus.
        /// </summary>
        /// <param name="aObject"></param>
        private void checkForObject(Interactive aObject)
        {
            if (aObject == null)
            {
                return;
            }
            //Do we already have an interactive object
            if (m_ObjectInFocus == null)
            {
                m_ObjectInFocus = aObject;
                //Send player focus begin to the new player in focus
                GameEventArgs eventArgs = new GameEventArgs(GameEventID.INTERACTION_PLAYER_FOCUS_BEGIN);
                eventArgs.sender = this;
                eventArgs.triggeringObject = m_ObjectInFocus;
                GameManager.triggerEvent(eventArgs.eventID, eventArgs);
                m_ObjectInFocus.onPlayerFocusEnter(this);
                m_ObjectInFocus.onPlayerFocus(this);
            }
            //Make a stay call only
            else if (aObject == m_ObjectInFocus)
            {
                m_ObjectInFocus.onPlayerFocus(this);
            }
            //IF the objects are not equal
            else if (aObject != m_ObjectInFocus)
            {
                //Send a player focus end to the current player in focus
                GameEventArgs eventArgs = new GameEventArgs(GameEventID.INTERACTION_PLAYER_FOCUS_END);
                eventArgs.sender = this;
                eventArgs.triggeringObject = m_ObjectInFocus;
                GameManager.triggerEvent(eventArgs.eventID, eventArgs);
                m_ObjectInFocus.onPlayerFocusExit(this);
                m_ObjectInFocus = aObject;

                //Send a player focus being to the new player.
                eventArgs = new GameEventArgs(GameEventID.INTERACTION_PLAYER_FOCUS_BEGIN);
                eventArgs.sender = this;
                eventArgs.triggeringObject = m_ObjectInFocus;
                GameManager.triggerEvent(eventArgs.eventID, eventArgs);
                m_ObjectInFocus.onPlayerFocusEnter(this);
                m_ObjectInFocus.onPlayerFocus(this);
            }
        }

        /// <summary>
        /// Use is private because only the character interactoin shyould handle these events.
        /// </summary>
        /// <param name="aObject"></param>
        private void use(Interactive aObject)
        {
            if(aObject == null)
            {
                return;
            }
            //If uusing a plant stop using it and stop.
            if(m_ObjectInUse != null)
            {
                bool overrideUseEnd = false;
                m_ObjectInUse.onUseEnd(this,out overrideUseEnd);
                if(overrideUseEnd )
                {
                    Debug.Log("On use end is being overrided.");
                    return;
                }
                GameEventArgs eventArgs = new GameEventArgs(GameEventID.INTERACTION_PLAYER_ON_USE_END);
                eventArgs.sender = this;
                eventArgs.triggeringObject = m_ObjectInFocus;
                GameManager.triggerEvent(eventArgs.eventID, eventArgs);

                
                m_ObjectInUse = null;
                lockMovement = false;
                lockRotation = false;
                lockGravity = false;
                return;
            }

            //Begin using a plant
            m_ObjectInUse = aObject;
            GameEventArgs eventArg = new GameEventArgs(GameEventID.INTERACTION_PLAYER_ON_USE);
            eventArg.sender = this;
            eventArg.triggeringObject = m_ObjectInFocus;
            GameManager.triggerEvent(eventArg.eventID, eventArg);
            m_ObjectInUse.onUse(this);
            //Lock all states
            lockMovement = true;
            lockRotation = true;
            lockGravity = true;

            characterMotor.resetVelocity();
        }

        /// <summary>
        /// An external object at any time can tell the character interaction to stop using it. (Example, the object was destroyed and player needs to be released).
        /// </summary>
        public void stopUsing()
        {
            
            if(m_ObjectInUse != null)
            {
                bool overrideUseEnd = false;
                m_ObjectInUse.onUseEnd(this, out overrideUseEnd);
                if(overrideUseEnd)
                {
                    Debug.Log("Overriding on use end.");
                    return;
                }
                GameEventArgs eventArgs = new GameEventArgs(GameEventID.INTERACTION_PLAYER_ON_USE_END);
                eventArgs.sender = this;
                eventArgs.triggeringObject = m_ObjectInFocus;
                GameManager.triggerEvent(eventArgs.eventID, eventArgs);
                
            }
            m_ObjectInUse = null;
            lockMovement = false;
            lockRotation = false;
            lockGravity = false;
        }

        /// <summary>
        /// Invoke this button every time the use button gets pressed to use / stop using a gameobject
        /// </summary>
        public void processActionButton()
        {
            if(m_ObjectInUse != null)
            {
                stopUsing();
                return;
            }

            if(m_TriggeringObjects.Count == 0)
            {
                return;
            }

            //Sort the interactive game objects list by distance

            m_TriggeringObjects.Sort(delegate(Interactive a, Interactive b)
            {
                return Vector3.Distance(transform.position, a.transform.position).CompareTo((Vector3.Distance(transform.position, b.transform.position)));
            });

            
            List<Interactive>.Enumerator iterator = m_TriggeringObjects.GetEnumerator();
            //Loop the list until a condition returns true
            //Where condition returns true use that object
            while(iterator.MoveNext())
            {
                if(iterator.Current == null)
                {
                    //Clean up null objects
                    m_TriggeringObjects.Remove(iterator.Current);
                    continue ;
                }
                if(iterator.Current.condition(transform,iterator.Current.transform))
                {
                    use(iterator.Current);
                    break;
                }
            }
        }

        /// <summary>
        /// Returns the current object in use. Null if none is there.
        /// </summary>
        public Interactive objectInUse
        {
            get { return m_ObjectInUse; }
        }
        /// <summary>
        /// Returns the current object in focus. Null if there is none
        /// </summary>
        public Interactive objectInFocus
        {
            get { return m_ObjectInFocus; }
        }
        /// <summary>
        /// Returns all the objects within in the character interaction collider.
        /// </summary>
        public Interactive[] triggeringObjects
        {
            get { return m_TriggeringObjects.ToArray(); }
        }

        public void releaseUsedObject()
        {
            if (m_ObjectInUse != null)
            {
                bool overrideUseEnd;
                m_ObjectInUse.onUseEnd(this, out overrideUseEnd);
                GameEventArgs eventArgs = new GameEventArgs(GameEventID.INTERACTION_PLAYER_ON_USE_END);
                eventArgs.sender = this;
                eventArgs.triggeringObject = m_ObjectInFocus;
                GameManager.triggerEvent(eventArgs.eventID, eventArgs);

            }
            m_ObjectInUse = null;
            lockMovement = false;
            lockRotation = false;
            lockGravity = false;
        }
            
    }
}