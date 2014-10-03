using UnityEngine;
using System.Collections.Generic;
using System.Collections;



namespace EndevGame
{
    //This struct represents the arguments that are passed on InteractiveArgs events.
    public struct InteractiveArgs
    {
        private string m_CallbackType;
        private CharacterInteraction m_TriggeringPlayer;

        public InteractiveArgs(string aCallbackType, CharacterInteraction aTriggeringPlayer)
        {
            m_CallbackType = aCallbackType;
            m_TriggeringPlayer = aTriggeringPlayer;
        }
        public string callbackType
        {
            get { return m_CallbackType; }

        }
            
        public CharacterInteraction triggeringPlayer
        {
            get { return m_TriggeringPlayer; }
        }

    }

    public delegate void OnInteractiveCallback(Interactive aSender,InteractiveArgs aArgs);
}