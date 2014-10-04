using UnityEngine;
using System.Collections;
using EndevGame;

namespace EndevGame
{

    public class CharacterManager : EndevBehaviour
    {
        [SerializeField]
        private CharacterMotor m_CharacterMotor = null;
        [SerializeField]
        private CharacterInteraction m_CharacterInteraction = null;
        [SerializeField]
        private CharacterAnimation m_CharacterAnimation = null;



        /// <summary>
        /// The character requires a camera to move around.
        /// </summary>
        [SerializeField]
        private Camera m_CharacterCamera = null;
        [SerializeField]
        private OrbitCamera m_OrbitCamera = null;
        [SerializeField]
        private Transform m_CameraTarget = null;

        [SerializeField]
        private CharacterInputState m_InputState = new CharacterInputState();
        #region InputNames
        //Input Names
        [SerializeField]
        private string m_ForwardAxis = "Vertical"; //W & S
        [SerializeField]
        private string m_SideAxis = "Horizontal"; //A & D
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

        /// <summary>
        /// The variables put locks onto the player states which allow for external sources to override the player.
        /// </summary>
        [SerializeField]
        private bool m_LockMovement;
        [SerializeField]
        private bool m_LockRotation;
        [SerializeField]
        private bool m_LockGravity;

        // Use this for initialization
        void Start()
        {
            m_CharacterCamera = CameraManager.gameplayCamera;
            m_OrbitCamera = CameraManager.orbitCamera;

            CameraManager camMan = CameraManager.instance;
            if (camMan != null)
            {
                camMan.transitionToOrbit(m_CameraTarget, CameraMode.INSTANT, 0.0f);
            }

            //Main Component Search
            m_CharacterMotor = GetComponent<CharacterMotor>();
            m_CharacterInteraction = GetComponent<CharacterInteraction>();

            //Child component Search
            if(m_CharacterMotor == null)
            {
                m_CharacterMotor = GetComponentInChildren<CharacterMotor>();
            }
            if(m_CharacterInteraction == null)
            {
                m_CharacterInteraction = GetComponentInChildren<CharacterInteraction>();
            }
        }

        // Update is called once per frame
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

        public CharacterInteraction characterInteraction
        {
            get { return m_CharacterInteraction; }
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
        /// Lock the players rotation preventing them from rotating properly.
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
        
    }
}