using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gem
{
    public class CharacterMotor : MonoBehaviour
    {
        #region ANIMATION HASH IDS
        private int m_ForwardID = 0;
        private int m_SideID = 0;
        private int m_IsAttackingID = 0;
        private int m_AttackSwordID = 0;
        private int m_AttackWandID = 0;
        private int m_AttackMotionID = 0;
        private int m_AttackGunID = 0;


        private const string ANIMATION_FORWARD = "pForwardSpeed";
        private const string ANIMATION_SIDE = "pSideSpeed";
        private const string ANIMATION_IS_ATTACKING = "pIsAttacking";
        private const string ANIMATION_ATTACK_SWORD = "pAttackSword";
        private const string ANIMATION_ATTACK_WAND = "pAttackWand";
        private const string ANIMATION_ATTACK_MOTION = "pAttackMotion";
        private const string ANIMATION_ATTACK_GUN = "pAttackGun";
        #endregion
        /// <summary>
        /// The characters camera
        /// </summary>
        private CharacterCamera m_CharacterCamera = null;
        /// <summary>
        /// How much sway the camera makes while moving
        /// </summary>
        [SerializeField]
        private float m_CameraSwaySpeed = 2.5f;
        private Animator m_Animator = null;
        private CharacterController m_CharacterController = null;
        /// <summary>
        /// The collision event receiver
        /// </summary>
        private CharacterCollisionHandler m_CollisionHandler = null;
        /// <summary>
        /// The character action sending events to this motor
        /// </summary>
        private CharacterAction m_CharacterAction = null;
        /// <summary>
        /// The unit data for this character
        /// </summary>
        private Unit m_Unit = null;
        /// <summary>
        /// The position for the camera
        /// </summary>
        [SerializeField]
        private Transform m_Head = null;
        /// <summary>
        /// The look rotation for the camera
        /// </summary>
        [SerializeField]
        private Transform m_VerticalLook = null;
        /// <summary>
        /// Look angle
        /// </summary>
        private float m_VerticalLookAngle = 0.0f;
        [SerializeField]
        private float m_MaxVerticalCameraAngle = 70.0f;
        [SerializeField]
        private bool m_EnabledMouseControl = true;
        [SerializeField]
        private float m_TurnSpeed = 10.0f;

        private Vector3 m_Velocity = Vector3.zero;
        [SerializeField]
        private float m_ChangeDirectionAccelerationThreshhold = 1.0f;
        [SerializeField]
        private float m_OnGroundStopEaseSpeed = 10.0f;
        [SerializeField]
        private float m_OnGroundMovementAcceleration = 10.0f;
        [SerializeField]
        private float m_GravityAcceleration = -9.81f;

        [SerializeField]
        private float m_InAirStopEaseSpeed = 2.0f;
        [SerializeField]
        private float m_InAirControlSpeed = 10.0f;
        [SerializeField]
        private float m_InAirMovementAcceleration = 30.0f;

        private bool m_AllowJump = true;

        ///Ground Detection
        private bool m_NearGround = false;
        private Vector3 m_GroundAngularVelocity = Vector3.zero;
        private Vector3 m_GroundVelocity = Vector3.zero;
        private float m_GroundDistance = 0.0f;
        [SerializeField]
        private float m_CheckGroundDistance = 0.5f;
        [SerializeField]
        private float m_OnGroundThreshholdDistance = 0.1f;
        private int m_GroundCheckMask = 0;

        //Jumping
        private float m_JumpMidAirTimeLeft = 0.0f;
        [SerializeField]
        private float m_JumpMidAirMaxTime = 0.3f;
        private float m_JumpTimeLeft = 0.0f;
        [SerializeField]
        private float m_JumpMaxHoldTime = 0.5f;
        [SerializeField]
        private float m_JumpSpeed = 10.0f;
        [SerializeField]
        private float m_JumpForce = 0.5f;


        //Forces
        private Vector3 m_NetExternalForce = Vector3.zero;
        [SerializeField]
        private float m_Mass = 1.0f;
        [SerializeField]
        private List<string> m_AcceptedForceTags = new List<string>();

        //Springs
        [SerializeField]
        private float m_CollisionSpringMaxLength = 1.0f;
        [SerializeField]
        private float m_CollisionSpringMaxForce = 5000.0f;
        [SerializeField]
        private float m_CollisionSpringDamping = 1.0f;
        [SerializeField]
        private float m_PushForceAmount = 400.0f;

        private AttackType m_AttackType = AttackType.NONE;
        private float m_AttackMotion = 0.0f;

        [SerializeField]
        private Vector3 m_InverseVelocity = Vector3.zero;

        // Use this for initialization
        void Start()
        {
            m_GroundCheckMask = ~LayerMask.GetMask("Player");

            m_CharacterController = GetComponent<CharacterController>();
            m_CharacterAction = GetComponent<CharacterAction>();
            m_Unit = GetComponent<Unit>();
            m_Animator = GetComponentInChildren<Animator>();
            if (m_Animator != null)
            {
                m_ForwardID = Animator.StringToHash(ANIMATION_FORWARD);
                m_SideID = Animator.StringToHash(ANIMATION_SIDE);
                m_IsAttackingID = Animator.StringToHash(ANIMATION_IS_ATTACKING);
                m_AttackSwordID = Animator.StringToHash(ANIMATION_ATTACK_SWORD);
                m_AttackWandID = Animator.StringToHash(ANIMATION_ATTACK_WAND);
                m_AttackMotionID = Animator.StringToHash(ANIMATION_ATTACK_MOTION);
                m_AttackGunID = Animator.StringToHash(ANIMATION_ATTACK_GUN);
            }

            Camera gameplayCamera = Game.gameplayCamera;
            m_CharacterCamera = gameplayCamera.GetComponent<CharacterCamera>();
            if (m_CharacterCamera != null)
            {
                m_CharacterCamera.positionCam = m_Head;
                m_CharacterCamera.xRotCam = m_VerticalLook;
                m_CharacterCamera.yRotCam = transform;
            }
        }

        // Update is called once per frame
        void Update()
        {
            CheckGrounded();

            Vector3 movementDirection = new Vector3(InputManager.GetAxis(GameConstants.INPUT_MOVE_HORIZONTAL), 0.0f, InputManager.GetAxis(GameConstants.INPUT_MOVE_VERTICAL));
            movementDirection.Normalize();

            bool isJumping = InputManager.GetButton(GameConstants.INPUT_JUMP);
            bool isAttacking = InputManager.GetButtonDown(GameConstants.INPUT_ATTACK);
            bool isOnGround = m_CharacterController.isGrounded || m_GroundDistance < m_OnGroundThreshholdDistance;

            if(isOnGround)
            {
                m_JumpMidAirTimeLeft = m_JumpMidAirMaxTime;
            }
            else
            {
                if(m_JumpTimeLeft <= 0.0f && m_JumpMidAirTimeLeft > 0.0f)
                {
                    m_JumpMidAirTimeLeft -= Time.deltaTime;
                }
                else
                {
                    m_JumpMidAirTimeLeft = 0.0f;
                }
            }

            UpdateExternalForces();

            if(isOnGround)
            {
                UpdateGroundMovement(movementDirection, isJumping);
            }
            else
            {
                UpdateAirMovement(movementDirection, isJumping);
            }

            Vector3 inverseVelocity = transform.InverseTransformDirection(m_Velocity);
            UpdateRotation();
            UpdateCamera(inverseVelocity);
            UpdateAnimations(isOnGround,inverseVelocity);
            
        }

        /// <summary>
        /// Gets called to check for a collision.
        /// </summary>
        /// <param name="aInfo"></param>
        public void OnCollisionStay(Collision aInfo)
        {
            
            if(aInfo.collider.gameObject.layer != Game.LAYER_SURFACE)
            {
                return;
            }
            Debug.Log("Collision");
            CapsuleCollider capsuleCollider = (CapsuleCollider)collisionHandler.collider;

            Vector3 contactPoint = aInfo.contacts[0].point;
            Vector3 capsulePoint = MathUtils.GetClosestPtOnCapsule(contactPoint, transform.position, capsuleCollider.height, capsuleCollider.radius);

            Vector3 bottomPoint = transform.position - Vector3.up * (capsuleCollider.height * 0.5f);
        
            if(capsulePoint.y - bottomPoint.y < m_CharacterController.stepOffset)
            {
                capsulePoint.y = bottomPoint.y;
            }

            Vector3 collisionDisplacement = contactPoint - capsulePoint;

            {
                float displacementAmount = collisionDisplacement.magnitude;
                if(displacementAmount > 0.0f)
                {
                    Vector3 forceDir = collisionDisplacement / displacementAmount;
                    float forceAmount = MathUtils.CalcSpringForce(
                        displacementAmount,
                        0.0f,
                        0.0f,
                        m_CollisionSpringMaxLength,
                        forceDir,
                        m_Velocity,
                        m_CollisionSpringMaxForce,
                        m_CollisionSpringDamping);

                    float accelerationThisFrame = Time.fixedDeltaTime * forceAmount / m_Mass;

                    m_Velocity += accelerationThisFrame * forceDir;

                    if(aInfo.collider.rigidbody != null && m_AcceptedForceTags.Any(Element => Element ==  aInfo.collider.tag ))
                    {
                        Debug.Log("Adding Force To");
                        Vector3 forcePoint = contactPoint;
                        forcePoint.y = collider.transform.position.y;
                        aInfo.collider.rigidbody.AddForceAtPosition(-(m_PushForceAmount + forceAmount) * forceDir,forcePoint);
                    }
                        
                }
            }
        }

        /// <summary>
        /// Checks if the character is grounded.
        /// </summary>
        private void CheckGrounded()
        {
            m_NearGround = false;
            m_GroundAngularVelocity.Set(0.0f, 0.0f, 0.0f);
            m_GroundVelocity.Set(0.0f, 0.0f, 0.0f);
            m_GroundDistance = float.MaxValue;

            Vector3 origin = transform.position;
            Vector3 direction = -Vector3.up;

            float distance = m_CharacterController.height * 0.5f + m_CheckGroundDistance;
            RaycastHit hitInfo;
            if(!Physics.Raycast(origin,direction,out hitInfo,distance,m_GroundCheckMask))
            {
                return;
            }

            m_NearGround = true;
            m_GroundDistance = hitInfo.distance - m_CharacterController.height * 0.5f;

            IMovingPlatform movingPlatform = hitInfo.collider.GetComponent("IMovingPlatform") as IMovingPlatform;
            if(movingPlatform != null)
            {
                m_GroundAngularVelocity = movingPlatform.GetSurfaceAngularVelocity(hitInfo.point);
                m_GroundVelocity = movingPlatform.GetSurfaceVelocity(hitInfo.point);
            }
            else if(hitInfo.collider.rigidbody != null)
            {
                m_GroundAngularVelocity = hitInfo.collider.rigidbody.angularVelocity;
                m_GroundVelocity = hitInfo.collider.rigidbody.velocity;
            }
        }

        /// <summary>
        /// Updates the external forces added onto the player.
        /// </summary>
        private void UpdateExternalForces()
        {
            Vector3 acceleration = m_NetExternalForce / m_Mass;
            m_Velocity += acceleration * Time.deltaTime;
            m_NetExternalForce.Set(0.0f, 0.0f, 0.0f);
        }

        private void UpdateGroundMovement(Vector3 aMovementDirection, bool aIsJumping)
        {
            if(aMovementDirection.sqrMagnitude > MathUtils.CompareEpsilon)
            {
                Vector3 localVelocity = m_Velocity - m_GroundVelocity;
                float originalLocalVelY = localVelocity.y;
                localVelocity.y = 0.0f;

                Vector3 movementAcceleration = transform.TransformDirection(aMovementDirection);
                Vector3 velocityAlongMovementDirection = Vector3.Project(localVelocity, movementAcceleration);

                if(Vector3.Dot(velocityAlongMovementDirection,movementAcceleration) > -m_ChangeDirectionAccelerationThreshhold)
                {
                    localVelocity = MathUtils.ExponentialEase(m_OnGroundStopEaseSpeed, localVelocity, velocityAlongMovementDirection, Time.deltaTime);

                    float speedSqrd = velocityAlongMovementDirection.sqrMagnitude;
                    if(speedSqrd < m_Unit.movementSpeed * m_Unit.movementSpeed)
                    {
                        movementAcceleration *= m_OnGroundMovementAcceleration;
                        localVelocity += movementAcceleration * Time.deltaTime;
                    }

                    localVelocity.y = originalLocalVelY;
                    m_Velocity = localVelocity + m_GroundVelocity;
                }
                else
                {
                    UpdateStopping(m_OnGroundStopEaseSpeed);
                }
            }
            else
            {
                UpdateStopping(m_OnGroundStopEaseSpeed);
            }


            m_Velocity.y = m_GroundVelocity.y;

            if(aIsJumping)
            {
                Jump();
            }
            else
            {
                m_AllowJump = true;
            }
            m_CharacterController.Move(m_Velocity * Time.deltaTime);

        }
        private void UpdateAirMovement(Vector3 aMovementDirection, bool aIsJumping)
        {
            if (aMovementDirection.sqrMagnitude > MathUtils.CompareEpsilon)
            {
                Vector3 localVelocity = m_Velocity - m_GroundVelocity;
                float originalLocalVelY = localVelocity.y;
                localVelocity.y = 0.0f;

                Vector3 movementAcceleration = transform.TransformDirection(aMovementDirection);
                Vector3 velocityAlongMovementDirection = Vector3.Project(localVelocity, movementAcceleration);

                if (Vector3.Dot(velocityAlongMovementDirection, movementAcceleration) > -m_ChangeDirectionAccelerationThreshhold)
                {
                    localVelocity = MathUtils.ExponentialEase(m_OnGroundStopEaseSpeed, localVelocity, velocityAlongMovementDirection, Time.deltaTime);

                    float speedSqrd = velocityAlongMovementDirection.sqrMagnitude;
                    if (speedSqrd < m_InAirControlSpeed * m_InAirControlSpeed)
                    {
                        movementAcceleration *= m_InAirMovementAcceleration;
                        localVelocity += movementAcceleration * Time.deltaTime;
                    }

                    localVelocity.y = originalLocalVelY;
                    m_Velocity = localVelocity + m_GroundVelocity;
                }
                else
                {
                    UpdateStopping(m_InAirStopEaseSpeed);
                }
            }


            if (m_NearGround || m_JumpMidAirTimeLeft > 0.0f)
            {
                if (aIsJumping)
                {
                    Jump();
                }
                else
                {
                    m_AllowJump = true;
                }
            }

            if(m_JumpTimeLeft > 0.0f && aIsJumping)
            {
                m_JumpTimeLeft -= Time.deltaTime;
            }
            else
            {
                m_Velocity.y += m_GravityAcceleration * Time.deltaTime;
                m_JumpTimeLeft = 0.0f;
            }

            m_CharacterController.Move(m_Velocity * Time.deltaTime);
        }

        private void UpdateStopping(float aStopEaseSpeed)
        {
            m_Velocity = MathUtils.ExponentialEase(aStopEaseSpeed, m_Velocity, m_GroundVelocity, Time.deltaTime);
        }

        private void Jump()
        {
            if(m_AllowJump)
            {
                m_Velocity.y = m_JumpSpeed + m_GroundVelocity.y;
                transform.position += new Vector3(0.0f, m_JumpForce, 0.0f);
                m_JumpTimeLeft = m_JumpMaxHoldTime;
                m_AllowJump = false;
            }
        }

        /// <summary>
        /// Updates the rotation based on mouse input
        /// </summary>
        private void UpdateRotation()
        {
            if(Screen.lockCursor == false)
            {
                return;
            }
            {
                float rotationThisFrame = 0.0f;

                if(m_EnabledMouseControl)
                {
                    rotationThisFrame = m_TurnSpeed * InputManager.GetAxis("Mouse Y");
                }

                m_VerticalLookAngle += rotationThisFrame;
                m_VerticalLookAngle = Mathf.Clamp(m_VerticalLookAngle, -m_MaxVerticalCameraAngle, m_MaxVerticalCameraAngle);

                m_VerticalLook.localRotation = Quaternion.Euler(m_VerticalLookAngle, 0.0f, 0.0f);
            }
            {
                float rotationThisFrame = 0.0f;

                if (m_EnabledMouseControl)
                {
                    rotationThisFrame = m_TurnSpeed * InputManager.GetAxis("Mouse X");
                }

                Quaternion rotation = Quaternion.Euler(0.0f, rotationThisFrame, 0.0f);

                rotation *= Quaternion.Euler(m_GroundAngularVelocity * Time.deltaTime);
                transform.rotation *= rotation;
            }
        }

        /// <summary>
        /// Updates the camera's swaying state
        /// </summary>
        /// <param name="aMovementSpeed"></param>
        private void UpdateCamera(Vector3 aMovementSpeed)
        {

            if(Mathf.Abs(aMovementSpeed.z) > 0.1f)
            {
                m_CharacterCamera.isSwaying = true;
                m_CharacterCamera.swaySpeed = Mathf.Abs(aMovementSpeed.z / m_Unit.movementSpeed) * m_CameraSwaySpeed;
            }
            else
            {
                m_CharacterCamera.isSwaying = false;
            }
        }

        /// <summary>
        /// Updates the animations for the character based on the state.
        /// </summary>
        /// <param name="aIsOnGround"></param>
        /// <param name="aMovementSpeed"></param>
        private void UpdateAnimations(bool aIsOnGround, Vector3 aMovementSpeed)
        {
            m_InverseVelocity = aMovementSpeed;
            m_Animator.SetFloat(m_ForwardID, aMovementSpeed.z / m_Unit.movementSpeed);
            m_Animator.SetFloat(m_SideID, aMovementSpeed.x / m_Unit.movementSpeed);
            m_Animator.SetFloat(m_AttackMotionID, m_AttackMotion);
            switch(m_AttackType)
            {
                case AttackType.NONE:
                    m_Animator.SetBool(m_IsAttackingID, false);
                    m_Animator.SetBool(m_AttackSwordID, false);
                    m_Animator.SetBool(m_AttackWandID, false);
                    m_Animator.SetBool(m_AttackGunID, false);
                    break;
                case AttackType.SWORD:
                    m_Animator.SetBool(m_IsAttackingID, true);
                    m_Animator.SetBool(m_AttackSwordID, true);
                    m_Animator.SetBool(m_AttackWandID, false);
                    m_Animator.SetBool(m_AttackGunID, false);
                    break;
                case AttackType.WAND:
                    m_Animator.SetBool(m_IsAttackingID, true);
                    m_Animator.SetBool(m_AttackSwordID, false);
                    m_Animator.SetBool(m_AttackWandID, true);
                    m_Animator.SetBool(m_AttackGunID, false);
                    break;
                case AttackType.GUN:
                    m_Animator.SetBool(m_IsAttackingID, true);
                    m_Animator.SetBool(m_AttackSwordID, false);
                    m_Animator.SetBool(m_AttackWandID, false);
                    m_Animator.SetBool(m_AttackGunID, true);
                    break;
            }
        }
        

        public void AddForce(Vector3 aForce)
        {
            m_NetExternalForce += aForce;
        }

        public CharacterCollisionHandler collisionHandler
        {
            get { return m_CollisionHandler; }
            set { m_CollisionHandler = value; }
        }

        public Vector3 velocity
        {
            get { return m_Velocity; }
        }

        public AttackType attackType
        {
            get { return m_AttackType; }
            set { m_AttackType = value; }
        }
        public float attackMotion
        {
            get { return m_AttackMotion;}
            set { m_AttackMotion = value; }
        }
    }
}