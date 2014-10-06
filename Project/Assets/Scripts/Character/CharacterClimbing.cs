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
                        characterInteraction.releaseUsedObject();
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

        }
        private void updateGrabbing()
        {

        }
        private void updateClimbingUpFinish()
        {

        }
        private void updateClimbLeft()
        {

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