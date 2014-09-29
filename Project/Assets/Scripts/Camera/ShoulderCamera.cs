using UnityEngine;
using System;
using System.Collections;


[Serializable]
public class ShoulderCamera : CameraController
{
    public ShoulderCamera(Transform aParent) : base(aParent)
    {

    }
    public ShoulderCamera(Transform aParent, Transform aTarget)
        : base(aParent, aTarget)
    {

    }

    [SerializeField]
    private float m_CollisionCheckDistance = 0.66f;
    [SerializeField]
    private float m_Distance = 0.0f;
    [SerializeField]
    private bool m_InCollision = false;

    private void missingProperty(string aName)
    {
        Debug.LogError("Missing \'" + aName + "\' in ShoulderCamera");
        enabled = false;
    }

    public override void update()
    {
        if(enabled == false)
        {
            return;
        }
        if(parent == null)
        {
            missingProperty("Parent");
            return;
        }
        if(target == null)
        {
            missingProperty("Target");
            return;
        }


        parent.rotation = target.rotation;
        if (m_InCollision == false)
        {
            parent.position = target.position + target.rotation * offset;
        }
        else
        {
            parent.position = target.position + target.rotation * new Vector3(offset.x,offset.y, m_Distance);
        }
    }
    public override void physicsUpdate()
    {
        if(enabled == false)
        {
            return;
        }
        if(parent == null)
        {
            missingProperty("Parent");
            return;
        }
        if(target == null)
        {
            missingProperty("Target");
            return;
        }
        //Check a raycast against all objects defined as a surface.
        int layerMask = 1 << GameManager.SURFACE_LAYER;

        Vector3 targetLookAt = target.position + target.rotation * Vector3.forward;
        Vector3 direction = targetLookAt - parent.position;
        direction.Normalize();

        float distanceBetween = Vector3.Distance(targetLookAt, parent.position) + m_CollisionCheckDistance;

        RaycastHit hit;
        if (Physics.Raycast(targetLookAt, direction, out hit, distanceBetween, layerMask))
        {
            m_Distance = hit.distance - m_CollisionCheckDistance;
            m_InCollision = true;
        }
        else
        {
            m_Distance = offset.z;
            m_InCollision = false;
        }

    }

    public override void reset(Transform aTarget)
    {
        target = aTarget;
        if(aTarget == null)
        {
            enabled = false;
        }
        else
        {
            parent.rotation = target.rotation;
            parent.position = target.position + target.rotation * offset;
        }
    }
    public override Vector3 getTargetPosition(Vector3 aTargetPosition, Quaternion aTargetOrientation)
    {
        if(parent == null)
        {
            missingProperty("Parent");
            return Vector3.zero;
        }
        parent.rotation = aTargetOrientation;

        bool collisionOccured = false;

        //Check a raycast against all objects defined as a surface.
        int layerMask = 1 << GameManager.SURFACE_LAYER;

        Vector3 targetLookAt = aTargetPosition + aTargetOrientation * Vector3.forward;
        Vector3 direction = targetLookAt - parent.position;
        direction.Normalize();

        float distanceBetween = Vector3.Distance(targetLookAt, parent.position) + m_CollisionCheckDistance;
        float hitDistance = 0.0f;
        RaycastHit hit;
        if (Physics.Raycast(targetLookAt, direction, out hit, distanceBetween, layerMask))
        {
            hitDistance = hit.distance - m_CollisionCheckDistance;
            collisionOccured = true;
        }
        else
        {
            hitDistance = offset.z;
            collisionOccured = false;
        }

        if (collisionOccured == false)
        {
            return aTargetPosition + aTargetOrientation * offset;
        }
        return aTargetPosition + aTargetOrientation * new Vector3(offset.x, offset.y, hitDistance);
    }
    public override Quaternion getTargetRotation(Vector3 aTargetPosition, Quaternion aTargetOrientation)
    {
        return aTargetOrientation;
    }


    
    public float collisionCheckDistance
    {
        get { return m_CollisionCheckDistance; }
        set { m_CollisionCheckDistance = value; }
    }
    
    public float distance
    {
        get { return m_Distance; }
    }
    
    public bool inCollision
    {
        get { return m_InCollision; }
    }

    
}
