using UnityEngine;
using System;
using System.Collections.Generic;


namespace OnLooker
{
    namespace UI
    {
        public class UIEventHandler : MonoBehaviour
        {

            protected virtual void onUIEvent(UIToggle aSender, UIEventArgs aArgs)
            {
                Debug.Log(aArgs.eventType);
            }
                
        }
    }
}
