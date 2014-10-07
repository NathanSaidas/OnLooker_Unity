using UnityEngine;
using System.Collections;

namespace EndevGame
{
    public enum CharacterMotorMode
    {
        CAMERA_ORIENTED, //The character moves using the cameras orientation
        FIXED //The character moves using its own orientation
    }

    #region
    /* October,6,2014 - Nathan Hanlan - Added additional regions and comments.
    *  October,6,2014 - Nathan Hanlan - Added support for Fixed Character movement mode.
    */
    #endregion
    /// <summary>
    /// /
    /// </summary>
    public class CharacterMotor : CharacterComponent
    {
        /// <summary>
        /// The speed at which the Character moves at.
        /// </summary>
        [SerializeField]
        private float m_MovementSpeed = 2.0f;
        /// <summary>
        /// The height at which the Character jumps at
        /// </summary>
        [SerializeField]
        private float m_JumpHeight = 3.0f;
        /// <summary>
        /// The amount of gravity acceleration to apply on the Character. (g = m * a)
        /// </summary>
        [SerializeField]
        private float m_Gravity = 9.81f;
        /// <summary>
        /// The speed at which the Character turns at.
        /// </summary>
        [SerializeField]
        private float m_TurnSpeed = 45.0f;

        /// <summary>
        /// The max speed the player can travel at. (This is modified at run time depending on the state of the Character. Crouching / Sprinting)
        /// </summary>
        [SerializeField]
        private float m_MaxVelocity = 10.0f;


        /// <summary>
        /// How far should the ground detection check offset from the capsule collider used on the Character
        /// </summary>
        [SerializeField]
        private float m_GroundCheckOffset = 0.1f;
        /// <summary>
        /// How wide should the capsule cast be
        /// </summary>
        [SerializeField]
        private float m_GroundCheckRadius = 0.18f;
        
        /// <summary>
        /// Determines whether or not the player should have gravity applied.
        /// </summary>
#if UNITY_EDITOR
        [SerializeField] //SerializedField  for debugging purposes
#endif
        private bool m_ApplyGravity = true;

        /// <summary>
        /// How much time to wait before applying gravity again
        /// </summary>
        [SerializeField]
        private float m_JumpTimeLimit = 0.5f;
        /// <summary>
        /// How much time passed to apply gravity.
        /// </summary>
#if UNITY_EDITOR
        [SerializeField] //SerializedField  for debugging purposes
#endif
        private float m_GravityTimer = 0.0f;

        /// <summary>
        /// How far up should the character be allowed to step.
        /// </summary>
        [SerializeField]
        private float m_StepOffset = 0.5f;
        /// <summary>
        /// How far forward should the characters step offset be placed.
        /// </summary>
        [SerializeField]
        private float m_StepForwardDistance = 1.0f;

        #region DEBUGVARIABLES
#if UNITY_EDITOR
        //Debug Variables
        [SerializeField]
        private Vector3 m_DebugStart = Vector3.zero;
        [SerializeField]
        private Vector3 m_DebugEnd = Vector3.zero;
#endif
        #endregion
        
        /// <summary>
        /// Determines if the player is crouching or not
        /// </summary>
        [SerializeField]
        private bool m_IsCrouching = false;
        /// <summary>
        /// Stat modifiers used for sprinting. Uses multply -> add
        /// </summary>
        [SerializeField]
        private StatModifier m_SprintModifier = new StatModifier(0.0f, 1.1f);
        /// <summary>
        /// State modifiers used for crouching. Uses multiply -> add
        /// </summary>
        [SerializeField]
        private StatModifier m_CrouchModifier = new StatModifier(0.0f, 0.6f);

        [SerializeField]
        private CharacterMotorMode m_Mode = CharacterMotorMode.CAMERA_ORIENTED;

        /// <summary>
        /// Determines if the character is grounded or not.
        /// </summary>
#if UNITY_EDITOR
        [SerializeField] //Serialized for debugging purposes only
#endif
        private bool m_IsGrounded = false;


        /// <summary>
        /// Freezes the rigidbody's rotation and initializes the character motor.
        /// </summary>
        void Start()
        {
            init();
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
        }

        protected override void Update()
        {
            //Toggle Crouch Mode
            if (crouch == true)
            {
                toggleStance();
            }
            //Disable crouch if sprinting
            if (m_IsCrouching == true && sprint == true)
            {
                m_IsCrouching = false;
            }
        }

        private void FixedUpdate()
        {
            //Dont process the game object if the game manager is paused.
            if(GameManager.isPaused)
            {
                return;
            }


            //Ground Detection Check
            checkGrounded();
            m_GravityTimer -= Time.fixedDeltaTime;
            if (m_GravityTimer < 0.0f)
            {
                applyGravity = true;
            }

            //Rotate the character towards the camera
            if(lockRotation == false && m_Mode == CharacterMotorMode.CAMERA_ORIENTED)
            {
                turnWithCamera();
            }
            

            //Apply Gravity
            if (applyGravity == true && lockGravity == false)
            {
                //addForce(0.0f, -m_Gravity * rigidbody.mass, 0.0f, ForceMode.VelocityChange);
                velocity = new Vector3(velocity.x, velocity.y - (m_Gravity * rigidbody.mass * Time.fixedDeltaTime), velocity.z);
            }
            //Constantly reset the rigidbodies angular velocity to stop it from rotating.
            resetAngularVelocity();

            //If the character is moving check to see if the slope is to high.
            if (forwardMotion != 0.0f || sideMotion != 0.0f)
            {
                Vector3 startPoint = transform.position;
                Vector3 endPoint = startPoint + transform.rotation * new Vector3(0.0f, m_StepOffset, 1.0f);
                Vector3 direction = (endPoint - startPoint).normalized;
                float distance = Vector3.Distance(startPoint, endPoint);
                int layerMask = 1 << GameManager.SURFACE_LAYER;
                RaycastHit hit;
                //If the slope was to high slow the velocity down to 0 and dont bother trying to move.
                if (Physics.Raycast(startPoint, direction, out hit, distance, layerMask))
                {
                    Vector3 slopeVelocity = Vector3.Lerp(rigidbody.velocity, Vector3.zero, Time.fixedDeltaTime * 5.0f);
                    slopeVelocity.y = rigidbody.velocity.y;
                    velocity = slopeVelocity;
                    return;
                }


            }

            //Check for character actions such as jump / roll
            if (jump == true && isGrounded)
            {
                doJump();
                m_GravityTimer = m_JumpTimeLimit;
                applyGravity = false;
            }

            


            //Finally move the character.
            if (lockMovement == false)
            {
                if (m_Mode == CharacterMotorMode.CAMERA_ORIENTED)
                {
                    move();
                }
                else
                {
                    moveFixed();
                }
            }

        }

        public void setMode(CharacterMotorMode aMode)
        {
            if(aMode != m_Mode)
            {
                if(aMode == CharacterMotorMode.CAMERA_ORIENTED)
                {
                    m_Mode = aMode;
                    CameraManager.instance.transitionToOrbit(transform, CameraMode.LERP, 2.0f);
                }
                else
                {
                    m_Mode = aMode;
                    CameraManager.instance.transitionToShoulder(transform, CameraMode.LERP, 2.0f);
                }
            }
        }

        private void moveFixed()
        {
            float rotation = (fixedSideMotion + sideMotion) * m_TurnSpeed;

            Vector3 currentRotation = transform.rotation.eulerAngles;
            currentRotation.y += rotation;

            Quaternion targetRotation = Quaternion.Euler(currentRotation);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 5.0f);

            Vector3 targetVelocity = transform.rotation * new Vector3(0.0f, 0.0f, forwardMotion);


            //Scale the vector by the movement speed.
            if (sprint == true)
            {
                targetVelocity *= m_SprintModifier.multiply(m_MovementSpeed);
            }
            else if (isCrouching == true)
            {
                targetVelocity *= m_CrouchModifier.multiply(m_MovementSpeed);
            }
            else
            {
                targetVelocity *= m_MovementSpeed;
            }

            //Calculate the velocity change and add the forces
            Vector3 lVelocity = velocity;
            Vector3 velocityChange = (targetVelocity - lVelocity);
            float maxVel = calcMaxVelocity();
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVel, maxVel);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVel, maxVel);
            velocityChange.y = 0.0f;


            //Add the force to the rigidbody.
            addForce(velocityChange, ForceMode.VelocityChange);

        }

        /// <summary>
        /// Turns the character with the camera.
        /// </summary>
        private void turnWithCamera()
        {
            if (characterCamera == null)
            {
                return;
            }


            float cameraYRotation = characterCamera.transform.rotation.eulerAngles.y;

            Quaternion cameraOrientation = Quaternion.Euler(0.0f, cameraYRotation, 0.0f);
            Vector3 direction = cameraOrientation * new Vector3(sideMotion, 0.0f, forwardMotion);

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), m_TurnSpeed * Time.fixedDeltaTime);
            }


        }

        /// <summary>
        /// Moves the character in a direction relative to camera
        /// </summary>
        private void move()
        {
            if (characterCamera == null)
            {
                return;
            }
            float cameraYRotation = characterCamera.transform.rotation.eulerAngles.y;

            Quaternion cameraOrientation = Quaternion.Euler(0.0f, cameraYRotation, 0.0f);
            //Calculate the target direction relative to the cameras rotation on the y.
            Vector3 targetVelocity = cameraOrientation * new Vector3(sideMotion, 0.0f, forwardMotion);

            


            if (isGrounded == true)
            {
                moveGround(targetVelocity);
            }
            else
            {
                moveAir(targetVelocity);
            }
        }

        /// <summary>
        /// Movement calculations on the ground
        /// </summary>
        /// <param name="aVelocity"></param>
        private void moveGround(Vector3 aVelocity)
        {
            //Scale the vector by the movement speed.
            if (sprint == true)
            {
                aVelocity *= m_SprintModifier.multiply(m_MovementSpeed);
            }
            else if (isCrouching == true)
            {
                aVelocity *= m_CrouchModifier.multiply(m_MovementSpeed);
            }
            else
            {
                aVelocity *= m_MovementSpeed;
            }

            //Calculate the velocity change and add the forces
            Vector3 lVelocity = velocity;
            Vector3 velocityChange = (aVelocity - lVelocity);
            float maxVel = calcMaxVelocity();
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVel, maxVel);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVel, maxVel);
            velocityChange.y = 0.0f;


            //Add the force to the rigidbody.
            addForce(velocityChange, ForceMode.VelocityChange);
        }
        /// <summary>
        /// Movement calculations in the air.
        /// </summary>
        /// <param name="aVelocity"></param>
        private void moveAir(Vector3 aVelocity)
        {
            //Scale the vector by the movement speed.
            if (sprint == true)
            {
                aVelocity *= m_SprintModifier.multiply(m_MovementSpeed);
            }
            else if (isCrouching == true)
            {
                aVelocity *= m_CrouchModifier.multiply(m_MovementSpeed);
            }
            else
            {
                aVelocity *= m_MovementSpeed;
            }

            //Calculate the velocity change and add the forces
            Vector3 lVelocity = velocity;
            Vector3 velocityChange = (aVelocity - lVelocity);
            float maxVel = calcMaxVelocity();
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVel, maxVel);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVel, maxVel);
            velocityChange.y = 0.0f;
            //Add the force to the rigidbody.
            addForce(velocityChange, ForceMode.VelocityChange);
        }

        private void doJump()
        {
            float verticalJumpSpeed = Mathf.Sqrt(2.0f * m_JumpHeight * m_Gravity);
            velocity = new Vector3(velocity.x, verticalJumpSpeed, velocity.y * 1.3f);
            onBeginJump();
        }


        /// <summary>
        /// Checks to see if the character is grounded
        /// </summary>
        private void checkGrounded()
        {
            if (collider == null)
            {
                Debug.LogWarning("Missing a collider. Aborting Ground Detection.");
                return;
            }
            int layerMask = 1 << GameManager.SURFACE_LAYER;
            Vector3 center = collider.bounds.center;
            Vector3 size = Vector3.zero;
            size.x = center.x;
            size.y = center.y - m_GroundCheckOffset;
            size.z = center.z;


            if (Physics.CheckCapsule(center, size, m_GroundCheckRadius, layerMask))
            {
                if (m_IsGrounded == false)
                {
                    onBeginGround();
                }
                m_IsGrounded = true;
            }
            else
            {
                if (m_IsGrounded == true)
                {
                    onBeginAir();
                }
                m_IsGrounded = false;
            }

        }

        /// <summary>
        /// Calculates the max velocity of the character based on . Crouching or Sprinting
        /// </summary>
        /// <returns></returns>
        public float calcMaxVelocity()
        {
            if (sprint == true)
            {
                return m_SprintModifier.multiply(m_MaxVelocity);
            }
            else if (isCrouching == true)
            {
                return m_CrouchModifier.multiply(m_MaxVelocity);
            }
            return m_MaxVelocity;
        }

        #region Rigidbody Accessors

        /// <summary>
        /// Adds force onto the player
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="aForceMode"></param>
        public void addForce(float x, float y, float z, ForceMode aForceMode)
        {
            if (rigidbody != null)
            {
                rigidbody.AddForce(x, y, z, aForceMode);
            }
            else
            {
                Debug.Log("Missing rigidbody");
            }
        }
        public void addForce(Vector3 aForce, ForceMode aForceMode)
        {
            if (rigidbody != null)
            {
                rigidbody.AddForce(aForce, aForceMode);
            }
            else
            {
                Debug.Log("Missing rigidbody");
            }
        }
        /// <summary>
        /// Get or set the players velocity.
        /// </summary>
        public Vector3 velocity
        {
            get { return rigidbody == null ? Vector3.zero : rigidbody.velocity; }
            set { if (rigidbody.velocity != null && rigidbody.isKinematic == false) { rigidbody.velocity = value; } }
        }
        public bool isAscending
        {
            get { return velocity.y > 0.0f; }
        }
        public bool isDescending
        {
            get { return velocity.y < 0.0f; }
        }
        public void resetVelocity()
        {
            velocity = Vector3.zero;
        }
        public void resetAngularVelocity()
        {
            if (rigidbody != null && rigidbody.isKinematic == false)
            {
                rigidbody.angularVelocity = Vector3.zero;
            }
        }
        public void disableRigidbody()
        {
            rigidbody.isKinematic = true;
        }
        public void enableRigidbody()
        {
            rigidbody.isKinematic = false;
        }

        #endregion

        /// <summary>
        /// Toggles the players stance. (Crouching or Standing)
        /// </summary>
        public void toggleStance()
        {
            m_IsCrouching = !m_IsCrouching;
        }
        public bool applyGravity
        {
            get { return m_ApplyGravity; }
            set { m_ApplyGravity = value; }
        }
        public bool isCrouching
        {
            get { return m_IsCrouching; }
            set { m_IsCrouching = value; }
        }
        public bool isGrounded
        {
            get { return m_IsGrounded; }
        }


        #region EventCallbacks

        /// <summary>
        /// Gets called when the player first hits the ground
        /// </summary>
        void onBeginGround()
        {

        }
        /// <summary>
        /// Gets called when the player first leaves the ground and is falling or ascending
        /// </summary>
        void onBeginAir()
        {

        }

        /// <summary>
        /// Gets called when the player jumps
        /// </summary>
        void onBeginJump()
        {

        }

        #endregion

#if UNITY_EDITOR
        ///Debug Draw the Slope Height Vector
        private void OnDrawGizmos()
        {
            Vector3 startPoint = transform.position;
            Vector3 endPoint = startPoint + transform.rotation * new Vector3(0.0f, m_StepOffset, m_StepForwardDistance);
            Vector3 direction = (endPoint - startPoint).normalized;
        
        
            Gizmos.color = Color.red;
            Gizmos.DrawLine(startPoint, endPoint);
            m_DebugStart = startPoint;
            m_DebugEnd = endPoint;
        }
#endif

    }
}