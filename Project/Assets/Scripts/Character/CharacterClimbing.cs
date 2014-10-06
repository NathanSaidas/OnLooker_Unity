using UnityEngine;
using System.Collections;

namespace EndevGame
{
    public class CharacterClimbing : CharacterComponent
    {
        private enum State
        {
            NONE,
            IDLE,
            GRABBING,
            CLIMB_UP_FINISH,
            CLIMB_LEFT,
            CLIMB_RIGHT,
            CLIMB_UP,
            CLIMB_DOWN,
            CLIMBING_LEFT,
            CLIMBING_RIGHT,
            CLIMBING_UP,
            CLIMBING_DOWN
        }

        private Climbable m_ClimbableObject = null;
        

        private Vector3 m_InitialPosition = Vector3.zero;
        private Quaternion m_InitialRotation = Quaternion.identity;
        private Vector3 m_TargetPosition = Vector3.zero;
        private Quaternion m_TargetRotation = Quaternion.identity;


        private bool m_FallOff = false;
        /// <summary>
        /// The current time passed for a character motion. (Initial Grab, Climbing left,right up or down)
        /// </summary>
        private float m_CurrentTime = 0.0f;
        /// <summary>
        /// The offset distance the character should be from the target position for grabbing
        /// </summary>
        private float m_GrabOffset = 0.0f;
        /// <summary>
        /// How fast should the character grab onto the object
        /// </summary>
        private float m_GrabSpeed = 3.0f;

        private State m_State = State.NONE;


        /// <summary>
        /// The amount of input time it takes to try and climb to a side.
        /// </summary>
        [SerializeField]
        private float m_ClimbInputSpeed = 0.5f;
        [SerializeField]
        private float m_ClimbSpede = 1.0f;
        [SerializeField]
        private float m_ClimbDistance = 2.0f;



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

        private void Start()
        {
            init();
        }


        public void startClimbing(Climbable aClimbableObject)
        {
            if(aClimbableObject == null || m_ClimbableObject != null)
            {
                return;
            }
            Transform climbTransform = aClimbableObject.transform;

            Vector3 eulerAngles = climbTransform.rotation.eulerAngles;
            eulerAngles.y += 180.0f;
            Quaternion rotation = Quaternion.Euler(eulerAngles);

            CapsuleCollider capsuleCollider = manager.GetComponent<CapsuleCollider>();
            float distance = capsuleCollider.radius;

            Vector3 forward = Vector3.forward * distance;
            forward.z += m_GrabOffset;
            Vector3 direction = rotation * forward;

            m_InitialPosition = manager.transform.position;
            m_InitialRotation = manager.transform.rotation;

            m_TargetRotation = rotation;
            m_TargetPosition = new Vector3(manager.transform.position.x + direction.x, manager.transform.position.y, manager.transform.position.z + direction.z);

            m_CurrentTime = 0.0f;

           
            
            m_State = State.GRABBING;

            m_ClimbableObject = aClimbableObject;
            m_ClimbableObject.grab(this);

            //Lock the character motor
            lockMovement = true;
            lockGravity = true;
            lockRotation = true;
            characterMotor.resetVelocity();
            characterMotor.disableRigidbody();
            
        }

        private void release()
        {
            lockMovement = false;
            lockGravity = false;
            lockRotation = false;
            m_State = State.NONE;
            characterMotor.enableRigidbody();
        }

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
                        Debug.LogWarning("Missing a \'Character Interaction\' on this Character(" + manager.gameObject.animation + ").");
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
                    m_CurrentTime += Time.deltaTime * m_GrabSpeed;
                    manager.transform.position = Vector3.Lerp(m_InitialPosition, m_TargetPosition, m_CurrentTime);
                    manager.transform.rotation = Quaternion.Slerp(m_InitialRotation, m_TargetRotation, m_CurrentTime);
                    if(m_CurrentTime > 1.0f || Vector3.Distance(manager.transform.position, m_TargetPosition) < 0.02f)
                    {
                        m_State = State.IDLE;
                    }
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
            


        }
        private void updateClimbingUpFinish()
        {

        }
        private void updateClimbLeft()
        {
            if (m_ClimbableObject == null)
            {
                if (characterInteraction != null)
                {
                    characterInteraction.releaseOverride();
                }
                release();
                return;
            }

            Vector3 targetPosition = Vector3.zero;
            bool canMove = m_ClimbableObject.checkHorizontalSide(manager.transform, m_ClimbDistance, true, out targetPosition);

            if (canMove == true)
            {
                m_InitialPosition = manager.transform.position;
                m_TargetPosition = targetPosition;
                m_State = State.CLIMBING_LEFT;
                m_CurrentTime = 0.0f;
            }
            else
            {
                if(Vector3.Distance(targetPosition,manager.transform.position) < 0.02f)
                {
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
                }
                else
                {
                    m_InitialPosition = manager.transform.position;
                    m_TargetPosition = targetPosition;
                    m_State = State.CLIMBING_LEFT;
                    m_CurrentTime = 0.0f;
                }
            }
        }
        private void updateClimbRight()
        {

        }
        private void updateClimbUp()
        {

        }
        private void updateClimbDown()
        {

        }
        private void updateClimbingLeft()
        {

        }
        private void updateClimbingRight()
        {

        }
        private void updateClimbingUp()
        {

        }
        private void updateClimbingDown()
        {

        }


    }
}