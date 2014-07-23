using UnityEngine;
using System;
using System.Collections.Generic;


namespace OnLooker
{
    namespace UI
    {
        [ExecuteInEditMode()]
        public class UIEventHandler : MonoBehaviour
        {
            
            //public UIToggle m_Toggle;

            private void Start()
            {
                if (Application.isPlaying)
                {
                    gameStart();
                }
            }

            
            private void OnDestroy()
            {
                if (Application.isPlaying)
                {
                    gameDestroy();
                }
            }

            protected virtual void gameStart()
            {

            }
            protected virtual void gameDestroy()
            {

            }

            protected virtual void onUIEvent(UIToggle aSender, UIEventArgs aArgs)
            {
                Debug.Log(aArgs.eventType);
            }
                
        }
    }
}
