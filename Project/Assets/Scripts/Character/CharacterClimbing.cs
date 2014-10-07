using UnityEngine;
using System.Collections;

namespace EndevGame
{

    #region
    /* October,6,2014 - Nathan Hanlan - Added and implemented the first iteration of the CharacterClimbing
     * October,7,2014 - Nathan Hanlan - Added and implemented the onAnimation Method
    */
    #endregion
    /// <summary>
    /// Controls how the character moves when climbing on a 'Climbable' object. See Character_TDD for more information.
    /// </summary>
    public class CharacterClimbing : CharacterComponent
    {
        private enum State
        {
            NONE, //The character is not using a climbable object
            IDLE, //The character is using the climbable object but processing input
            GRABBING, //The character is currently in the motion to grab onto the climbable object
            CLIMB_UP_FINISH,//The character is currently climbing up and ontop of the climbable object


            //The character is processing to climb in a direction
            CLIMB_LEFT,
            CLIMB_RIGHT,
            CLIMB_UP,
            CLIMB_DOWN,

            //The character is moving towards their target in a direction
            CLIMBING_LEFT,
            CLIMBING_RIGHT,
            CLIMBING_UP,
            CLIMBING_DOWN
        }
        #region Fields
        /// <summary>
        /// The object currently being used to climb
        /// </summary>
        private Climbable m_ClimbableObject = null;
        
        //Only Serialize this data for the editor for debugging.
#if UNITY_EDITOR
        /// <summary>
        /// The initial position of the character at climb
        /// </summary>
        [SerializeField]
        private Vector3 m_InitialPosition = Vector3.zero;
        /// <summary>
        /// The initial rotation of the character at climb
        /// </summary>
        [SerializeField]
        private Quaternion m_InitialRotation = Quaternion.identity;
        /// <summary>
        /// The target position of the character at climb
        /// </summary>
        [SerializeField]
        private Vector3 m_TargetPosition = Vector3.zero;
        /// <summary>
        /// The target rotation of the character at climb
        /// </summary>
        [SerializeField]
        private Quaternion m_TargetRotation = Quaternion.identity;
#else
        /// <summary>
        /// The initial position of the character at climb
        /// </summary>
        private Vector3 m_InitialPosition = Vector3.zero;
        /// <summary>
        /// The initial rotation of the character at climb
        /// </summary>
        private Quaternion m_InitialRotation = Quaternion.identity;
        /// <summary>
        /// The target position of the character at climb
        /// </summary>
        private Vector3 m_TargetPosition = Vector3.zero;
        /// <summary>
        /// The target rotation of the character at climb
        /// </summary>
        private Quaternion m_TargetRotation = Quaternion.identity;
#endif

        /// <summary>
        /// Determines whether or not the character falls off upon reaching the end of a climbable object.
        /// </summary>
        [SerializeField]
        private bool m_FallOff = false;
        /// <summary>
        /// The current time passed for a character motion. (Initial Grab, Climbing left,right up or down)
        /// </summary>
        [SerializeField]
        private float m_CurrentTime = 0.0f;
        /// <summary>
        /// The offset distance the character should be from the target position for grabbing
        /// </summary>
        [SerializeField]
        private float m_GrabOffset = 0.0f;
        /// <summary>
        /// How fast should the character grab onto the object
        /// </summary>
        [SerializeField]
        private float m_GrabSpeed = 3.0f;

        /// <summary>
        /// The current state of the character within the Climbing state machine.
        /// </summary>
        [SerializeField]
        private State m_State = State.NONE;


        /// <summary>
        /// The amount of input time it takes to try and climb to a side.
        /// </summary>
        [SerializeField]
        private float m_ClimbInputSpeed = 0.5f;
        [SerializeField]
        private float m_ClimbSpeed = 1.0f;
        [SerializeField]
        private float m_ClimbDistance = 2.0f;
        [SerializeField]
        private float m_CharacterHeight = 1.72f;
        /// <summary>
        /// The offset to use to climb up an object
        /// </summary>
        [SerializeField]
        private Vector2 m_ClimbUpOffset = new Vector2(0.4f, 0.5f);



        //Input delay variables
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
        /// <summary>
        /// Initializes the character climbing component
        /// </summary>
        private void Start()
        {
            init();
        }


        /// <summary>
        /// Begins the climbing process
        /// </summary>
        /// <param name="aClimbableObject"></param>
        public void startClimbing(Climbable aClimbableObject)
        {
             //If target object is null or were currently climbing exit out
            if(aClimbableObject == null || m_ClimbableObject != null)
            {
                return;
            }
            //Grab the transform for
            Transform climbTransform = aClimbableObject.transform;

            //Rotate the current rotation by 180 degrees
            Vector3 eulerAngles = climbTransform.rotation.eulerAngles;
            eulerAngles.y += 180.0f;
            Quaternion rotation = Quaternion.Euler(eulerAngles);

            //Calculate a distance
            CapsuleCollider capsuleCollider = manager.GetComponent<CapsuleCollider>();
            float distance = capsuleCollider.radius;


            //Move forwards in the inverse direction of the climbable objects rotation
            Vector3 forward = Vector3.forward * distance;
            forward.z += m_GrabOffset;
            Vector3 direction = rotation * forward;
            
            //Store the initial state
            m_InitialPosition = manager.transform.position;
            m_InitialRotation = manager.transform.rotation;

            //Calculate the target state
            m_TargetRotation = rotation;
            m_TargetPosition = new Vector3(manager.transform.position.x + direction.x, manager.transform.position.y, manager.transform.position.z + direction.z);

            //Reset the time
            m_CurrentTime = 0.0f;

           
            //Begin transition to grabbing onto the object
            m_State = State.GRABBING;
            m_ClimbableObject = aClimbableObject;
            m_ClimbableObject.grab(this);

            //Lock the character motor
            lockMovement = true;
            lockGravity = true;
            lockRotation = true;
            if (characterMotor != null)
            {
                characterMotor.resetVelocity();
                characterMotor.disableRigidbody();
            }
            if(characterAnimation != null)
            {
                characterAnimation.setState(CharacterAnimationState.CLIMBING);
            }
        }

        /// <summary>
        /// Helper method used to release the character from the locked state. Does not release the used object
        /// </summary>
        private void release()
        {
            lockMovement = false;
            lockGravity = false;
            lockRotation = false;
            m_State = State.NONE;
            if(characterMotor != null)
            {
                characterMotor.enableRigidbody();
            }
            if(characterAnimation != null)
            {
                characterAnimation.releaseState(CharacterAnimationState.CLIMBING);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void Update()
        {
            
            base.Update();
            ///TODO: Change this key code to whatever you want the end-user to press to release.
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (m_ClimbableObject != null)
                {
                    m_ClimbableObject.release(this);
                    m_ClimbableObject = null;
                    if(characterInteraction == null)
                    {
#if UNITY_EDITOR
                        Debug.LogWarning("Missing a \'Character Interaction\' on this Character(" + manager.gameObject.animation + ").");
#endif
                    }
                    else
                    {
                        characterInteraction.releaseOverride();
                    }
                    release();
                }
            }

            switch(m_State)
            {
                case State.GRABBING:
                    updateGrabbing();
                    break;
                case State.IDLE:
                    updateIdle();
                    break;
                case State.CLIMB_LEFT:
                    updateClimb(Direction.LEFT);
                    break;
                case State.CLIMB_RIGHT:
                    updateClimb(Direction.RIGHT);
                    break;
                case State.CLIMB_UP:
                    updateClimb(Direction.UP);
                    break;
                case State.CLIMB_DOWN:
                    updateClimb(Direction.DOWN);
                    break;
                case State.CLIMBING_LEFT:
                case State.CLIMBING_RIGHT:
                case State.CLIMBING_UP:
                case State.CLIMBING_DOWN:
                    updateClimbing();
                    break;
                case State.CLIMB_UP_FINISH:
                    updateClimbingUpFinish();
                    break;
            }
        }

        private void updateIdle()
        {
            ///Check forward motion state
            if (forwardMotion != 0.0f)
            {
                ///Increase time to go up
                if (forwardMotion > 0.0f)
                {
                    if (m_InputUp == false)
                    {
                        m_InputTime.y = 0.0f;
                    }
                    m_InputUp = true;
                    m_InputTime.y += Time.deltaTime;
                    m_InputDown = false;
                }
                ///Increase time to go down
                else if (forwardMotion < 0.0f)
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
            else //Reset time
            {
                m_InputTime.y = 0.0f;
                m_InputUp = false;
                m_InputDown = false;
            }
            ///Check the side motoin
            if (sideMotion != 0.0f)
            {
                //Increase time to go left
                if (sideMotion < 0.0f)
                {
                    if (m_InputLeft == false)
                    {
                        m_InputTime.x = 0.0f;
                    }
                    m_InputLeft = true;
                    m_InputTime.x += Time.deltaTime;
                    m_InputRight = false;
                }
                //Increase time to go right
                else if (sideMotion > 0.0f)
                {
                    if (m_InputRight == false)
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

            //Change states if enough input time has passed.
            if(m_InputTime.y > m_ClimbInputSpeed)
            {
                if(m_InputDown == true)
                {
                    m_State = State.CLIMB_DOWN;
                }
                else if(m_InputUp == true)
                {
                    m_State = State.CLIMB_UP;
                }
                m_InputTime.y -= m_ClimbInputSpeed * 0.3f;
            }
            else if(m_InputTime.x > m_ClimbInputSpeed)
            {
                if (m_InputLeft == true)
                {
                    m_State = State.CLIMB_LEFT;
                }
                else if (m_InputRight == true)
                {
                    m_State = State.CLIMB_RIGHT;
                }
                m_InputTime.x -= m_ClimbInputSpeed * 0.3f;
            }


        }
        private void updateGrabbing()
        {
            m_CurrentTime += Time.deltaTime * m_GrabSpeed;
            manager.transform.position = Vector3.Lerp(m_InitialPosition, m_TargetPosition, m_CurrentTime);
            manager.transform.rotation = Quaternion.Slerp(m_InitialRotation, m_TargetRotation, m_CurrentTime);
            if (m_CurrentTime > 1.0f || Vector3.Distance(manager.transform.position, m_TargetPosition) < 0.02f)
            {
                m_State = State.IDLE;
            }


        }
        private void updateClimbingUpFinish()
        {
            m_CurrentTime += Time.deltaTime * m_ClimbSpeed;
            manager.transform.position = Vector3.Lerp(m_InitialPosition, m_TargetPosition, m_CurrentTime);
            if (m_CurrentTime > 1.0f || Vector3.Distance(manager.transform.position, m_TargetPosition) < 0.02f)
            {
                if(m_ClimbableObject != null)
                {
                    m_ClimbableObject.release(this);
                    m_ClimbableObject = null;
                }
                if(characterInteraction != null)
                {
                    characterInteraction.releaseOverride();
                }
                release();
                m_State = State.NONE;
            }
        }
        /// <summary>
        /// Updates the climb left functionality of the character
        /// </summary>
        private void updateClimb(Direction aDirection)
        {
            //If there is no climbable object to use. Release the object
            if (m_ClimbableObject == null)
            {
                if (characterInteraction != null)
                {
                    characterInteraction.releaseOverride();
                }
                release();
                return;
            }


            //Check to see if we can move in the left direction
            Vector3 targetPosition = Vector3.zero;
            bool canMove = m_ClimbableObject.checkSide(manager.transform, m_ClimbDistance, aDirection, out targetPosition);

            //If we can set the state to move left
            if (canMove == true)
            {
                m_InitialPosition = manager.transform.position;
                m_TargetPosition = targetPosition;
                switch (aDirection)
                {
                    case Direction.LEFT:
                        m_State = State.CLIMBING_LEFT;
                        break;
                    case Direction.RIGHT:
                        m_State = State.CLIMBING_RIGHT;
                        break;
                    case Direction.UP:
                        m_State = State.CLIMBING_UP;
                        break;
                    case Direction.DOWN:
                        m_State = State.CLIMBING_DOWN;
                        break;
                }
                m_CurrentTime = 0.0f;
            }
            else
            {
                Debug.Log("Raycast hit object");

                bool releaseObject = false;
                //if(aDirection == Direction.UP)
                //{
                //    Debug.Log(Vector3.Distance(targetPosition, manager.transform.position));
                //    releaseObject = Vector3.Distance(targetPosition, manager.transform.position) < 0.02f + characterHeight;
                //}
                //else
                //{
                    releaseObject = Vector3.Distance(targetPosition, manager.transform.position) < 0.02f;
                //}
                //if we cant check the distance
                if (releaseObject)
                {
#region CLIMB FINISHED
                    switch(aDirection)
                    {
                        case Direction.LEFT:
                        case Direction.RIGHT:
                            //If were below the clamp change then release the climbable object if were need to fall off otherwise go back to the idle state
                            if(m_FallOff == true)
                            {
                                if(m_ClimbableObject != null)
                                {
                                    m_ClimbableObject.release(this);
                                    m_ClimbableObject = null;
                                }
                                if(characterInteraction != null)
                                {
                                    characterInteraction.releaseOverride();
                                }
                                release();
                            }
                            else
                            {
                                m_State = State.IDLE;
                            }
                            break;
                        case Direction.DOWN: //release from the object
                            if(m_ClimbableObject != null)
                            {
                                m_ClimbableObject.release(this);
                                m_ClimbableObject = null;
                            }
                            if(characterInteraction != null)
                            {
                                characterInteraction.releaseOverride();
                            }
                            release();
                            break;
                        case Direction.UP: //finish the climb
                            {
                                Debug.Log("Climb Up");
                                Vector3 climbOffset = manager.transform.rotation * new Vector3(0.0f,0.0f,m_ClimbUpOffset.x );
                                m_InitialPosition = manager.transform.position;
                                m_TargetPosition = manager.transform.position + climbOffset;
                                m_TargetPosition.y = manager.transform.position.y + characterHeight;
                                m_CurrentTime = 0.0f;
                                m_State = State.CLIMB_UP_FINISH;
                            }
                            break;
                    }
                    
#endregion
                }
                else //We still have some distance to go so get the character moving towards that area.
                {
                    m_InitialPosition = manager.transform.position;
                    m_TargetPosition = targetPosition;
                    switch(aDirection)
                    {
                        case Direction.LEFT:
                            m_State = State.CLIMBING_LEFT;
                        break;
                        case Direction.RIGHT:
                            m_State = State.CLIMBING_RIGHT;
                        break;
                        case Direction.UP:
                            m_State = State.CLIMBING_UP;
                        break;
                        case Direction.DOWN:
                            m_State = State.CLIMBING_DOWN;
                        break;
                    }
                    m_CurrentTime = 0.0f;
                }
            }
        }
        private void updateClimbing()
        {
            m_CurrentTime += Time.deltaTime * m_ClimbSpeed;
            manager.transform.position = Vector3.Lerp(m_InitialPosition, m_TargetPosition, m_CurrentTime);
            if (m_CurrentTime > 1.0f || Vector3.Distance(manager.transform.position, m_TargetPosition) < 0.02f)
            {
                m_State = State.IDLE;
            }
        }

        public override void onAnimateCharacter(CharacterAnimation aAnimation)
        {
            if (aAnimation == null)
            {
                return;
            }
            Animation animation = aAnimation.animationComponent;

            if (animation == null)
            {
                return;
            }

            switch(m_State)
            {
                case State.CLIMBING_LEFT:
                    animation.CrossFade(CharacterAnimation.ANIMATION_CLIMB_LEFT);
                    break;
                case State.CLIMBING_RIGHT:
                    animation.CrossFade(CharacterAnimation.ANIMATION_CLIMB_RIGHT);
                    break;
                case State.CLIMBING_UP:
                    animation.CrossFade(CharacterAnimation.ANIMATION_CLIMB_UP);
                    break;
                case State.CLIMBING_DOWN:
                    animation.CrossFade(CharacterAnimation.ANIMATION_CLIMB_DOWN);
                    break;
                case State.NONE:

                    break;
                case State.CLIMB_UP_FINISH:
                    animation.CrossFade(CharacterAnimation.ANIMATION_IDLE);
                    break;
                default:
                    animation.CrossFade(CharacterAnimation.ANIMATION_CLIMB_IDLE);
                    break;
            }

        }

        #endregion

        public float characterHeight
        {
            get { return m_CharacterHeight; }
        }

    }
}