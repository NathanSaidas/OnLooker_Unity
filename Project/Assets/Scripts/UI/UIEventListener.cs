using UnityEngine;
using System.Collections;

namespace Gem
{
    

    public class UIEventListener : MonoBehaviour
    {
        protected UIToggle m_Toggle = null;
        //protected MeshRenderer m_MeshRenderer = null;
        // Use this for initialization
        protected virtual void Start()
        {
            m_Toggle = GetComponent<UIToggle>();
            if(m_Toggle == null)
            {
                m_Toggle = GetComponentInParent<UIToggle>();
            }
            //m_MeshRenderer = GetComponent<MeshRenderer>();
        }

        public void OnEvent(UIEvent aEvent)
        {
            switch(aEvent)
            {
                case UIEvent.MOUSE_CLICK:
                    OnMouseClickEvent();
                    break;
                case UIEvent.MOUSE_DOUBLE_CLICK:
                    OnMouseDoubleClickedEvent();
                    break;
                case UIEvent.MOUSE_DOWN:
                    OnMouseDownEvent();
                    break;
                case UIEvent.MOUSE_ENTER:
                    OnMouseEnterEvent();
                    break;
                case UIEvent.MOUSE_EXIT:
                    OnMouseExitEvent();
                    break;
                case UIEvent.MOUSE_HOVER:
                    OnMouseHoverEvent();
                    break;
                case UIEvent.ON_ACTION:
                    OnActionEvent();
                    break;
                case UIEvent.SELECTED:
                    OnSelectedEvent();
                    break;
                case UIEvent.UNSELECTED:
                    OnUnselectedEvent();
                    break;
            }
        }

        public virtual void OnRelayEvent(UIEvent aEvent, UIEventListener aListener)
        {

        }

        protected virtual void OnMouseHoverEvent()
        {
            
        }
        protected virtual void OnMouseEnterEvent()
        {

        }
        protected virtual void OnMouseExitEvent()
        {

        }
        protected virtual void OnMouseDownEvent()
        {

        }
        protected virtual void OnMouseClickEvent()
        {
            
        }
        protected virtual void OnMouseDoubleClickedEvent()
        {

        }
        protected virtual void OnSelectedEvent()
        {
           
        }
        protected virtual void OnUnselectedEvent()
        {
            
        }
        protected virtual void OnActionEvent()
        {

        }

        /// <summary>
        /// The toggle responsible for triggering the events.
        /// </summary>
        public UIToggle uiToggle
        {
            get { return m_Toggle; }
            set { m_Toggle = value; }
        }
    }

}