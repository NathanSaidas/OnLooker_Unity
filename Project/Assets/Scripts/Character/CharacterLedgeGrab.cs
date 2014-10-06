using UnityEngine;
using System.Collections;

namespace EndevGame
{

    /// <summary>
    /// Original Design by Justin
    /// </summary>
    public class CharacterLedgeGrab : CharacterComponent
    {
        enum State
        {
            NONE,
            GRABBING,
            ON,
            CLIMBING_LEFT,
            CLIMBING_RIGHT,
        }
        
        /// <summary>
        /// The current ledge the player is grabbing
        /// </summary>
        private LedgeGrab m_TriggeringLedgeGrab = null;

        /// <summary>
        /// The target point the character has to reach to get to the ledge
        /// </summary>
        private Vector3 m_TargetPoint = Vector3.zero;
        /// <summary>
        /// The initial point the character was at. (At the time of collision)
        /// </summary>
        private Vector3 m_InitialPoint = Vector3.zero;


        /// <summary>
        /// The target rotation the character has to reach
        /// </summary>
        [SerializeField]
        private Quaternion m_TargetRotation = Quaternion.identity;
        /// <summary>
        /// The initial rotation the character was at. (At the time of collision)
        /// </summary>
        [SerializeField]
        private Quaternion m_InitialRotation = Quaternion.identity;

        /// <summary>
        /// An offset used to set the characters y position correctly relative to how high the character is.
        /// </summary>
        [SerializeField]
        private float m_CharacterHeight = 1.72f;
        /// <summary>
        /// An offset used to set the characters distance from the ledge. (Good for hand placement)
        /// </summary>
        [SerializeField]
        private float m_GrabOffset = 0.4f;
        /// <summary>
        /// How much time must be allowed to pass before the next state
        /// </summary>
        [SerializeField]
        private float m_GrabTime = 1.0f;
        /// <summary>
        /// How fast should the character grab onto the ledge
        /// </summary>
        [SerializeField]
        private float m_GrabSpeed = 1.0f;
        /// <summary>
        /// The characters current state regarding the ledge grabbing
        /// </summary>
        [SerializeField]
        private State m_State = State.NONE;
        /// <summary>
        /// The current time passed for the character grabbing the ledge. (State.GRABBING)
        /// </summary>
        [SerializeField]
        private float m_CurrentTime = 0.0f;



        [SerializeField]
        private float m_InputTime = 0.0f;
        [SerializeField]
        private bool m_InputLeft = false;
        [SerializeField]
        private bool m_InputRight = false;


        private void Start()
        {
            init();
        }

        private void OnTriggerEnter(Collider aCollider)
        {
            LedgeGrab ledgeGrab = aCollider.GetComponent<LedgeGrab>();
            if(ledgeGrab == null || m_TriggeringLedgeGrab != null)
            {
                return;
            }
            //try and grab, if it fails abort..
            if(ledgeGrab.grab(manager) == false)
            {
                return;
            }
            //Upon colliding with a ledge
            //Calculate the distance forwards towards the triggering ledge
            CapsuleCollider capsuleCollider = manager.GetComponent<CapsuleCollider>();
            Vector3 ledgeSize = ledgeGrab.size;
            float distance = capsuleCollider.radius + ledgeSize.z * 0.5f;

            //Create a forward vector and rotate it to face the ledge and scale the vector by the distance calculated.
            Vector3 forward = Vector3.forward * distance;
            forward.z += m_GrabOffset;
            Vector3 direction = aCollider.transform.rotation * forward;

            //Calculate the target point, and store the initial state of the character
            m_TargetPoint = new Vector3(manager.transform.position.x + direction.x, ledgeGrab.transform.position.y - m_CharacterHeight, manager.transform.position.z + direction.z);
            m_TargetRotation = ledgeGrab.transform.rotation;
            m_InitialPoint = manager.transform.position;
            m_InitialRotation = manager.transform.rotation;
            //Reset the time and begin the state
            m_CurrentTime = 0.0f;
            m_State = State.GRABBING;

            //Tell the ledge the character is grabbing onto it
            
            //Store the Ledge in the Character Ledge 
            m_TriggeringLedgeGrab = ledgeGrab;
            
            //Lock the character motor 
            lockMovement = true;
            lockGravity = true;
            lockRotation = true;
            characterMotor.resetVelocity();
        }

        protected override void Update()
        {
            base.Update();
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                if(m_TriggeringLedgeGrab != null)
                {
                    m_TriggeringLedgeGrab.release(manager);
                    m_TriggeringLedgeGrab = null;
                    lockMovement = false;
                    lockGravity = false;
                    lockRotation = false;
                    m_State = State.NONE;
                }
            }

            m_CurrentTime += Time.deltaTime * m_GrabSpeed;

            switch(m_State)
            {
                case State.GRABBING:
                    manager.transform.position = Vector3.Lerp(m_InitialPoint, m_TargetPoint, m_CurrentTime);
                    manager.transform.rotation = Quaternion.Slerp(m_InitialRotation, m_TargetRotation, m_CurrentTime);
                    if (m_CurrentTime > m_GrabTime || Vector3.Distance(manager.transform.position, m_TargetPoint) < 0.02f)
                    {
                        m_State = State.ON;
                    }
                    break;
                default:

                    break;
                case State.ON:
                    updateOn();
                    break;
            }

            
        }

        void updateOn()
        {

        }


        
    }
}