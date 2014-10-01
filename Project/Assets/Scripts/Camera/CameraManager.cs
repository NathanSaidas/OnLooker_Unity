using UnityEngine;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour
{
    #region SINGLETON
    private static CameraManager s_Instance = null;
    public static CameraManager instance
    {
        get { return s_Instance; }
    }
    #endregion
    #region StateDefinitions
    //Private State Definitions
    private enum TransitionState
    {
        NONE,
        TO_ORBIT,
        TO_OVER_SHOULDER,
        TO_FIRST_PERSON,
    }
    private enum State
    {
        NONE,
        FIRST_PERSON,
        SHOULDER,
        ORBIT,
        TRANSITION,
        CUTSCENE_FADE_IN,
        CUTSCENE_FADE_OUT,
        CUTSCENE,
    }
    #endregion

    /// <summary>
    /// Initializes the singleton, or gives warning if there is multiple.
    /// </summary>
    void Start()
    {
        if(s_Instance == null)
        {
            s_Instance = this;
        }
        else
        {
            Debug.LogWarning("Attempting to create more than one \'Camera Manager\'");
            Debug.LogWarning("Floating GameObject " + gameObject.name);
            if (Application.isPlaying)
            {
                Destroy(this);
            }
            else
            {
                DestroyImmediate(this);
            }
            return;
        }

        DontDestroyOnLoad(gameObject);
        init();
    }
    //Destroys self owned singletons.
    void OnDestroy()
    {
        if(s_Instance == this)
        {
            s_Instance = null;
        }
    }


    /// <summary>
    /// Use this camera for gameplay
    /// </summary>
    [SerializeField]
    private Camera m_GameplayCamera = null;
    /// <summary>
    /// Use this camera for fade effects. (2D)
    /// </summary>
    [SerializeField]
    private Camera m_FadeCamera = null;
    /// <summary>
    /// The component responsible for fading the camera in and out
    /// </summary>
    [SerializeField]
    private FadeMesh m_FadeMesh = null;

    /// <summary>
    /// The field of view of the camera
    /// </summary>
    private float m_FieldOfView = 0.0f;
    /// <summary>
    /// The alpha value of the fademesh for fade effects on the camera
    /// </summary>
    [SerializeField]
    private float m_Alpha = 0.0f;

    /// <summary>
    /// The camera controllers that manipulate the camera.
    /// </summary>
    [SerializeField]
    private FirstPersonCamera m_FirstPersonCamera = null;
    [SerializeField]
    private ShoulderCamera m_ShoulderCamera = null;
    [SerializeField]
    private OrbitCamera m_OrbitCamera = null;


    /// <summary>
    /// Defines the state of the transition
    /// </summary>
    [SerializeField]
    private TransitionState m_TransitionState = TransitionState.NONE;
    /// <summary>
    /// Defines the targetted state for camera transitions
    /// </summary>
    [SerializeField]
    private State m_TargetState = State.NONE;
    /// <summary>
    /// Defines the current state of the camera manager
    /// </summary>
    [SerializeField]
    private State m_CurrentState = State.NONE;
    /// <summary>
    /// Defines the previous state of the camera manager(First Person,Shoulder,Orbit,Cutscene)
    /// </summary>
    [SerializeField]
    private State m_PreviousState = State.NONE;

    /// <summary>
    /// The transition Mode Specified
    /// </summary>
    [SerializeField]
    private CameraMode m_TransitionMode = CameraMode.NONE;
    /// <summary>
    /// The fall off mode specified
    /// </summary>
    [SerializeField]
    private CameraMode m_FallOffMode = CameraMode.NONE;

    /// <summary>
    /// How fast the camera moves from point a to point b.
    /// </summary>
    [SerializeField]
    private float m_TransitionSpeed = 0.0f;
    /// <summary>
    /// The fast the transition speed should fall off.
    /// </summary>
    [SerializeField]
    private float m_FallOffSpeed = 0.0f;
    /// <summary>
    /// The min(x) and max(y) values that the transition speed can be for falloff
    /// </summary>
    [SerializeField]
    private Vector2 m_TransitionSpeedRange = Vector2.zero;

    /// <summary>
    /// The target for all cameras to use
    /// </summary>
    [SerializeField]
    private Transform m_CurrentTarget = null;
    /// <summary>
    /// The target the camera manager transitions the camera to
    /// </summary>
    [SerializeField]
    private Transform m_TransitionTarget = null;
    /// <summary>
    /// The target the camera manager transitions the camera to
    /// </summary>
    [SerializeField]
    private Vector3 m_TransitionTargetPosition = Vector3.zero;


    /// <summary>
    /// Error message helpers.
    /// </summary>
    /// <param name="aField"></param>
    private void errorMissing(string aField)
    {
        Debug.LogError("Missing \'"+aField+"\' from Camera Manager.");
    }
    private void warnMissing(string aField)
    {
        Debug.LogWarning("Missing \'" + aField + "\' from Camera Manager.");
    }

    //Setup the cameras and the camera controllers
    private void init()
    {
        if(m_GameplayCamera == null)
        {
            warnMissing("Gameplay Camera");
        }
        else
        {
            DontDestroyOnLoad(m_GameplayCamera.gameObject);
        }
        if(m_FadeCamera == null)
        {
            warnMissing("Fade Camera");
            warnMissing("Fade Mesh");
        }
        else
        {
            m_FadeMesh = m_FadeCamera.GetComponentInChildren<FadeMesh>();
            if(m_FadeMesh != null)
            {
                
                m_FadeMesh.alpha = m_Alpha;
                DontDestroyOnLoad(m_FadeCamera.gameObject);
            }
            MeshRenderer fadeRenderer = m_FadeCamera.GetComponentInChildren<MeshRenderer>();
            if(fadeRenderer!=null)
            {
                fadeRenderer.enabled = true;
            }
            
        }

        m_FirstPersonCamera.parent = m_GameplayCamera.transform;
        m_ShoulderCamera.parent = m_GameplayCamera.transform;
        m_OrbitCamera.parent = m_GameplayCamera.transform;

        m_ShoulderCamera.start();
        initCutscene();
    }

    
    private void Update()
    {
        if(m_FadeMesh != null)
        {
            m_FadeMesh.alpha = m_Alpha;
        }
        switch(m_CurrentState)
        {
            case State.NONE:

                break;
            case State.TRANSITION:
                switch (m_TransitionState)
                {
                    case TransitionState.NONE:

                        break;
                    case TransitionState.TO_FIRST_PERSON:
                        moveTo(m_FirstPersonCamera.getTargetPosition(targetPosition, targetRotation));
                        
                        break;
                    case TransitionState.TO_ORBIT:
                        moveTo(m_OrbitCamera.getTargetPosition(targetPosition, targetRotation));
                        break;
                    case TransitionState.TO_OVER_SHOULDER:
                        moveTo(m_ShoulderCamera.getTargetPosition(targetPosition, targetRotation));
                        break;
                }
                break;
            case State.FIRST_PERSON:
                if(m_FirstPersonCamera != null)
                {
                    m_FirstPersonCamera.update();
                }
                break;
            case State.SHOULDER:
                if(m_ShoulderCamera != null)
                {
                    m_ShoulderCamera.update();
                }
                break;
            case State.ORBIT:
                if(m_OrbitCamera != null)
                {
                    m_OrbitCamera.update();
                }
                break;
            case State.CUTSCENE_FADE_OUT:
            case State.CUTSCENE_FADE_IN:
            case State.CUTSCENE:
                updateCutscene();
                break;
        }
    }


    void FixedUpdate()
    {
        if (m_GameplayCamera == null)
        {
            return;
        }
        switch (m_CurrentState)
        {
            case State.FIRST_PERSON:
                if (m_FirstPersonCamera != null)
                {
                    m_FirstPersonCamera.physicsUpdate();
                }
                break;
            case State.SHOULDER:
                if (m_ShoulderCamera != null)
                {
                    m_ShoulderCamera.physicsUpdate();
                }
                break;
            case State.ORBIT:
                if (m_OrbitCamera != null)
                {
                    m_OrbitCamera.physicsUpdate();
                }
                break;

        }
    }


    public void disable()
    {
        m_CurrentState = State.NONE;
    }

    #region Helpers
    /// <summary>
    /// Sets the previous state if the current state is a valid state. (First,Shoulder,Orbit)
    /// </summary>
    private void pushState()
    {
        if (m_CurrentState == State.FIRST_PERSON || m_CurrentState == State.SHOULDER || m_CurrentState == State.ORBIT)
        {
            m_PreviousState = m_CurrentState;
        }
    }
    /// <summary>
    /// Disables all the cameras. (Except cutscene)
    /// </summary>
    private void disableForTransition()
    {
        if (m_FirstPersonCamera != null)
        {
            m_FirstPersonCamera.enabled = false;
        }
        if (m_ShoulderCamera != null)
        {
            m_ShoulderCamera.enabled = false;
        }
        if (m_OrbitCamera != null)
        {
            m_OrbitCamera.enabled = false;
        }
    }
    /// <summary>
    /// Called to set the state of the camera manager just before a transition
    /// </summary>
    private void preTransitionState()
    {
        //Before we can transition from point a to point be we need to disable the current camera behaviours.
        //To stop them from updating
        disableForTransition();
        //We also need to do a safe check to push the current state so that  we can allow for a revert
        pushState();
        //We also need to set the current state to in transition
        m_CurrentState = State.TRANSITION;

    }
    #endregion

    #region TransitionFunctions

    /// <summary>
    /// Moves the camera from where ever it is currently to the new position given.
    /// </summary>
    /// <param name="aPosition">The target position to move to</param>
    /// <param name="aTransitionMode">How to calculate the camera moving to position</param>
    /// <param name="aTransitionSpeed">The speed at which to make the transition. (Start,End)</param>
    /// <param name="aFallOff">Fall off rate. How fast the camera slows down</param>
    /// <param name="aFalloffMode">How to calculate camera slow down.</param>
    public void transitionToFirstPerson(Vector3 aPosition, CameraMode aTransitionMode, Vector2 aTransitionSpeed, float aFallOff, CameraMode aFalloffMode )
    {
        if (m_CurrentState == State.CUTSCENE || m_CurrentState == State.CUTSCENE_FADE_IN || m_CurrentState == State.CUTSCENE_FADE_OUT)
        {
            Debug.LogWarning("You must first cancel the cutscene before you can make a transition between gameplay cameras.");
            return;
        }
        //Disable camera controllers, push the state and set current state to transition
        preTransitionState();

        //Set the state
        m_TransitionTarget = null;
        m_TransitionTargetPosition = aPosition;
        m_TransitionMode = aTransitionMode;
        m_TransitionSpeed = aTransitionSpeed.y;


        m_FallOffMode = aFalloffMode;
        m_FallOffSpeed = aFallOff;
        m_TransitionSpeedRange = aTransitionSpeed;
        m_TargetState = State.FIRST_PERSON;
        m_TransitionState = TransitionState.TO_FIRST_PERSON;


        //Invoke the callback
        onBeginTransition();
    }
    public void transitionToFirstPerson(Transform aPosition, CameraMode aTransitionMode, Vector2 aTransitionSpeed, float aFallOff, CameraMode aFalloffMode)
    {
        if (m_CurrentState == State.CUTSCENE || m_CurrentState == State.CUTSCENE_FADE_IN || m_CurrentState == State.CUTSCENE_FADE_OUT)
        {
            Debug.LogWarning("You must first cancel the cutscene before you can make a transition between gameplay cameras.");
            return;
        }
        //Disable camera controllers, push the state and set current state to transition
        preTransitionState();

        //Set the state
        m_TransitionTarget = aPosition;
        m_TransitionTargetPosition = aPosition.position;
        m_TransitionMode = aTransitionMode;
        m_TransitionSpeed = aTransitionSpeed.y;


        m_FallOffMode = aFalloffMode;
        m_FallOffSpeed = aFallOff;
        m_TransitionSpeedRange = aTransitionSpeed;
        m_TargetState = State.FIRST_PERSON;
        m_TransitionState = TransitionState.TO_FIRST_PERSON;


        //Invoke the callback
        onBeginTransition();
    }
    public void transitionToFirstPerson(Vector3 aPosition, CameraMode aTransitionMode, float aTransitionSpeed)
    {
        if (m_CurrentState == State.CUTSCENE || m_CurrentState == State.CUTSCENE_FADE_IN || m_CurrentState == State.CUTSCENE_FADE_OUT)
        {
            Debug.LogWarning("You must first cancel the cutscene before you can make a transition between gameplay cameras.");
            return;
        }
        //Disable camera controllers, push the state and set current state to transition
        preTransitionState();

        //Set the state
        m_TransitionTarget = null;
        m_TransitionTargetPosition = aPosition;
        m_TransitionMode = aTransitionMode;
        m_TransitionSpeed = aTransitionSpeed;


        m_FallOffMode = CameraMode.NONE;
        m_FallOffSpeed = 0.0f;
        m_TransitionSpeedRange = Vector2.zero;
        m_TargetState = State.FIRST_PERSON;
        m_TransitionState = TransitionState.TO_FIRST_PERSON;


        //Invoke the callback
        onBeginTransition();
    }
    public void transitionToFirstPerson(Transform aPosition, CameraMode aTransitionMode,float aTransitionSpeed)
    {
        if (m_CurrentState == State.CUTSCENE || m_CurrentState == State.CUTSCENE_FADE_IN || m_CurrentState == State.CUTSCENE_FADE_OUT)
        {
            Debug.LogWarning("You must first cancel the cutscene before you can make a transition between gameplay cameras.");
            return;
        }
        //Disable camera controllers, push the state and set current state to transition
        preTransitionState();

        //Set the state
        m_TransitionTarget = aPosition;
        m_TransitionTargetPosition = aPosition.position;
        m_TransitionMode = aTransitionMode;
        m_TransitionSpeed = aTransitionSpeed;


        m_FallOffMode = CameraMode.NONE;
        m_FallOffSpeed = 0.0f;
        m_TransitionSpeedRange = Vector2.zero;
        m_TargetState = State.FIRST_PERSON;
        m_TransitionState = TransitionState.TO_FIRST_PERSON;


        //Invoke the callback
        onBeginTransition();
    }

    ///Shoulder Camera Transition

    public void transitionToShoulder(Vector3 aPosition, CameraMode aTransitionMode, Vector2 aTransitionSpeed, float aFallOff, CameraMode aFalloffMode)
    {
        if (m_CurrentState == State.CUTSCENE || m_CurrentState == State.CUTSCENE_FADE_IN || m_CurrentState == State.CUTSCENE_FADE_OUT)
        {
            Debug.LogWarning("You must first cancel the cutscene before you can make a transition between gameplay cameras.");
            return;
        }
        //Disable camera controllers, push the state and set current state to transition
        preTransitionState();

        //Set the state
        m_TransitionTarget = null;
        m_TransitionTargetPosition = aPosition;
        m_TransitionMode = aTransitionMode;
        m_TransitionSpeed = aTransitionSpeed.y;


        m_FallOffMode = aFalloffMode;
        m_FallOffSpeed = aFallOff;
        m_TransitionSpeedRange = aTransitionSpeed;
        m_TargetState = State.SHOULDER;
        m_TransitionState = TransitionState.TO_OVER_SHOULDER;


        //Invoke the callback
        onBeginTransition();
    }
    public void transitionToShoulder(Transform aPosition, CameraMode aTransitionMode, Vector2 aTransitionSpeed, float aFallOff, CameraMode aFalloffMode)
    {
        if (m_CurrentState == State.CUTSCENE || m_CurrentState == State.CUTSCENE_FADE_IN || m_CurrentState == State.CUTSCENE_FADE_OUT)
        {
            Debug.LogWarning("You must first cancel the cutscene before you can make a transition between gameplay cameras.");
            return;
        }
        //Disable camera controllers, push the state and set current state to transition
        preTransitionState();

        //Set the state
        m_TransitionTarget = aPosition;
        m_TransitionTargetPosition = aPosition.position;
        m_TransitionMode = aTransitionMode;
        m_TransitionSpeed = aTransitionSpeed.y;


        m_FallOffMode = aFalloffMode;
        m_FallOffSpeed = aFallOff;
        m_TransitionSpeedRange = aTransitionSpeed;
        m_TargetState = State.SHOULDER;
        m_TransitionState = TransitionState.TO_OVER_SHOULDER;


        //Invoke the callback
        onBeginTransition();
    }
    public void transitionToShoulder(Vector3 aPosition, CameraMode aTransitionMode, float aTransitionSpeed)
    {
        if (m_CurrentState == State.CUTSCENE || m_CurrentState == State.CUTSCENE_FADE_IN || m_CurrentState == State.CUTSCENE_FADE_OUT)
        {
            Debug.LogWarning("You must first cancel the cutscene before you can make a transition between gameplay cameras.");
            return;
        }
        //Disable camera controllers, push the state and set current state to transition
        preTransitionState();

        //Set the state
        m_TransitionTarget = null;
        m_TransitionTargetPosition = aPosition;
        m_TransitionMode = aTransitionMode;
        m_TransitionSpeed = aTransitionSpeed;


        m_FallOffMode = CameraMode.NONE;
        m_FallOffSpeed = 0.0f;
        m_TransitionSpeedRange = Vector2.zero;
        m_TargetState = State.SHOULDER;
        m_TransitionState = TransitionState.TO_OVER_SHOULDER;


        //Invoke the callback
        onBeginTransition();
    }
    public void transitionToShoulder(Transform aPosition, CameraMode aTransitionMode, float aTransitionSpeed)
    {
        if (m_CurrentState == State.CUTSCENE || m_CurrentState == State.CUTSCENE_FADE_IN || m_CurrentState == State.CUTSCENE_FADE_OUT)
        {
            Debug.LogWarning("You must first cancel the cutscene before you can make a transition between gameplay cameras.");
            return;
        }
        //Disable camera controllers, push the state and set current state to transition
        preTransitionState();

        //Set the state
        m_TransitionTarget = aPosition;
        m_TransitionTargetPosition = aPosition.position;
        m_TransitionMode = aTransitionMode;
        m_TransitionSpeed = aTransitionSpeed;


        m_FallOffMode = CameraMode.NONE;
        m_FallOffSpeed = 0.0f;
        m_TransitionSpeedRange = Vector2.zero;
        m_TargetState = State.SHOULDER;
        m_TransitionState = TransitionState.TO_OVER_SHOULDER;


        //Invoke the callback
        onBeginTransition();
    }


    /// Orbit Camera Transition
    public void transitionToOrbit(Vector3 aPosition, CameraMode aTransitionMode, Vector2 aTransitionSpeed, float aFallOff, CameraMode aFalloffMode)
    {
        if (m_CurrentState == State.CUTSCENE || m_CurrentState == State.CUTSCENE_FADE_IN || m_CurrentState == State.CUTSCENE_FADE_OUT)
        {
            Debug.LogWarning("You must first cancel the cutscene before you can make a transition between gameplay cameras.");
            return;
        }
        //Disable camera controllers, push the state and set current state to transition
        preTransitionState();

        //Set the state
        m_TransitionTarget = null;
        m_TransitionTargetPosition = aPosition;
        m_TransitionMode = aTransitionMode;
        m_TransitionSpeed = aTransitionSpeed.y;


        m_FallOffMode = aFalloffMode;
        m_FallOffSpeed = aFallOff;
        m_TransitionSpeedRange = aTransitionSpeed;
        m_TargetState = State.ORBIT;
        m_TransitionState = TransitionState.TO_ORBIT;


        //Invoke the callback
        onBeginTransition();
    }
    public void transitionToOrbit(Transform aPosition, CameraMode aTransitionMode, Vector2 aTransitionSpeed, float aFallOff, CameraMode aFalloffMode)
    {
        if (m_CurrentState == State.CUTSCENE || m_CurrentState == State.CUTSCENE_FADE_IN || m_CurrentState == State.CUTSCENE_FADE_OUT)
        {
            Debug.LogWarning("You must first cancel the cutscene before you can make a transition between gameplay cameras.");
            return;
        }
        //Disable camera controllers, push the state and set current state to transition
        preTransitionState();

        //Set the state
        m_TransitionTarget = aPosition;
        m_TransitionTargetPosition = aPosition.position;
        m_TransitionMode = aTransitionMode;
        m_TransitionSpeed = aTransitionSpeed.y;


        m_FallOffMode = aFalloffMode;
        m_FallOffSpeed = aFallOff;
        m_TransitionSpeedRange = aTransitionSpeed;
        m_TargetState = State.ORBIT;
        m_TransitionState = TransitionState.TO_ORBIT;


        //Invoke the callback
        onBeginTransition();
    }
    public void transitionToOrbit(Vector3 aPosition, CameraMode aTransitionMode, float aTransitionSpeed)
    {
        if (m_CurrentState == State.CUTSCENE || m_CurrentState == State.CUTSCENE_FADE_IN || m_CurrentState == State.CUTSCENE_FADE_OUT)
        {
            Debug.LogWarning("You must first cancel the cutscene before you can make a transition between gameplay cameras.");
            return;
        }
        //Disable camera controllers, push the state and set current state to transition
        preTransitionState();

        //Set the state
        m_TransitionTarget = null;
        m_TransitionTargetPosition = aPosition;
        m_TransitionMode = aTransitionMode;
        m_TransitionSpeed = aTransitionSpeed;


        m_FallOffMode = CameraMode.NONE;
        m_FallOffSpeed = 0.0f;
        m_TransitionSpeedRange = Vector2.zero;
        m_TargetState = State.ORBIT;
        m_TransitionState = TransitionState.TO_ORBIT;


        //Invoke the callback
        onBeginTransition();
    }
    public void transitionToOrbit(Transform aPosition, CameraMode aTransitionMode, float aTransitionSpeed)
    {
        if (m_CurrentState == State.CUTSCENE || m_CurrentState == State.CUTSCENE_FADE_IN || m_CurrentState == State.CUTSCENE_FADE_OUT)
        {
            Debug.LogWarning("You must first cancel the cutscene before you can make a transition between gameplay cameras.");
            return;
        }
        //Disable camera controllers, push the state and set current state to transition
        preTransitionState();

        //Set the state
        m_TransitionTarget = aPosition;
        m_TransitionTargetPosition = aPosition.position;
        m_TransitionMode = aTransitionMode;
        m_TransitionSpeed = aTransitionSpeed;


        m_FallOffMode = CameraMode.NONE;
        m_FallOffSpeed = 0.0f;
        m_TransitionSpeedRange = Vector2.zero;
        m_TargetState = State.ORBIT;
        m_TransitionState = TransitionState.TO_ORBIT;


        //Invoke the callback
        onBeginTransition();
    }
    private void moveTo(Vector3 aPosition)
    {

        //Check the distance between the current position / goal
        switch (m_TransitionMode)
        {
            case CameraMode.NONE:

                break;
            case CameraMode.LERP:

                if (Vector3.Distance(m_GameplayCamera.transform.position, aPosition) < 1.0f)
                {
                    //m_Camera.transform.position = aPosition;
                    onGoalReached();
                    break;
                }
                m_GameplayCamera.transform.position = Vector3.Lerp(m_GameplayCamera.transform.position, aPosition, Time.deltaTime * m_TransitionSpeed);

                break;
            case CameraMode.EASE:
                if (Vector3.Distance(m_GameplayCamera.transform.position, aPosition) < 0.1f)
                {
                    //m_Camera.transform.position = aPosition;
                    onGoalReached();
                    break;
                }
                m_GameplayCamera.transform.position = Utilities.exponentialEase(m_GameplayCamera.transform.position, aPosition, Time.deltaTime * m_TransitionSpeed);
                break;
            case CameraMode.SMOOTH_DAMP:
                {
                    if (Vector3.Distance(m_GameplayCamera.transform.position, aPosition) < 1.0f)
                    {
                        //m_Camera.transform.position = aPosition;
                        onGoalReached();
                        break;
                    }
                    Vector3 refVec = new Vector3(m_TransitionSpeed, m_TransitionSpeed, m_TransitionSpeed);
                    m_GameplayCamera.transform.position = Vector3.SmoothDamp(m_GameplayCamera.transform.position, aPosition, ref refVec, Time.deltaTime);
                }
                break;
            case CameraMode.INSTANT:
                m_GameplayCamera.transform.position = aPosition;
                onGoalReached();
                break;

        }
    }
    private void setFallOff()
    {
        if(m_FallOffMode == CameraMode.NONE || m_FallOffSpeed == 0.0f)
        {
            return;
        }
        switch(m_FallOffMode)
        {
            case CameraMode.EASE:
                m_TransitionSpeed = Utilities.exponentialEase(m_TransitionSpeed, m_TransitionSpeedRange.x, Time.deltaTime * 2.0f);
                break;
            case CameraMode.LERP:
            case CameraMode.SMOOTH_DAMP:
                m_TransitionSpeed = Mathf.Lerp(m_TransitionSpeed, m_TransitionSpeedRange.x, Time.deltaTime * 2.0f);
                break;
        }
    }

    //Gets called when the transition is finished
    private void onGoalReached()
    {
        //Clamp the position, set the new state
        m_CurrentState = m_TargetState;
        m_GameplayCamera.transform.position = targetPosition;
        switch (m_CurrentState)
        {
            case State.FIRST_PERSON:
                m_FirstPersonCamera.enabled = true;
                m_FirstPersonCamera.reset(transitionTarget);
                m_CurrentTarget = transitionTarget;
                break;
            case State.SHOULDER:
                m_ShoulderCamera.enabled = true;
                m_ShoulderCamera.reset(transitionTarget);
                m_CurrentTarget = transitionTarget;
                break;
            case State.ORBIT:
                m_OrbitCamera.enabled = true;
                m_OrbitCamera.reset(transitionTarget);
                m_CurrentTarget = transitionTarget;
                break;
        }
        //Make event call
        onEndTransition();
        //Clean State
        m_TransitionTarget = null;
        m_TransitionTargetPosition = Vector3.zero;
        m_TransitionMode = CameraMode.NONE;
        m_TransitionSpeed = 0.0f;

        m_FallOffMode = CameraMode.NONE;
        m_FallOffSpeed = 0.0f;
        m_TransitionSpeedRange = Vector2.zero;
        m_TargetState = State.NONE;
        m_TransitionState = TransitionState.NONE;

    }
    #endregion



    #region Events
    private void onBeginTransition()
    {

    }
    private void onEndTransition()
    {

    }
    private void onBeginCutscene()
    {

    }
    private void onEndCutscene()
    {

    }


    #endregion

    #region Properties
    public Camera gameplayCamera
    {
        get { return m_GameplayCamera; }
        set { m_GameplayCamera = value; }
    }

    public FirstPersonCamera firstPersonCamera
    {
        get { return m_FirstPersonCamera; }
    }
    public ShoulderCamera shoulderCamera
    {
        get { return m_ShoulderCamera; }
    }
    public OrbitCamera orbitCamera
    {
        get { return m_OrbitCamera; }
    }

    //Read Only properties to check the state values of the Camera Manager
    //Helper method to get the target position
    private Vector3 targetPosition
    {
        get { return m_TransitionTarget == null ? m_TransitionTargetPosition : m_TransitionTarget.position; }
    }
    //Helper method to get the target rotation
    private Quaternion targetRotation
    {
        get { return m_TransitionTarget == null ? m_GameplayCamera.transform.rotation : m_TransitionTarget.rotation; }
    }
    //Helper method to get the target
    private Transform transitionTarget
    {
        get { return m_TransitionTarget; }
    }
    public Vector2 transitionSpeedRange
    {
        get { return m_TransitionSpeedRange; }
    }
    public float fallOffSpeed
    {
        get { return m_FallOffSpeed; }
    }
    public float transitionSpeed
    {
        get { return m_TransitionSpeed; }
    }
    public CameraMode fallOffMode
    {
        get { return m_FallOffMode; }
    }
    public CameraMode transitionMode
    {
        get { return m_TransitionMode; }
    }
    public string previousState
    {
        get { return m_PreviousState.ToString(); }
    }
    public string currentState
    {
        get { return m_CurrentState.ToString(); }
    }
    public string transitionState
    {
        get { return m_TransitionState.ToString(); }
    }
    #endregion


    #region Cutscene
    /// <summary>
    /// How fast to fade the cutscene
    /// </summary>
    [SerializeField]
    private float m_CutsceneFadeSpeed = 0.5f;
    //Whether or not to fade the cutscene in
    [SerializeField]
    private bool m_CutsceneFadeIn = false;
    /// <summary>
    /// Whether or not to fade the cutscene out
    /// </summary>
    [SerializeField]
    private bool m_CutsceneFadeOut = false;
    /// <summary>
    /// Whether or not to resume the transition we were left in
    /// </summary>
    [SerializeField]
    private bool m_ResumeInTransition = false;
    /// <summary>
    /// The list of registered cutscenes in this current scene
    /// </summary>
    [SerializeField]
    private List<Cutscene> m_Cutscenes = new List<Cutscene>();

    /// <summary>
    /// The current cutscene playing
    /// </summary>
    [SerializeField]
    private Cutscene m_CurrentCutscene = null;
    /// <summary>
    /// The camera used in the cutscene
    /// </summary>
    [SerializeField]
    private Camera m_CutsceneCamera = null;
    /// <summary>
    /// temp...
    /// </summary>
    [SerializeField]
    private float m_CurrentFrame = 10.0f;

    


    public List<Cutscene> cutscenes
    {
        get { return m_Cutscenes; }
    }
    public Camera cutsceneCamera
    {
        get { return m_CutsceneCamera; }
    }
    public void registerCutscene(Cutscene aCutscene)
    {
        if(aCutscene == null || m_Cutscenes.Contains(aCutscene))
        {
            return;
        }
        m_Cutscenes.Add(aCutscene);
    }
    public void unregisterCutscene(Cutscene aCutscene)
    {
        if(aCutscene == null)
        {
            return;
        }
        m_Cutscenes.Remove(aCutscene);
    }
    public void triggerCutscene(string aCutsceneName, bool aFadeIn, bool aFadeOut)
    {
        //Check the state to restore it after the cutscene
        if (m_CurrentState == State.TRANSITION)
        {
            m_ResumeInTransition = true;
        }
        //A cutscene cannot occur while in a cutscene
        else if (m_CurrentState == State.CUTSCENE || m_CurrentState == State.CUTSCENE_FADE_IN || m_CurrentState == State.CUTSCENE_FADE_OUT)
        {
            Debug.LogWarning("You cannot trigger a cutscene while in a cutscene.");
            return;
        }
        else
        {
            pushState(); //save the state to return to it
        }

        //Get the Cutscene from the list
        for (int i = 0; i < m_Cutscenes.Count; i++)
        {
            if (m_Cutscenes[i].cutsceneName == aCutsceneName)
            {
                m_CurrentCutscene = m_Cutscenes[i];
                break;
            }
        }
        //Cutscene not found
        if (m_CurrentCutscene == null && aCutsceneName != "BACKDOOR")
        {
            Debug.LogWarning("Cutscene \'" + aCutsceneName + "\' could not be found.");
            return;
        }

        m_CutsceneFadeIn = aFadeIn;
        m_CutsceneFadeOut = aFadeOut;
        disableForTransition();
        
        if (m_CutsceneFadeIn == true)
        {
            m_CurrentState = State.CUTSCENE_FADE_IN;
        }
        else
        {
            m_CurrentState = State.CUTSCENE;
            m_GameplayCamera.enabled = false;
            m_CutsceneCamera.enabled = true;
        }

        m_CurrentFrame = 15.0f;
    }
    public void skipCutscene()
    {
        //Resume state
        if (m_CurrentState == State.CUTSCENE_FADE_IN || m_CurrentState == State.CUTSCENE)
        {
            if (m_CutsceneFadeOut == true)
                m_CurrentState = State.CUTSCENE_FADE_OUT;
            else if (m_ResumeInTransition)
            {
                m_CurrentState = State.TRANSITION;
                m_GameplayCamera.enabled = true;
                m_CutsceneCamera.enabled = false;
            }
            else
            {
                m_CurrentState = m_PreviousState;
                m_GameplayCamera.enabled = true;
                m_CutsceneCamera.enabled = false;
            }
        }
    }
    /// <summary>
    /// Gets called when the camera manager inits to give the cutscene a chance to initialize
    /// </summary>
    public void initCutscene()
    {
        if(m_CutsceneCamera == null)
        {
            warnMissing("Cutscene Camera");
        }
        else
        {
            m_CutsceneCamera.enabled = false;
        }
    }
    /// <summary>
    /// Gives the cutscene a chance to update when the camera managers state is set to Cutscene.
    /// </summary>
    public void updateCutscene()
    {
        switch(m_CurrentState)
        {
            case State.CUTSCENE_FADE_IN:
                {
                    if (m_Alpha < 0.98f)
                    {
                        m_GameplayCamera.enabled = true;
                        m_CutsceneCamera.enabled = false;
                    }
                    else if (m_Alpha >= 0.98f)
                    {
                        m_GameplayCamera.enabled = false;
                        m_CutsceneCamera.enabled = true;
                    }
                    m_Alpha = Mathf.Clamp(m_Alpha + Time.deltaTime * m_CutsceneFadeSpeed, 0.0f, 1.0f);
                    if (m_Alpha >= 1.0f)
                    {
                        m_Alpha = 0.0f;
                        m_CurrentState = State.CUTSCENE;
                        m_CurrentCutscene.play(0);
                    }
                    
                }
                break;
            case State.CUTSCENE_FADE_OUT:
                {
                    if (m_Alpha < 0.98f)
                    {
                        m_GameplayCamera.enabled = false;
                        m_CutsceneCamera.enabled = true;
                    }
                    else if (m_Alpha >= 0.98f)
                    {
                        m_GameplayCamera.enabled = true;
                        m_CutsceneCamera.enabled = false;
                    }
                    m_Alpha = Mathf.Clamp(m_Alpha + Time.deltaTime * m_CutsceneFadeSpeed, 0.0f, 1.0f);
                    if (m_Alpha >= 1.0f)
                    {
                        m_Alpha = 0.0f;
                        if (m_ResumeInTransition)
                        {
                            m_CurrentState = State.TRANSITION;
                        }
                        else
                        {
                            m_CurrentState = m_PreviousState;
                        }

                    }
                    
                }
                break;
            case State.CUTSCENE:
                //Else update the cutscene stuff
                m_CurrentCutscene.update();
                if (m_CurrentCutscene.isStopped)
                {
                    if (m_CutsceneFadeOut == true)
                    {
                        m_CurrentState = State.CUTSCENE_FADE_OUT;
                    }
                    else
                    {
                        if (m_ResumeInTransition)
                        {
                            m_GameplayCamera.enabled = true;
                            m_CutsceneCamera.enabled = false;
                            m_CurrentState = State.TRANSITION;
                        }
                        else
                        {
                            m_GameplayCamera.enabled = true;
                            m_CutsceneCamera.enabled = false;
                            m_CurrentState = m_PreviousState;
                        }
                    }
                }
                break;
        }
    }



    #endregion

}
