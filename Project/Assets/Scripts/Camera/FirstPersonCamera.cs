using UnityEngine;
using System;
using System.Collections;

namespace EndevGame
{

    [Serializable]
    public class FirstPersonCamera : CameraController
    {

        public FirstPersonCamera(Transform aParent)
            : base(aParent)
        {

        }
        public FirstPersonCamera(Transform aParent, Transform aTarget)
            : base(aParent, aTarget)
        {

        }

        private void missingProperty(string aName)
        {
            Debug.LogError("Missing \'" + aName + "\' in FirstPersonCamera");
            enabled = false;
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

            parent.position = target.position + target.rotation * offset;
            parent.rotation = target.rotation;
        }

        public override void physicsUpdate()
        {
            //No Logic to implement
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
                parent.position = target.position + target.rotation * offset;
                parent.rotation = target.rotation;
            }
        }
        public override Vector3 getTargetPosition(Vector3 aTargetPosition, Quaternion aTargetOrientation)
        {
            if (parent == null)
            {
                missingProperty("Parent");
                return Vector3.zero;
            }
            return aTargetPosition + aTargetOrientation * offset;
        }
        public override Quaternion getTargetRotation(Vector3 aTargetPosition, Quaternion aTargetOrientation)
        {
            if (parent == null)
            {
                missingProperty("Parent");
                return Quaternion.identity;
            }
            return aTargetOrientation;
        }
    }
}