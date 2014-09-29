using UnityEngine;
using System;
using System.Collections;


[Serializable]
public abstract class CameraController : NativeObject
{
    /// <summary>
    /// Camera's transform.
    /// </summary>
    [SerializeField]
    private Transform m_Parent = null;

    /// <summary>
    /// The target for this camera
    /// </summary>
    [SerializeField]
    private Transform m_Target = null;


    /// <summary>
    /// The offset of this camera from the target
    /// </summary>
    [SerializeField]
    private Vector3 m_Offset = Vector3.zero;

    /// <summary>
    /// Whether or not the camera is enabled.
    /// </summary>
    [SerializeField]
    private bool m_Enabled = false;


    #region Constructor

    public CameraController(Transform aParent)
    {
        m_Parent = aParent;
    }
    public CameraController(Transform aParent, Transform aTarget)
    {
        m_Parent = aParent;
        m_Target = aTarget;
    }

    #endregion


    #region Properties

    /// <summary>
    /// The cameras transform
    /// </summary>
    public Transform parent
    {
        get { return m_Parent; }
        set { if (m_Parent == null) { m_Parent = value; } }
    }

    /// <summary>
    /// The target for this camera
    /// </summary>
    public Transform target
    {
        get { return m_Target; }
        set { m_Target = value; }
    }
    /// <summary>
    ///  The offset of this camera from the target
    /// </summary>
    public Vector3 offset
    {
        get { return m_Offset; }
        set { m_Offset = value; }
    }
    /// <summary>
    /// Whether or not the camera is enabled.
    /// </summary>
	public bool enabled
    {
        get { return m_Enabled; }
        set { m_Enabled = value; }
    }

    #endregion


    #region Abstract
    /// <summary>
    /// Gets called to update the camera
    /// </summary>
    public abstract void update();
    /// <summary>
    /// Gets called each physics world update for the camera.
    /// </summary>
    public abstract void physicsUpdate();


    /// <summary>
    /// Resets the cameras with the given target.
    /// </summary>
    /// <param name="aTarget"></param>
    public abstract void reset(Transform aTarget);
    /// <summary>
    /// Returns the position the camera tries to go to.
    /// </summary>
    /// <param name="aTargetPosition"></param>
    /// <param name="aTargetOrientation"></param>
    /// <returns></returns>
    public abstract Vector3 getTargetPosition(Vector3 aTargetPosition, Quaternion aTargetOrientation);
    
    /// <summary>
    /// Returns the rotation the camera tries to go to
    /// </summary>
    /// <param name="aTargetPosition"></param>
    /// <param name="aTargetOrientation"></param>
    /// <returns></returns>
    public abstract Quaternion getTargetRotation(Vector3 aTargetPosition, Quaternion aTargetOrientation);

    #endregion
}
