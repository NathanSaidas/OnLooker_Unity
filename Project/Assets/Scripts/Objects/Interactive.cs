using UnityEngine;
using System.Collections.Generic;
using System.Collections;



namespace EndevGame
{
    //This is the base class for all interactive objects in the game world
    /*
    *   Class: PlantManager
    *   Base Class: MonoBehaviour
    *   Interfaces: IConditional
    *   Description: This is the base class for all interactive objects in the game world. It provides a number of functions to call to invoke events. It also provides a way to send its message out to other components
    *   Date Reviewed: Sept 26th 2014 by Nathan Hanlan
    */
    public class Interactive : MonoBehaviour, IConditional
    {
        //The event other components can listen in on 
        private event OnInteractiveCallback m_PlayerEvent;
        /// <summary>
        /// Event registration
        /// </summary>
        /// <param name="aCallback">The reference to the function to callback to</param>
        public void register(OnInteractiveCallback aCallback)
        {
            if (aCallback != null)
            {
                m_PlayerEvent += aCallback;
            }
        }
        /// <summary>
        /// Event unregistration
        /// </summary>
        /// <param name="aCallback">The reference to the function to stop making callbacks to</param>
        public void unregister(OnInteractiveCallback aCallback)
        {
            if(aCallback != null)
            {
                m_PlayerEvent -= aCallback;
            }
        }
        //Call this to invoke the event
        protected void invokeCallback(InteractiveArgs aArgs)
        {
            if (m_PlayerEvent != null)
            {
                m_PlayerEvent.Invoke(this, aArgs);
            }
        }
        //Gets called when the player enters the plant trigger area
        public virtual void onPlayerEnter(CharacterInteraction aPlayer)
        {
            invokeCallback(new InteractiveArgs("PlayerEnter", aPlayer));
        }
        //Gets called when the player stays within the plant trigger area
        public virtual void onPlayerStay(CharacterInteraction aPlayer)
        {
            invokeCallback(new InteractiveArgs("PlayerStay", aPlayer));
        }
        //Gets called when the player leaves the plant trigger area
        public virtual void onPlayerExit(CharacterInteraction aPlayer)
        {
            invokeCallback(new InteractiveArgs("PlayerExit", aPlayer));
        }
        //Gets called when the player looks at an object with a collider who has their layer set to "Object Interaction" ie 8th Layer
        public virtual void onPlayerFocusEnter(CharacterInteraction aPlayer)
        {
            invokeCallback(new InteractiveArgs("FocusBegin", aPlayer));
        }
        //Gets called for every frame the player looks at an object with a collider who has their layer set to "Object Interaction" ie 8th Layer
        public virtual void onPlayerFocus(CharacterInteraction aPlayer)
        {
            invokeCallback(new InteractiveArgs("FocusContinue", aPlayer));
        }
        //Gets called when the player stops looking at the object with a collider who has their layer set to "Object Interaction" ie 8th Layer
        public virtual void onPlayerFocusExit(CharacterInteraction aPlayer)
        {
            invokeCallback(new InteractiveArgs("FocusEnd", aPlayer));
        }
        //Gets called when the player starts using this object.
        public virtual void onUse(CharacterInteraction aPlayer)
        {
            invokeCallback(new InteractiveArgs("Use", aPlayer));
        }
        //Gets called when the player stops using this object
        public virtual void onUseEnd(CharacterInteraction aPlayer)
        {
            invokeCallback(new InteractiveArgs("StopUsing", aPlayer));
        }
        //The condition to check before the player may use this object.
        public virtual bool condition(Transform aPlayer, Transform aPlant)
        {
            //This is an example condition which checks if the player is facing the plant
            //And is within the minimum distance from the target
            float minDistance = 3.0f;

            //facing > 0 = facing forward
            //facing < 0 = facing backward
            float distance = Vector3.Distance(aPlant.position, aPlayer.position);
            Vector3 direction = (aPlant.position - aPlayer.position).normalized;
            float facing = Vector3.Dot(direction, aPlayer.forward);

            if (distance <= minDistance && facing > 0.0f)
            {
                return true;
            }
            return false;
        }

    }
}