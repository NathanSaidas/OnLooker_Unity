using UnityEngine;
using System.Collections;

namespace Gem
{
    

    public class UIEventListener : MonoBehaviour
    {
        protected UIToggle m_Toggle = null;
        protected MeshRenderer m_MeshRenderer = null;
        // Use this for initialization
        void Start()
        {
            m_Toggle = GetComponent<UIToggle>();
            m_MeshRenderer = GetComponent<MeshRenderer>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnEvent(UIEvent aEvent)
        {
            //TODO: Sync this up with EndevGame.GameManager Event
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
            Debug.Log("Click Event Yay");
        }
        protected virtual void OnMouseDoubleClickedEvent()
        {

        }
        protected virtual void OnSelectedEvent()
        {
            Debug.Log("Selected Ouuu");
        }
        protected virtual void OnUnselectedEvent()
        {
            Debug.Log("Unselected :(");
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