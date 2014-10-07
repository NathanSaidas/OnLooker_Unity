using UnityEngine;
using System;
using System.Collections;

namespace EndevGame
{


    [Serializable]
    public class ShoulderCamera : CameraController, IDebugGizmo, IDebugWatch
    {
        public ShoulderCamera(Transform aParent)
            : base(aParent)
        {

        }
        public ShoulderCamera(Transform aParent, Transform aTarget)
            : base(aParent, aTarget)
        {

        }

        /// <summary>
        /// The distance the camera goes back to search for a obstruction
        /// </summary>
        [SerializeField]
        private float m_CollisionCheckDistance = 0.66f;
        /// <summary>
        /// The distance the camera is away from 
        /// </summary>
        [SerializeField]
        private float m_Distance = 0.0f;
        /// <summary>
        /// Determines if there is an obstruction between the camera and the target
        /// </summary>
        [SerializeField]
        private bool m_InCollision = false;

        [SerializeField]
        private Vector3 m_LookAtPosition = Vector3.zero;

        [SerializeField]
        private float m_Y = 0.0f;


        Vector3 m_DebugPosition = Vector3.zero;
        Vector3 m_DebugDirection = Vector3.zero;

        private void missingProperty(string aName)
        {
            Debug.LogError("Missing \'" + aName + "\' in ShoulderCamera");
            enabled = false;
        }
        public override void start()
        {
            if (GameManager.instance != null)
            {

                DebugUtils.addGizmo(this);
                DebugUtils.addWatch(this);
            }
        }

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



            if (m_InCollision == false)
            {
                parent.position = target.position + target.rotation * offset;
            }
            else
            {
                parent.position = target.position + target.rotation * new Vector3(offset.x, offset.y, -m_Distance);
            }
            parent.LookAt(target.position + target.rotation * (m_LookAtPosition + new Vector3(0.0f, m_Y, 0.0f)));
        }
        public override void physicsUpdate()
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
            //Check a raycast against all objects defined as a surface.
            int layerMask = 1 << GameManager.SURFACE_LAYER;
            //target = sphere, transform
            //parent = camera, transform
            //Vector3 targetPosition = target.position + target.rotation * offset;
            Vector3 direction = parent.position - target.position;// targetPosition - parent.position;
            direction.Normalize();

            float distanceBetween = Vector3.Distance(target.position, parent.position) + m_CollisionCheckDistance;
            RaycastHit hit;
            if (Physics.Raycast(target.position, direction, out hit, distanceBetween, layerMask))
            //if(Physics.Linecast(parent.position, target.position, out hit, layerMask))
            {
                m_Distance = hit.distance - m_CollisionCheckDistance;
                m_InCollision = true;
            }
            else
            {
                m_Distance = Mathf.Clamp(m_Distance + 2.0f * Time.fixedDeltaTime, 0.0f, offset.z);
                m_InCollision = false;
            }

        }

        public override void reset(Transform aTarget)
        {
            target = aTarget;
            if (aTarget == null)
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
            if (parent == null)
            {
                missingProperty("Parent");
                return Vector3.zero;
            }
            parent.rotation = aTargetOrientation;

            bool collisionOccured = false;

            //Check a raycast against all objects defined as a surface.
            int layerMask = 1 << GameManager.SURFACE_LAYER;

            Vector3 targetPosition = aTargetPosition + aTargetOrientation * offset;
            Vector3 direction = targetPosition - parent.position;
            direction.Normalize();

            float distanceBetween = Vector3.Distance(targetPosition, parent.position) + m_CollisionCheckDistance;
            float hitDistance = 0.0f;
            RaycastHit hit;
            if (Physics.Raycast(parent.position, direction, out hit, distanceBetween, layerMask))
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



        public void onReport()
        {
            if (enabled == true)
            {
                GUILayout.Label("Shoulder Camera | Collision : " + (inCollision == true ? "TRUE" : "FALSE"));
                GUILayout.Label("Shoulder Camera | Distance : " + Vector3.Distance(target.position, parent.position) + collisionCheckDistance);
                GUILayout.Label("Shoulder Camera | Target Pos : " + target.position);
                GUILayout.Label("Shoulder Camera | Parent Pos : " + parent.position);
                GUILayout.Label("Shoulder Camera | Collision Check : " + collisionCheckDistance);
            }
        }

        public void onDebugDraw()
        {
            if (parent == null || target == null)
            {
                return;
            }

            //The targets position
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(target.position, Vector3.one * 1.3f);
            Gizmos.color = Color.red;
            Gizmos.matrix = Matrix4x4.TRS(parent.position, parent.rotation, Vector3.one);


            //The target position
            Gizmos.DrawWireCube(m_DebugPosition, Vector3.one * 1.1f);
            //My Position with direction
            //The target position
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(m_DebugPosition + m_DebugDirection * 3.0f, Vector3.one * 1.2f);
        }
    }
}