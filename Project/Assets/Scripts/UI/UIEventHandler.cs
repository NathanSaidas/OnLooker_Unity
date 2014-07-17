using UnityEngine;
using System;
using System.Collections.Generic;


namespace OnLooker
{
    namespace UI
    {
        public class UIEventHandler : MonoBehaviour
        {
            public UIToggle m_Toggle;

            private void Start()
            {
                if (m_Toggle == null)
                {
                    m_Toggle = GetComponent<UIToggle>();
                }
                if (m_Toggle != null)
                {
                    m_Toggle.registerEvent(onUIEvent);
                }
            }
            private void OnDestroy()
            {
                if (m_Toggle != null)
                {
                    m_Toggle.unregisterEvent(onUIEvent);
                }
            }


            protected virtual void onUIEvent(UIToggle aSender, UIEventArgs aArgs)
            {
                Debug.Log(aArgs.eventType);
            }
                
        }
    }
}
