using UnityEngine;
using System.Collections;

public class CharacterMotor : CharacterComponent 
{
    /// <summary>
    /// The speed at which the player moves at.
    /// </summary>
    [SerializeField]
    private float m_MovementSpeed = 2.0f;
    [SerializeField]
    private float m_JumpHeight = 3.0f;
    [SerializeField]
    private float m_Gravity = 9.81f;


    [SerializeField]
    private float m_MaxVelocity = 10.0f;

    [SerializeField]
    private float m_GroundCheckOffset = 0.1f;
    [SerializeField]
    private float m_GroundCheckRadius = 0.18f;
    [SerializeField]
    private bool m_ApplyGravity = true;
    
    [SerializeField]
    private float m_JumpTimeLimit = 0.5f;
    [SerializeField]
    private float m_GravityTimer = 0.0f;

    //Managed State
    [SerializeField]
    private bool m_IsCrouching = false;
    [SerializeField]
    private StatModifier m_SprintModifier = new StatModifier(0.0f, 1.1f);
    [SerializeField]
    private StatModifier m_CrouchModifier = new StatModifier(0.0f, 0.6f);


    public Vector3 m_DebugVelocity = Vector3.zero;
    public Vector3 m_DebugVelocityChanged = Vector3.zero;
    public Vector3 m_DebugForce = Vector3.zero;

    /// <summary>
    /// Determines if the character is grounded or not.
    /// </summary>
    [SerializeField] //Serialized for debugging purposes only
    private bool m_IsGrounded = false;

	void Start ()
    {
        manager = GetComponent<CharacterManager>();
        rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
	}

    protected override void Update()
    {
        //Toggle Crouch Mode
        if (crouch == true)
        {
            toggleStance();
        }
        //Disable crouch if sprinting
        if(m_IsCrouching == true && sprint == true)
        {
            m_IsCrouching = false;
        }
	}

    private void FixedUpdate()
    {
        checkGrounded();
        resetAngularVelocity();

        m_GravityTimer -= Time.fixedDeltaTime;
        if (m_GravityTimer < 0.0f)
        {
            applyGravity = true;
        }
        


        if(applyGravity == true)
        {
            //addForce(0.0f, -m_Gravity * rigidbody.mass, 0.0f, ForceMode.VelocityChange);
            velocity = new Vector3(velocity.x, velocity.y - (m_Gravity * rigidbody.mass * Time.fixedDeltaTime), velocity.z);
        }
        if(jump == true && isGrounded)
        {
            doJump();
            m_GravityTimer = m_JumpTimeLimit;
            applyGravity = false;
        }

        move();
        m_DebugVelocity = velocity;

        
    }

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

    private void moveGround(Vector3 aVelocity)
    {
        //Scale the vector by the movement speed.
        if(sprint == true)
        {
            aVelocity *= m_SprintModifier.multiply(m_MovementSpeed);
        }
        else if(isCrouching == true)
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

        m_DebugVelocityChanged = velocityChange;
        m_DebugVelocity = velocity;

        //Add the force to the rigidbody.
        addForce(velocityChange, ForceMode.VelocityChange);
    }
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

        m_DebugVelocityChanged = velocityChange;
        

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
        if(collider == null)
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


        if(Physics.CheckCapsule(center,size,m_GroundCheckRadius,layerMask))
        {
            if(m_IsGrounded == false)
            {
                onBeginGround();
            }
            m_IsGrounded = true;
        }
        else
        {
            if(m_IsGrounded == true)
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
        if(sprint == true)
        {
            return m_SprintModifier.multiply(m_MaxVelocity);
        }
        else if(isCrouching == true)
        {
            return m_CrouchModifier.multiply(m_MaxVelocity);
        }
        return m_MaxVelocity;
    }

    /// <summary>
    /// Adds force onto the player
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="aForceMode"></param>
    public void addForce(float x, float y, float z, ForceMode aForceMode)
    {
        if(rigidbody != null)
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
        m_DebugVelocity = aForce;
        if(rigidbody != null)
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
        set { if (rigidbody.velocity != null) { rigidbody.velocity = value; } }
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
        if(rigidbody != null)
        {
            rigidbody.angularVelocity = Vector3.zero;
        }
    }

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

}
