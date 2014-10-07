using UnityEngine;
using System.Collections;
using EndevGame;

namespace EndevGame
{
    #region 
    /* October,6,2014 - Nathan Hanlan - Added additional regions and comments.
     * October,7,2014 - Nathan Hanlan - Added support for CharacterAnimation.
    */
    #endregion
    /// <summary>
    /// 
    /// </summary>
    public class CharacterManager : EndevBehaviour
    {
        #region Fields
        /// <summary>
        /// The character motor for the Character. 
        /// </summary>
        [SerializeField]
        private CharacterMotor m_CharacterMotor = null;

        /// <summary>
        /// The character interaction for the character.
        /// </summary>
        [SerializeField]
        private CharacterInteraction m_CharacterInteraction = null;

        /// <summary>
        /// The character animation for the character.
        /// </summary>
        [SerializeField]
        private CharacterAnimation m_CharacterAnimation = null;

        /// <summary>
        /// The character ledge grab for the character.
        /// </summary>
        [SerializeField]
        private CharacterLedgeGrab m_CharacterLedgeGrab = null;
        /// <summary>
        /// The character climbing for the character.
        /// </summary>
        [SerializeField]
        private CharacterClimbing m_CharacterClimbing = null;


        /// <summary>
        /// The required camera for the character. Used by CharacterMotor
        /// </summary>
        [SerializeField]
        private Camera m_CharacterCamera = null;
        /// <summary>
        /// The required orbit camera for the character. Used by CharacterMotor
        /// </summary>
        [SerializeField]
        private OrbitCamera m_OrbitCamera = null;
        /// <summary>
        /// The target for the orbit camera to follow.
        /// </summary>
        [SerializeField]
        private Transform m_CameraTarget = null;

        /// <summary>
        /// A data structure to hold all the input states.
        /// </summary>
        [SerializeField]
        private CharacterInputState m_InputState = new CharacterInputState();
        #endregion

        #region InputNames

        ///All the input names used by the character.
        ///TODO: Make a new class file to store all these values as constants. :)
        

        [SerializeField]
        private string m_ForwardAxis = "Vertical"; //W & S
        [SerializeField]
        private string m_SideAxis = "Horizontal"; //A & D
        [SerializeField]
        private string m_FixedSideAxis = "Mouse X"; //mouse x motion
        [SerializeField]
        private string m_JumpButton = "Jump"; //SPACE
        [SerializeField]
        private string m_CrouchButton = "Crouch"; //Left Control
        [SerializeField]
        private string m_ActionButton = "Use"; //E
        [SerializeField]
        private string m_SprintButton = "Sprint"; //Q

        [SerializeField]
        private string m_GrowButton = "Grow";
        [SerializeField]
        private string m_ShrinkButton = "Shrink";

        [SerializeField]
        private string m_ShootButton = "Shoot";
        [SerializeField]
        private string m_ProjectileScroll = "AmmoSwitch";
        [SerializeField]
        private string m_ShootMode = "ShootMode";
        #endregion

        #region State Lock Variables
        /// The variables put locks onto the player states which allow for external sources to override the player.
        /// <summary> 
        /// Locking the movement will prevent the Character motor from driving the character.
        /// </summary>
        [SerializeField]
        private bool m_LockMovement;
        /// <summary>
        /// Locking rotation will prevent the Character motor from rotating the character
        /// </summary>
        [SerializeField]
        private bool m_LockRotation;
        /// <summary>
        /// Locking gravity will force gravity to not be applied to the character Motor.
        /// </summary>
        [SerializeField]
        private bool m_LockGravity;
        #endregion
        

        /// <summary>
        /// Invoked to initialize the character.
        /// </summary>
        void Start()
        {
            //Grab the camera from the camera manager.
            m_CharacterCamera = CameraManager.gameplayCamera;
            m_OrbitCamera = CameraManager.orbitCamera;

            //Tell the camera manager to make a transition
            CameraManager camMan = CameraManager.instance;
            if (camMan != null)
            {
                camMan.transitionToOrbit(m_CameraTarget, CameraMode.INSTANT, 0.0f);
                //camMan.transitionToShoulder(m_CameraTarget, CameraMode.INSTANT, 0.0f);
            }

            //Main Component Search
            m_CharacterMotor = GetComponent<CharacterMotor>();
            m_CharacterInteraction = GetComponent<CharacterInteraction>();
            m_CharacterLedgeGrab = GetComponent<CharacterLedgeGrab>();
            m_CharacterClimbing = GetComponent<CharacterClimbing>();
            m_CharacterAnimation = GetComponent<CharacterAnimation>();
            //End sarch

            //Child component Search
            if(m_CharacterMotor == null)
            {
                m_CharacterMotor = GetComponentInChildren<CharacterMotor>();
            }
            if(m_CharacterInteraction == null)
            {
                m_CharacterInteraction = GetComponentInChildren<CharacterInteraction>();
            }
            if(m_CharacterLedgeGrab == null)
            {
                m_CharacterLedgeGrab = GetComponentInChildren<CharacterLedgeGrab>();
            }
            if(m_CharacterClimbing == null)
            {
                m_CharacterClimbing = GetComponentInChildren<CharacterClimbing>();
            }
            if(m_CharacterAnimation == null)
            {
                m_CharacterAnimation = GetComponentInChildren<CharacterAnimation>();
            }
            //End search
        }

        
        protected override void Update()
        {
            if (m_InputState == null)
            {
                Debug.LogError("Missing \'InputState\' for \'CharacterManager\'");
                gameObject.SetActive(false);
                return;
            }

            //Retrieve the input states of the character.
            m_InputState.forwardMotion = InputManager.getAxis(m_ForwardAxis);
            m_InputState.sideMotion = InputManager.getAxis(m_SideAxis);
            m_InputState.fixedSideMotion = InputManager.getAxis(m_FixedSideAxis);
            m_InputState.jump = InputManager.getButtonDown(m_JumpButton);
            m_InputState.crouch = InputManager.getButtonDown(m_CrouchButton);
            m_InputState.action = InputManager.getButtonDown(m_ActionButton);
            m_InputState.sprint = InputManager.getButton(m_SprintButton);
            m_InputState.shrink = InputManager.getButton(m_ShrinkButton);
            m_InputState.grow = InputManager.getButton(m_GrowButton);
            m_InputState.shootMode = InputManager.getButtonDown(m_ShootMode);
            m_InputState.projectileType = InputManager.getAxis(m_ProjectileScroll);
            m_InputState.shoot = InputManager.getButtonDown(m_ShootButton);

        }


        #region InputProperties
        public float forwardMotion
        {
            get { return m_InputState.forwardMotion; }
        }
        public float sideMotion
        {
            get { return m_InputState.sideMotion; }
        }
        public float fixedSideMotion
        {
            get { return m_InputState.fixedSideMotion; }
        }
        public bool jump
        {
            get { return m_InputState.jump; }
        }
        public bool crouch
        {
            get { return m_InputState.crouch; }
        }
        public bool action
        {
            get { return m_InputState.action; }
        }
        public bool grow
        {
            get { return m_InputState.grow; }
        }
        public bool shrink
        {
            get { return m_InputState.shrink; }
        }
        public bool shoot
        {
            get { return m_InputState.shoot; }
        }
        public bool sprint
        {
            get { return m_InputState.sprint; }
        }

        public float projectileType
        {
            get { return m_InputState.projectileType; }
        }

        public bool shootMode
        {
            get { return m_InputState.shootMode; }
        }
        #endregion

        #region ChildComponents
        /* October,6,2014 - Nathan Hanlan - Added characterInteraction, characterLedgeGrab, characterClimbing
         * 
         * 
         * 
         */
        /// <summary>
        /// Returns the character motor attached to the Character Manager gameobject. This component is responsible for the movement of the character.
        /// </summary>
        public CharacterMotor characterMotor
        {
            get { return m_CharacterMotor; }
        }
        /// <summary>
        /// Returns the character animation attached to the Character Manager gameobject. This component is reposnisble for playing all the animation for the character.
        /// </summary>
        public CharacterAnimation characterAnimation
        {
            get { return m_CharacterAnimation; }
        }
        /// <summary>
        /// Returns the character interaction component attached to the Character Manager. This component is responsible for creating the interaction link between X objects and Interactive Objects
        /// </summary>
        public CharacterInteraction characterInteraction
        {
            get { return m_CharacterInteraction; }
        }
        /// <summary>
        /// Returns the character ledge grab component. This component is responsible for driving the characters motion while on a ledge.
        /// </summary>
        public CharacterLedgeGrab characterLedgeGrab
        {
            get { return m_CharacterLedgeGrab; }
        }
        /// <summary>
        /// Returns the character climbing component. This component is responsible for driving the characters motoin while climbing on a surface. (Does not include legdes).
        /// </summary>
        public CharacterClimbing characterClimbing
        {
            get { return m_CharacterClimbing; }
        }

       


        /// <summary>
        /// Returns the camera the character uses. This is used for characters movement.
        /// </summary>
        public Camera characterCamera
        {
            get { return m_CharacterCamera; }
        }
        /// <summary>
        /// Returns the orbit camera which is taken from the CameraManager.
        /// </summary>
        public OrbitCamera orbitCamera
        {
            get { return m_OrbitCamera; }
        }
        /// <summary>
        /// Returns the character camera target. This is used for the camera's target.
        /// </summary>
        public Transform cameraTarget
        {
            get { return m_CameraTarget; }
            set { m_CameraTarget = value; }
        }
        #endregion

        #region Character State Variables
        /// <summary>
        /// Returns true if the current camera is focused on the player.
        /// </summary>
        public bool ownsCamera
        {
            get { return CameraManager.cameraIsMine(m_CameraTarget); }
        }
        /// <summary>
        /// Lock the players movement preventing them to move.
        /// </summary>
        public bool lockMovement
        {
            get { return m_LockMovement; }
            set { m_LockMovement = value; }
        }
        /// <summary>
        /// Lock the players rotation preventing them from rotating using the CharacterMotor.
        /// </summary>
        public bool lockRotation
        {
            get { return m_LockRotation; }
            set { m_LockRotation = value; }
        }
        /// <summary>
        /// Lock the players gravity forcing gravity to not be applied.
        /// </summary>
        public bool lockGravity
        {
            get { return m_LockGravity; }
            set { m_LockGravity = value; }
        }
        #endregion

    }
}