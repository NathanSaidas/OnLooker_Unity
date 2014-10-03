using UnityEngine;
using System.Collections;
using EndevGame;

[System.Serializable]
public class CharacterInputState
{
    /// <summary>
    /// This represents the characters forward motion (z axis)
    /// </summary>
    [SerializeField] //Only serialized for debugging purposes.
    private float m_ForwardMotion = 0.0f;


    /// <summary>
    /// This represents the characters side motion. (x axis)
    /// </summary>
    [SerializeField]//Only serialized for debugging purposes.
    private float m_SideMotion = 0.0f;

    /// <summary>
    /// This will be true if jump button is pressed. (First frame only)
    /// </summary>
    [SerializeField]//Only serialized for debugging purposes.
    private bool m_Jump = false;

    /// <summary>
    /// This will be true if crouch button is pressed.(First frame only)
    /// </summary>
    [SerializeField]//Only serialized for debugging purposes.
    private bool m_Crouch = false;

    /// <summary>
    /// This button is also referred to as the 'Use Key' or 'Function Key'.
    /// Any action the character wants to do this button is used.
    /// This will be true if its pressed. (First Frame Only)
    /// </summary>
    [SerializeField]//Only serialized for debugging purposes.
    private bool m_Action = false;

    /// <summary>
    /// This will be true if its pressed. False if not. (All Frames)
    /// </summary>
    [SerializeField]//Only serialized for debugging purposes.
    private bool m_Grow = false;

    /// <summary>
    /// This will be true if its pressed. False if not. (All Frames)
    /// </summary>
    [SerializeField]//Only serialized for debugging purposes.
    private bool m_Shrink = false;
    /// <summary>
    /// This will be true if its pressed. (First frame only)
    /// </summary>
    [SerializeField]//Only serialized for debugging purposes.
    private bool m_Shoot = false;

    /// <summary>
    /// This button will be true if its pressed. (All Frames)
    /// </summary>
    [SerializeField]//Only serialized for debugging purposes.
    private bool m_Sprint = false;

    /// <summary>
    /// This will be positive value for scroll up, negative for scroll down. 0 for no scrolling
    /// </summary>
    [SerializeField]
    private float m_ProjectileType = 0.0f;

    /// <summary>
    /// This will be true if its pressed.(First Frame Only).
    /// </summary>
    [SerializeField]//Only serialized for debugging purposes.
    private bool m_ShootMode = false;


    public float forwardMotion
    {
        get { return m_ForwardMotion; }
        set { m_ForwardMotion = value; }
    }
    public float sideMotion
    {
        get { return m_SideMotion; }
        set { m_SideMotion = value; }
    }
    public bool jump
    {
        get { return m_Jump; }
        set{m_Jump = value;}
    }
    public bool crouch
    {
        get { return m_Crouch; }
        set { m_Crouch = value; }
    }
    public bool action
    {
        get { return m_Action; }
        set { m_Action = value; }
    }
    public bool grow
    {
        get { return m_Grow; }
        set { m_Grow = value; }
    }
    public bool shrink
    {
        get { return m_Shrink; }
        set { m_Shrink = value; }
    }
    public bool shoot
    {
        get { return m_Shoot; }
        set { m_Shoot = value; }
    }
    public bool sprint
    {
        get { return m_Sprint; }
        set { m_Sprint = value; }
    }

    public float projectileType
    {
        get { return m_ProjectileType; }
        set { m_ProjectileType = value; }
    }

    public bool shootMode
    {
        get { return m_ShootMode; }
        set { m_ShootMode = value; }
    }
}


public class CharacterManager : EndevBehaviour
{
    [SerializeField]
    private CharacterMotor m_CharacterMotor = null;
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

    // Use this for initialization
	void Start () 
    {
        m_CharacterCamera = CameraManager.gameplayCamera;
        m_OrbitCamera = CameraManager.orbitCamera;

        CameraManager camMan = CameraManager.instance;
        if(camMan != null)
        {
            camMan.transitionToOrbit(m_CameraTarget, CameraMode.INSTANT, 0.0f);
        }

        m_CharacterMotor = GetComponent<CharacterMotor>();
	}
	
	// Update is called once per frame
	protected override void Update () 
    {
	    if(m_InputState == null)
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
        get { return m_InputState.action ; }
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

    public CharacterMotor characterMotor
    {
        get { return m_CharacterMotor; }
    }
    public CharacterAnimation characterAnimation
    {
        get { return m_CharacterAnimation; }
    }

    public Camera characterCamera
    {
        get { return m_CharacterCamera; }
    }
    public OrbitCamera orbitCamera
    {
        get { return m_OrbitCamera; }
    }
    public Transform cameraTarget
    {
        get { return m_CameraTarget; }
        set { m_CameraTarget = value; }
    }
    public bool ownsCamera
    {
        get { return CameraManager.cameraIsMine(m_CameraTarget); }
    }
}
