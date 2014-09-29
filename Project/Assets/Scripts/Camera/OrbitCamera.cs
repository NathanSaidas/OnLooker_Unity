using UnityEngine;
using System;
using System.Collections;


[Serializable]
public class OrbitCamera : CameraController
{
    #region Constructor
    public OrbitCamera(Transform aParent)
        : base(aParent)
    {
        if (parent != null)
        {
            Vector3 angles = parent.eulerAngles;
            m_CurrentRotation.x = angles.y;
            m_CurrentRotation.y = angles.x;

            if (parent.rigidbody != null)
            {
                parent.rigidbody.freezeRotation = true;
            }
        }
    }
    public OrbitCamera(Transform aParent, Transform aTarget)
        : base(aParent, aTarget)
    {
        if (parent != null)
        {
            Vector3 angles = parent.eulerAngles;
            m_CurrentRotation.x = angles.y;
            m_CurrentRotation.y = angles.x;

            if (parent.rigidbody != null)
            {
                parent.rigidbody.freezeRotation = true;
            }
        }
    }
    #endregion

    #region Fields
    /// <summary>
    /// How fast the camera rotates on the x and y axis
    /// </summary>
    [SerializeField]
    private Vector2 m_RotateSpeed = new Vector2(120.0f, 120.0f);
    /// <summary>
    /// The min(x) and max(y) values of the rotation euler angles.
    /// </summary>
    [SerializeField]
    private Vector2 m_YRotationLimit = new Vector2(0.0f, 80.0f);
    /// <summary>
    /// The current x and y rotation of the camera
    /// </summary>
    [SerializeField]
    private Vector2 m_CurrentRotation = Vector2.zero;
    /// <summary>
    /// The min(x) and max(y) values of the zoom distance(m_Target Distance)
    /// </summary>
    [SerializeField]
    private Vector2 m_ZoomRange = new Vector2(0.5f, 15.0f);
    /// <summary>
    /// The speed at which the camera zooms in at.
    /// </summary>
    [SerializeField]
    private float m_ZoomSpeed = 5.0f;

    /// <summary>
    /// The distance the camera is away from the target
    /// </summary>
    [SerializeField]
    private float m_Distance = 0.0f;
    /// <summary>
    /// The distance the camera is trying to achieve
    /// </summary>
    [SerializeField]
    private float m_TargetDistance = 2.0f;
    /// <summary>
    /// The distance the camera is trying to achieve
    /// </summary>
    [SerializeField]
    private bool m_InCollision = false;

    /// <summary>
    /// How much distance should the camera go back to check for a collision
    /// </summary>
    [SerializeField]
    private float m_CollisionCheckDistance = 0.5f;

    /// <summary>
    /// To Invert the camera Input or not.
    /// </summary>
    [SerializeField]
    private bool m_Invert = false;

    #endregion

    /// <summary>
    /// Helper function to log an error for a missing property / reference
    /// </summary>
    /// <param name="aName"></param>
    private void missingProperty(string aName)
    {
        Debug.LogError("Missing \'" + aName + "\' in OrbitCamera");
        enabled = false;
    }

    /// <summary>
    /// Gets called every late update or update to move the camera's positive / rotation
    /// </summary>
    public override void update()
    {

        if (enabled == false)
        {
            return;
        }
        if (parent == null)
        {
            missingProperty("Parent");
            return;
        }
        if (target == null)
        {
            missingProperty("Target");
            return;
        }
        //TODO: Implement Mouse Sensitivity Here
        m_CurrentRotation.x += InputManager.getAxis("Mouse X") * m_RotateSpeed.x * m_Distance * 0.02f;
        m_CurrentRotation.y -= InputManager.getAxis("Mouse Y") * m_RotateSpeed.y * 0.02f;
        m_CurrentRotation.y = Utilities.clampAngle(m_CurrentRotation.y, m_YRotationLimit.x, m_YRotationLimit.y);

        Quaternion rotation = Quaternion.Euler(m_CurrentRotation.y, m_CurrentRotation.x, 0.0f);

        float zoomAxis = InputManager.getAxis("Mouse ScrollWheel");

        if (m_Invert)
        {
            zoomAxis = -zoomAxis;
        }


        m_TargetDistance = Mathf.Clamp(m_TargetDistance - zoomAxis * m_ZoomSpeed, m_ZoomRange.x, m_ZoomRange.y);
        if (m_InCollision == false)
        {
            m_Distance = Mathf.Lerp(m_Distance, m_TargetDistance, 4.0f * Time.deltaTime);
        }
        Vector3 targetPosition = target.position + target.rotation * offset;
        Vector3 negativeDistance = new Vector3(0.0f, 0.0f, -m_Distance);
        Vector3 position = rotation * negativeDistance + targetPosition;

        parent.rotation = rotation;
        parent.position = position;


    }

    /// <summary>
    /// Gets called every physics step to calculate collision detection between target and camera.
    /// </summary>
    public override void physicsUpdate()
    {

        if (enabled == false)
        {
            return;
        }
        if(target == null)
        {
            missingProperty("Target");
            return;
        }
        if(parent == null)
        {
            missingProperty("Parent");
            return;
        }
        int layerMask = 1 << GameManager.SURFACE_LAYER;
        Vector3 targetPosition = target.position + target.rotation * offset;
        Vector3 possiblePosition = parent.rotation * new Vector3(0.0f, 0.0f, -(m_Distance + m_CollisionCheckDistance)) + targetPosition;


        //TO FROM
        Vector3 direction = possiblePosition - targetPosition;
        direction.Normalize();


        float distance = Vector3.Distance(targetPosition, possiblePosition) + m_CollisionCheckDistance;

        RaycastHit hit;
        if (Physics.Raycast(targetPosition, direction, out hit, distance, layerMask))
        {
            m_Distance = hit.distance - m_CollisionCheckDistance;
            m_InCollision = true;
        }
        else
        {
            m_InCollision = false;
        }
    }


    /// <summary>
    /// Gets called to reset the camera's target. 
    /// </summary>
    /// <param name="aTarget">A null target disables the camera</param>
    public override void reset(Transform aTarget)
    {
        target = aTarget;
        if (aTarget == null)
        {
            enabled = false;
        }
        else
        {
            m_CurrentRotation.x = target.eulerAngles.y;
            m_CurrentRotation.y = parent.eulerAngles.x;
            m_TargetDistance = 2.0f;
        }
    }


    /// <summary>
    /// Returns a calculated position based on the target variables provided
    /// </summary>
    /// <param name="aTargetPosition"></param>
    /// <param name="aTargetOrientation"></param>
    /// <returns></returns>
    public override Vector3 getTargetPosition(Vector3 aTargetPosition, Quaternion aTargetOrientation)
    {
        if(parent == null)
        {
            missingProperty("Parent");
            return Vector3.zero;
        }

        Vector3 targetPosition = aTargetPosition + aTargetOrientation * offset;
        Vector3 negativeDistance = new Vector3(0.0f, 0.0f, -m_Distance);
        return getTargetRotation(aTargetPosition, aTargetOrientation) * negativeDistance + targetPosition;
    }

    /// <summary>
    /// Returns a calculated rotation based on the target variables provided
    /// </summary>
    /// <param name="aTargetPosition"></param>
    /// <param name="aTargetOrientation"></param>
    /// <returns></returns>
    public override Quaternion getTargetRotation(Vector3 aTargetPosition, Quaternion aTargetOrientation)
    {
        if(parent == null)
        {
            missingProperty("Parent");
            return Quaternion.identity;
        }
        //TODO: Implement Mouse Sensitivity Here
        Vector3 targetRotation = m_CurrentRotation;
        targetRotation.x += InputManager.getAxis("Mouse Y") * m_RotateSpeed.x * m_Distance * 0.02f;
        targetRotation.y -= InputManager.getAxis("Mouse X") * m_RotateSpeed.y * 0.02f;
        targetRotation.y = Utilities.clampAngle(targetRotation.y, m_YRotationLimit.x, m_YRotationLimit.y);

        return Quaternion.Euler(targetRotation.x, targetRotation.y, 0.0f);
    }

    /// <summary>
    /// Returns a calculated position with an offset of 0
    /// </summary>
    /// <param name="aTargetPosition"></param>
    /// <param name="aTargetOrientation"></param>
    /// <param name="aOffset"></param>
    /// <returns></returns>
    public Vector3 getTargetPosition(Vector3 aTargetPosition, Quaternion aTargetOrientation, Vector3 aOffset)
    {
        if (parent == null)
        {
            missingProperty("Parent");
            return Vector3.zero;
        }

        Vector3 targetPosition = aTargetPosition + aTargetOrientation * aOffset;
        Vector3 negativeDistance = new Vector3(0.0f, 0.0f, -m_Distance);
        return getTargetRotation(aTargetPosition, aTargetOrientation) * negativeDistance + targetPosition;
    }


    /// <summary>
    /// The speed at which the camera rotates at
    /// </summary>
    public Vector2 rotateSpeed
    {
        get { return m_RotateSpeed; }
        set { m_RotateSpeed = value; }
    }
    /// <summary>
    /// The y rotation limit of the camera (Euler Angles)
    /// </summary>
    public Vector2 yRotationLimit
    {
        get { return m_YRotationLimit; }
        set { m_YRotationLimit = value; }
    }
    /// <summary>
    /// The current rotation of the camera on the x and y axis. (Euler Angles)
    /// </summary>
    public Vector2 currentRotation
    {
        get { return m_CurrentRotation; }
    }
    /// <summary>
    /// How close / far can the camera zoom
    /// </summary>
    public Vector2 zoomRange
    {
        get { return m_ZoomRange; }
        set { m_ZoomRange = value; }
    }
    /// <summary>
    /// The speed at which the camera zooms in / out at 
    /// </summary>
    public float zoomSpeed
    {
        get { return m_ZoomSpeed; }
        set { m_ZoomSpeed = value; }
    }
    /// <summary>
    /// The distance the camera has to be away from the target
    /// </summary>
    public float distance
    {
        get { return m_Distance; }
    }
    /// <summary>
    /// The distance the camera is moving towards
    /// </summary>
    public float targetDistance
    {
        get { return m_TargetDistance; }
    }
    /// <summary>
    /// Returns true if the camera is in a collision between its target distance and the player.
    /// </summary>
    public bool inCollision
    {
        get { return m_InCollision; }
    }
    /// <summary>
    /// The distance the camera should check back for collisions.
    /// </summary>
    public float collisionCheckDistance
    {
        get { return m_CollisionCheckDistance; }
        set { m_CollisionCheckDistance = value; }
    }
}
