using UnityEngine;
using System.Collections;
#region CHANGE LOG
/* October,27,2014 - Nathan Hanlan, Added and implemented UIToggle
 * November,14,2014 - Nathan Hanlan, Exposed the UIEventListener field as a property.
 */
#endregion

namespace Gem
{
    /// <summary>
    /// This class serves as the main component of any UI element. 
    /// The UI Manager which manage toggles and send events out to toggles. The events will then be handled by a UIEventListener.
    /// </summary>
    public class UIToggle : MonoBehaviour
    {
        #region FIELDS
        bool m_HasMouseOver = false;
        bool m_IsSelected = false;
        float m_LastClickTime = 0.0f;
        /// <summary>
        /// The ID of the UI Toggle. Set the ID during edit time for a reserved ID. UI Manager will overwrite if its taken.
        /// </summary>
#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("The ID of the UI Toggle. Set the ID during edit time for a reserved ID. UI Manager will overwrite if its taken at runtime.")]
#endif
    	[SerializeField]
        int m_ID = -1;
        /// <summary>
        /// The space the UI element is to be put in. (2D,3D attached to camera. World, viewed within the world)
        /// </summary>
#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("The space the UI element is to be put in (2D,3D attached to camera. World, viewed within the world).")]
#endif
        [SerializeField]
        UISpace m_UISpace = UISpace.TWO_DIMENSIONAL;

        /// <summary>
        /// Whether or not to recieve action events.
        /// </summary>
#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("Whether or not to recieve action events.")]
#endif
        [SerializeField]
        private bool m_ReceiveActions = false;
        /// <summary>
        /// Whether or not the UI Toggle can be selected.
        /// </summary>
#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("Whether or not the UI Toggle can be selected.")]
#endif
        [SerializeField]
        private bool m_Selectable = true;
        /// <summary>
        /// A event listener listening in on UI events.
        /// </summary>
#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("A event listener listening in on UI events.")]
#endif
        [SerializeField]
        private UIEventListener m_EventListener = null;
        /// <summary>
        /// Determines whether or not the UI is enabled
        /// </summary>
#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("Determines whether or not the UI is enabled")]
#endif
        [SerializeField]
        private bool m_UIEnabled = true;
#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("The type of UI this toggle is considered.")]  
#endif
        [SerializeField]
        private UIType m_UIType = UIType.IMAGE;

        #endregion
        /// <summary>
        /// Searches for an event listener in sibling / children heirarchy.
        /// Registers with the UI manager.
        /// </summary>
        void Start()
        {
            if(m_EventListener == null)
            {
                m_EventListener = GetComponent<UIEventListener>();
            }
            if(m_EventListener == null)
            {
                m_EventListener = GetComponentInChildren<UIEventListener>();
            }
            UIManager.Register(this);
        }
        /// <summary>
        /// Unregisters with the UI Manager.
        /// </summary>
        void OnDestroy()
        {
            UIManager.Unregister(this);
        }
        /// <summary>
        /// Gets invoked by the UI Manager to send an event which can then be handled by the event listener.
        /// </summary>
        /// <param name="aEvent"></param>
        public void OnEvent(UIEvent aEvent)
        {
            if(m_EventListener != null && uiEnabled)
            {
                m_EventListener.OnEvent(aEvent);
            }
        }

        #region PROPERTIES
        /// <summary>
        /// The ID of the toggle.
        /// Avoid Changing at Runtime.
        /// </summary>
        public int id
        {
            get { return m_ID; }
            set { m_ID = value; }
        }
        /// <summary>
        /// Returns true if the toggle currently has the mouse over it.
        /// Avoid setting this value.
        /// </summary>
        public bool hasMouseOver
        {
            get { return m_HasMouseOver; }
            set { m_HasMouseOver = value; }
        }
        /// <summary>
        /// Returns true if the toggle is currently selected by the UI Manager.
        /// Avoid setting this value.
        /// </summary>
        public bool isSelected
        {
            get { return m_IsSelected; }
            set { m_IsSelected = value; }
        }
        /// <summary>
        /// Returns the last time the toggle was clicked.
        /// Avoid setting this value.
        /// </summary>
        public float lastClickTime
        {
            get { return m_LastClickTime; }
            set { m_LastClickTime = value; }
        }
        /// <summary>
        /// Get / Set the space for UI
        /// </summary>
        public UISpace uiSpace
        {
            get { return m_UISpace; }
            set { m_UISpace = value; }
        }
        /// <summary>
        /// Get / Set the state of whether or not this toggle will receive action events.
        /// Avoid setting this at runtime.
        /// </summary>
        public bool receivesActionEvents
        {
            get { return m_ReceiveActions; }
            set { m_ReceiveActions = value; }
        }
        /// <summary>
        /// Get / Set the state of whether or not this toggle will be selectable.
        /// Avoid setting this at runtime.
        /// </summary>
        public bool selectable
        {
            get { return m_Selectable; }
            set { m_Selectable = value; }
        }
        /// <summary>
        /// Determines if the UI is enabled.
        /// Disabled UI elements will not process events.
        /// </summary>
        public bool uiEnabled
        {
            get { return m_UIEnabled; }
            set { m_UIEnabled = value; }
        }
        /// <summary>
        /// Determines the type of UI this toggle is considered.
        /// </summary>
        public UIType uiType
        {
            get { return m_UIType; }
            set { m_UIType = value; }
        }

        public UIEventListener eventListener
        {
            get { return m_EventListener; }
            set { m_EventListener = value; }
        }
        #endregion

    }
}