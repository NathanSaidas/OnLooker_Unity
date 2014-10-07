using UnityEngine;
using System.Collections;

namespace EndevGame
{

    #region
    /* October,7,2014 - Nathan Hanlan - Added a method to handle animation control.
    *  
    */
    #endregion
    /// <summary>
    /// Enables the player to grab onto ledges and interact with them.
    /// </summary>
    public class CharacterLedgeGrab : CharacterComponent
    {
        enum State
        {
            NONE,
            GRABBING,
            IDLE,
            CLIMB_LEFT,
            CLIMB_RIGHT,
            CLIMBING_LEFT,
            CLIMBING_RIGHT,
            CLIMB_UP,
        }

        #region Fields
        /// <summary>
        /// The current ledge the player is grabbing
        /// </summary>
        private LedgeGrab m_TriggeringLedgeGrab = null;

        /// <summary>
        /// The target point the character has to reach to get to the ledge
        /// </summary>
        [SerializeField]
        private Vector3 m_TargetPosition = Vector3.zero;
        /// <summary>
        /// The initial point the character was at. (At the time of collision)
        /// </summary>
        [SerializeField]
        private Vector3 m_InitialPosition = Vector3.zero;


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
        //[SerializeField]
        //private float m_GrabTime = 1.0f;
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
        /// The current time passed for the character grabbing the ledge. (State.GRABBING) or climbing (left or right)
        /// </summary>
        [SerializeField]
        private float m_CurrentTime = 0.0f;

        /// <summary>
        /// The amount of input time it takes to try and climb to a side.
        /// </summary>
        [SerializeField]
        private float m_ClimbInputSpeed = 0.5f;
        /// <summary>
        /// The speed at which the character climbs at.
        /// </summary>
        [SerializeField]
        private float m_ClimbSpeed = 1.0f;
        /// <summary>
        /// A distance variable to determine how far the character climbs in the left or right direction.
        /// </summary>
        [SerializeField]
        private float m_ClimbDistance = 2.0f;
        /// <summary>
        /// The height offset used for climbing up an object. (The x is used for the z axis)
        /// </summary>
        [SerializeField]
        private Vector2 m_ClimbUpOffset = new Vector2(0.3f, 0.5f);
        /// <summary>
        /// Determines whether or not the character falls off upon reaching the end.
        /// </summary>
        [SerializeField]
        private bool m_FallOff = false;

        //Input Delay Variables
        [SerializeField]
        private Vector2 m_InputTime = Vector2.zero;
        [SerializeField]
        private bool m_InputLeft = false;
        [SerializeField]
        private bool m_InputRight = false;
        [SerializeField]
        private bool m_InputUp = false;
        [SerializeField]
        private bool m_InputDown = false;

        #endregion


        #region Methods
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
            if(ledgeGrab.grab(this) == false)
            {
                return;
            }
            Vector3 eulerAngles = ledgeGrab.transform.rotation.eulerAngles;
            eulerAngles.y += 180.0f;
            Quaternion rotation = Quaternion.Euler(eulerAngles);

            //Upon colliding with a ledge
            //Calculate the distance forwards towards the triggering ledge
            CapsuleCollider capsuleCollider = manager.GetComponent<CapsuleCollider>();
            Vector3 ledgeSize = ledgeGrab.size;
            float distance = capsuleCollider.radius + ledgeSize.z * 0.5f;

            //Create a forward vector and rotate it to face the ledge and scale the vector by the distance calculated.
            Vector3 forward = Vector3.forward * distance;
            forward.z += m_GrabOffset;
            Vector3 direction = rotation * forward;

            //Calculate the target point, and store the initial state of the character
            m_TargetPosition = new Vector3(manager.transform.position.x + direction.x, ledgeGrab.transform.position.y - m_CharacterHeight, manager.transform.position.z + direction.z);
            m_TargetRotation = rotation;
            m_InitialPosition = manager.transform.position;
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
            characterMotor.disableRigidbody();
            characterAnimation.setState(CharacterAnimationState.CLIMBING_LEDGE);
        }

        protected override void Update()
        {
            base.Update();
            ///TODO: Change this key code to whatever you want the end-user to press to release.
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                if(m_TriggeringLedgeGrab != null)
                {
                    m_TriggeringLedgeGrab.release(this);
                    m_TriggeringLedgeGrab = null;
                    release();
                }
            }

            

            switch(m_State)
            {
                case State.GRABBING:
                    m_CurrentTime += Time.deltaTime * m_GrabSpeed;
                    manager.transform.position = Vector3.Lerp(m_InitialPosition, m_TargetPosition, m_CurrentTime);
                    manager.transform.rotation = Quaternion.Slerp(m_InitialRotation, m_TargetRotation, m_CurrentTime);
                    if (m_CurrentTime > 1.0f || Vector3.Distance(manager.transform.position, m_TargetPosition) < 0.02f)
                    {
                        m_State = State.IDLE;
                    }
                    break;
                case State.CLIMBING_LEFT:
                    m_CurrentTime += Time.deltaTime * m_ClimbSpeed;
                    updateClimbingLeft();
                    break;
                case State.CLIMBING_RIGHT:
                    m_CurrentTime += Time.deltaTime * m_ClimbSpeed;
                    updateClimbingRight();
                    break;
                case State.CLIMB_LEFT:
                    updateClimbLeft();
                    break;
                case State.CLIMB_RIGHT:
                    updateClimbRight();
                    break;
                case State.IDLE:
                    updateIdle();
                    break;
                case State.CLIMB_UP:
                    m_CurrentTime += Time.deltaTime * m_ClimbSpeed;
                    updateClimbingUp();
                    break;


                default:

                    break;
            }

            
        }

        /// <summary>
        /// Release the locks on the character motor and set the state to none.
        /// </summary>
        void release()
        {
            lockMovement = false;
            lockGravity = false;
            lockRotation = false;
            m_State = State.NONE;
            if (characterMotor != null)
            {
                characterMotor.enableRigidbody();
            }
            if (characterAnimation != null)
            {
                characterAnimation.releaseState(CharacterAnimationState.CLIMBING_LEDGE);
            }
        }


        /// <summary>
        /// This method gets invoked by the CharacterAnimation component to give us a chance to animate the character while they are climbing.
        /// </summary>
        /// <param name="aAnimation"></param>
        public override void onAnimateCharacter(CharacterAnimation aAnimation)
        {
            if(aAnimation == null)
            {
                return;
            }
            Animation animation = aAnimation.animationComponent;

            if(animation == null)
            {
                return;
            }

            switch(m_State)
            {
                case State.CLIMBING_LEFT:
                    animation.CrossFade(CharacterAnimation.ANIMATION_CLIMB_LEFT,0.3f);
                    break;
                case State.CLIMBING_RIGHT:
                    animation.CrossFade(CharacterAnimation.ANIMATION_CLIMB_RIGHT, 0.3f);
                    break;
                case State.CLIMB_UP:
                    animation.CrossFade(CharacterAnimation.ANIMATION_IDLE, 0.3f);
                    break;
                case State.NONE:
                    aAnimation.releaseState(CharacterAnimationState.CLIMBING_LEDGE);
                    break;
                    //animate idle for everything else
                default:
                    animation.CrossFade(CharacterAnimation.ANIMATION_CLIMB_IDLE, 0.3f);
                    break;
            }

        }

        //This state checks for input then transitions to the other states
        void updateIdle()
        {
            if(forwardMotion != 0.0f)
            {
                if(forwardMotion > 0.0f)
                {
                    if(m_InputUp == false)
                    {
                        m_InputTime.y = 0.0f;
                    }
                    m_InputUp = true;
                    m_InputTime.y += Time.deltaTime;
                    m_InputDown = false;
                }
                else if(forwardMotion < 0.0f)
                {
                    if (m_InputDown == false)
                    {
                        m_InputTime.y = 0.0f;
                    }
                    m_InputDown = true;
                    m_InputTime.y += Time.deltaTime;
                    m_InputUp = false;
                }
            }
            else
            {
                m_InputTime.y = 0.0f;
                m_InputUp = false;
                m_InputDown = false;
            }
            if(sideMotion != 0.0f)
            {
                if(sideMotion < 0.0f)
                {
                    if(m_InputLeft == false)
                    {
                        m_InputTime.x = 0.0f;
                    }
                    m_InputLeft = true;
                    m_InputTime.x += Time.deltaTime;
                    m_InputRight = false;
                }
                else if(sideMotion > 0.0f)
                {
                    if(m_InputRight == false)
                    {
                        m_InputTime.x = 0.0f;
                    }
                    m_InputRight = true;
                    m_InputTime.x += Time.deltaTime;
                    m_InputLeft = false;
                }
            }
            else
            {
                m_InputTime.x = 0.0f;
                m_InputRight = false;
                m_InputLeft = false;
            }

            if(m_InputTime.y > m_ClimbInputSpeed)
            {
                if(m_InputDown == true)
                {
                    if (m_TriggeringLedgeGrab != null)
                    {
                        m_TriggeringLedgeGrab.release(this);
                        m_TriggeringLedgeGrab = null;
                    }
                    release();
                }
                else if(m_InputUp == true)
                {
                    m_State = State.CLIMB_UP;
                    Vector3 climbOffset = manager.transform.rotation * new Vector3(0.0f, 0.0f, m_ClimbUpOffset.x);
                    climbOffset.y = m_ClimbUpOffset.y + m_CharacterHeight;

                    m_InitialPosition = manager.transform.position;
                    m_TargetPosition = manager.transform.position + climbOffset;
                    m_CurrentTime = 0.0f;
                    

                    //m_TargetPosition.y = m_TriggeringLedgeGrab.transform.position.y + m_TriggeringLedgeGrab.size.y + m_ClimbUpOffset.y;
                    
                }
                m_InputTime = Vector2.zero;
                    
            }
            else if(m_InputTime.x > m_ClimbInputSpeed)
            {
                if(m_InputLeft == true)
                {
                    m_State = State.CLIMB_LEFT;
                }
                else if(m_InputRight == true)
                {
                    m_State = State.CLIMB_RIGHT;
                    
                }
                else //this should never happen but if it does it will be good to nkow
                {
                    Debug.LogWarning("Input time has surpassed move time however the left or right axis was not pressed.");
                }
                //Reduce the time passed by a small buffer
                m_InputTime.x -= m_ClimbInputSpeed * 0.3f;
            }

        }

        private void updateClimbLeft()
        {
            if(m_TriggeringLedgeGrab == null)
            {
                release();
                return;
            }
            //Check to see if we can move left. If we can move to the target point. 
            //If we cant check to see if there is a sibling ledge. Move to that target point.
            Vector3 targetPosition = Vector3.zero;
            bool canMove = m_TriggeringLedgeGrab.checkSide(manager.transform, m_ClimbDistance, true,out targetPosition);
            if(canMove == true)
            {
                //Climb left
                m_InitialPosition = manager.transform.position;
                m_TargetPosition = targetPosition;
                m_State = State.CLIMBING_LEFT;
                m_CurrentTime = 0.0f;
            }
            else
            {
                //If the distance between the two points is less than this buffer do one of the two
                if (Vector3.Distance(targetPosition, manager.transform.position) < 0.02f)
                {
                    //release from ledge
                    if (m_FallOff == true)
                    {
                        if (m_TriggeringLedgeGrab != null)
                        {
                            m_TriggeringLedgeGrab.release(this);
                            m_TriggeringLedgeGrab = null;
                        }
                        release();
                    }
                    else
                    {
                        m_State = State.IDLE;
                    }
                }
                else //Else move to the target
                {
                    //Climb right
                    m_InitialPosition = manager.transform.position;
                    m_TargetPosition = targetPosition;
                    m_State = State.CLIMBING_LEFT;
                    m_CurrentTime = 0.0f;
                }
            }

        }
        private void updateClimbRight()
        {
            //Check to see if we can move right. If we can move to the target point. 
            //If we cant check to see if there is a sibling ledge. Move to that target point.
            Vector3 targetPosition = Vector3.zero;
            bool canMove = m_TriggeringLedgeGrab.checkSide(manager.transform, m_ClimbDistance, false, out targetPosition);
            if(canMove == true)
            {
                //Climb right
                m_InitialPosition = manager.transform.position;
                m_TargetPosition = targetPosition;
                m_State = State.CLIMBING_RIGHT;
                m_CurrentTime = 0.0f;
            }
            else
            { 
                //If the distance between the two points is less than this buffer do one of the two
                if (Vector3.Distance(targetPosition, manager.transform.position) < 0.02f)
                {
                    //release from ledge
                    if (m_FallOff == true)
                    {
                        if(m_TriggeringLedgeGrab != null)
                        {
                            m_TriggeringLedgeGrab.release(this);
                            m_TriggeringLedgeGrab = null;
                        }
                        release();
                    }
                    else
                    {
                        m_State = State.IDLE;
                    }
                }
                else //Else move to the target
                {
                    //Climb right
                    m_InitialPosition = manager.transform.position;
                    m_TargetPosition = targetPosition;
                    m_State = State.CLIMBING_RIGHT;
                    m_CurrentTime = 0.0f;
                }
                
            }
        }

        private void updateClimbingLeft()
        {
            manager.transform.position = Vector3.Lerp(m_InitialPosition, m_TargetPosition, m_CurrentTime);
            if (m_CurrentTime > 1.0f || Vector3.Distance(manager.transform.position, m_TargetPosition) < 0.02f)
            {
                m_State = State.IDLE;
            }
        }
        private void updateClimbingRight()
        {
            manager.transform.position = Vector3.Lerp(m_InitialPosition, m_TargetPosition, m_CurrentTime);
            if (m_CurrentTime > 1.0f || Vector3.Distance(manager.transform.position, m_TargetPosition) < 0.02f)
            {
                m_State = State.IDLE;
            }
        }
        private void updateClimbingUp()
        {
            manager.transform.position = Vector3.Lerp(m_InitialPosition, m_TargetPosition, m_CurrentTime);
            if(m_CurrentTime > 1.0f || Vector3.Distance(manager.transform.position, m_TargetPosition) < 0.02f )
            {
                if(m_TriggeringLedgeGrab != null)
                {
                    m_TriggeringLedgeGrab.release(this);
                    m_TriggeringLedgeGrab = null;
                }
                release();
                m_State = State.NONE;
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// The current ledge the player is grabbing
        /// </summary>
        public LedgeGrab triggeringLedgeGrab
        {
            get { return m_TriggeringLedgeGrab; }
        }

        /// <summary>
        /// The target point the character has to reach to get to the ledge
        /// </summary>
        public Vector3 targetPosition
        {
            get { return m_TargetPosition; }
        }
        /// <summary>
        /// The initial point the character was at. (At the time of collision)
        /// </summary>
        public Vector3 initialPosition
        {
            get { return m_InitialPosition; }
        }


        /// <summary>
        /// The target rotation the character has to reach
        /// </summary>
        public Quaternion targetRotation
        {
            get { return m_TargetRotation; }
        }
        /// <summary>
        /// The initial rotation the character was at. (At the time of collision)
        /// </summary>
        public Quaternion initialRotation
        {
            get { return m_InitialRotation; }
        }

        /// <summary>
        /// An offset used to set the characters y position correctly relative to how high the character is.
        /// </summary>
        public float characterHeight
        {
            get { return m_CharacterHeight; }
        }
        /// <summary>
        /// An offset used to set the characters distance from the ledge. (Good for hand placement)
        /// </summary>
        public float grabOffset
        {
            get { return m_GrabOffset; }
        }
        /// <summary>
        /// How much time must be allowed to pass before the next state
        /// </summary>
        //public float grabTime
        //{
        //    get { return m_GrabTime; }
        //}
        /// <summary>
        /// How fast should the character grab onto the ledge
        /// </summary>
        public float grabSpeed
        {
            get { return m_GrabSpeed; }
        }
        /// <summary>
        /// The current time passed for the character grabbing the ledge. (State.GRABBING) or climbing (left or right)
        /// </summary>
        public float currentTime
        {
            get { return m_CurrentTime; }
        }

        /// <summary>
        /// The amount of input time it takes to try and climb to a side.
        /// </summary>
        public float climbInputSpeed
        {
            get{return m_ClimbInputSpeed;}
        }
        public float climbSpeed
        {
            get { return m_ClimbSpeed; }
        }
        /// <summary>
        /// A distance variable to determine how far the character climbs in the left or right direction.
        /// </summary>
        public float climbDistance
        {
            get{return m_ClimbDistance;}
        }
        /// <summary>
        /// Determines whether or not the character falls off upon reaching the end.
        /// </summary>
        public bool fallsOff 
        {
            get{return m_FallOff;}
        }
        public Vector2 inputTime 
        {
            get{return m_InputTime;}
        }
        public bool inputLeft
        {
            get{return m_InputLeft;}
        }
        public bool inputRight
        {
            get { return m_InputRight; }
        }
        #endregion


    }
}