using UnityEngine;
using System;
using System.Collections;

namespace Gem
{
    /// <summary>
    /// Base behvaiour class for all future AI behaviours
    /// </summary>
    public class AIBehaviour : MonoBehaviour
    {
        protected Transform m_Target = null;

        public virtual bool AquireTarget(AIMotor aController)
        {
            return false;
        }
        public virtual bool MoveToTarget(AIMotor aController)
        {
            return false;
        }

        public Transform target
        {
            get { return m_Target; }
            set { m_Target = value; }
        }
    }
}