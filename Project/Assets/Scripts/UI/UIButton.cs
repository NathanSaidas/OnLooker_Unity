using UnityEngine;
using System.Collections;
namespace Gem
{
    public enum ButtonState
    {
        /// <summary>
        /// Represents a disabled button state. The button does not relay events to its listener
        /// </summary>
        DISABLED,
        /// <summary>
        /// Represents the buttons normal state. There is no action going on
        /// </summary>
        NORMAL,
        /// <summary>
        /// Represents the buttons hover state. The mouse is over the button.
        /// </summary>
        HOVER,
        /// <summary>
        /// Represents the buttons down state. The mouse is pressed down on the button.
        /// </summary>
        DOWN
    }

    public class UIButton : UIEventListener
    {

#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("The current state of the button.")]
#endif
        [SerializeField]
        private ButtonState m_State = ButtonState.NORMAL;
#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("The texture for the buttons disabled state.")]
#endif
        [SerializeField]
        private Texture m_DisabledTexture = null;
#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("The texture for the buttons normal state.")]
#endif
        [SerializeField]
        private Texture m_NormalTexture = null;
#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("The texture for the buttons hover state.")]
#endif
        [SerializeField]
        private Texture m_HoverTexture = null;
#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("The texture for the buttons down state.")]
#endif
        [SerializeField]
        private Texture m_DownTexture = null;
#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("The text color for the buttons enabled state.")]
#endif
        [SerializeField]
        private Color m_EnabledTextColor = Color.white;
#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("The text color for the buttons disabled state.")]
#endif
        [SerializeField]
        private Color m_DisabledTextColor = Color.gray;
#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("The event listener listening in on the events fired by this button.")]
#endif
        [SerializeField]
        private UIEventListener m_EventListener = null;
        /// <summary>
        /// The state of the mouse being on top of the button or not
        /// </summary>
        private bool m_MouseInBounds = false;
        /// <summary>
        /// The state of the mouse being pressed down or not.
        /// </summary>
        private bool m_MouseDown = false;

        private UIImage m_Image = null;
        private UILabel m_Label = null;

        protected override void Start()
        {
            base.Start();
        }

        protected virtual void Update()
        {
            if(m_State != ButtonState.DISABLED)
            {
                if (m_MouseDown == true)
                {
                    m_State = ButtonState.DOWN;
                }
                else if(m_MouseInBounds == true)
                {
                    m_State = ButtonState.HOVER;
                }
                else
                {
                    m_State = ButtonState.NORMAL;
                }
            }

            ///TODO: Set texture of mesh based on state.
            switch(m_State)
            {
                case ButtonState.NORMAL:

                    break;
                case ButtonState.DISABLED:

                    break;
                case ButtonState.DOWN:

                    break;
                case ButtonState.HOVER:

                    break;
            }
        }

        protected override void OnMouseDownEvent()
        {
            m_MouseDown = true;
            if(m_EventListener != null)
            {
                m_EventListener.OnEvent(UIEvent.MOUSE_DOWN);
            }
        }
        protected override void OnMouseClickEvent()
        {
            m_MouseDown = false;
            if (m_EventListener != null)
            {
                m_EventListener.OnEvent(UIEvent.MOUSE_CLICK);
            }
        }
        protected override void OnMouseDoubleClickedEvent()
        {
            m_MouseDown = false;
            if (m_EventListener != null)
            {
                m_EventListener.OnEvent(UIEvent.MOUSE_DOUBLE_CLICK);
            }
        }
        protected override void OnMouseHoverEvent()
        {
            m_MouseDown = false;
            if (m_EventListener != null)
            {
                m_EventListener.OnEvent(UIEvent.MOUSE_HOVER);
            }
        }
        protected override void OnMouseEnterEvent()
        {
            m_MouseInBounds = true;
            if (m_EventListener != null)
            {
                m_EventListener.OnEvent(UIEvent.MOUSE_ENTER);
            }
        }
        protected override void OnMouseExitEvent()
        {
            m_MouseInBounds = false;
            if (m_EventListener != null)
            {
                m_EventListener.OnEvent(UIEvent.MOUSE_EXIT);
            }
        }

        public void Disable()
        {
            m_State = ButtonState.DISABLED;
        }
        public void Enable()
        {
            m_State = ButtonState.NORMAL;
        }

        public ButtonState buttonState
        {
            get{return m_State;}
        }

        /// <summary>
        /// Represents the texture of the button when disabled.
        /// </summary>
        public Texture disabledTexture
        {
            get { return m_DisabledTexture; }
            set { m_DisabledTexture = value; }
        }
        /// <summary>
        /// Represents the default texture of the button.
        /// </summary>
        public Texture normalTexture
        {
            get { return m_NormalTexture; }
            set { m_NormalTexture = value; }
        }
        /// <summary>
        /// Represents the texture of the button when hovered over.
        /// </summary>
        public Texture hoverTexture
        {
            get { return m_HoverTexture; }
            set { m_HoverTexture = value; }
        }
        /// <summary>
        /// Represents the texture of the button when pressed down.
        /// </summary>
        public Texture downTexture
        {
            get { return m_DownTexture; }
            set { m_DownTexture = value; }
        }
        /// <summary>
        /// Represents the color of the enabled button text.
        /// </summary>
        public Color enabledTextColor
        {
            get { return m_EnabledTextColor; }
            set { m_EnabledTextColor = value; }
        }
        /// <summary>
        /// Represents the color of the disabled button text.
        /// </summary>
        public Color disabledTextColor
        {
            get { return m_DisabledTextColor; }
            set { m_DisabledTextColor = value; }
        }

        /// <summary>
        /// Receives button events.
        /// </summary>
        public UIEventListener eventListener
        {
            get { return m_EventListener; }
            set { m_EventListener = value; }
        }

        public UIImage image
        {
            get { return m_Image; }
        }
        public UILabel label
        {
            get { return m_Label; }
        }
    }
}