using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;



namespace EndevGame
{
    //This struct represents the arguments that are passed on InteractiveArgs events.
    public struct InteractiveArgs
    {
        private string m_Message;
        private CharacterInteraction m_TriggeringPlayer;

        public InteractiveArgs(string aMessage, CharacterInteraction aTriggeringPlayer)
        {
            m_Message = aMessage;
            m_TriggeringPlayer = aTriggeringPlayer;
        }
        [Obsolete("Use \'Message\' instead.")]
        public string callbackType
        {
            get { return m_Message; }
        }
        public string message
        {
            get { return m_Message; }
        }
            
        public CharacterInteraction triggeringPlayer
        {
            get { return m_TriggeringPlayer; }
        }

    }

    public delegate void OnInteractiveCallback(Interactive aSender,InteractiveArgs aArgs);
}