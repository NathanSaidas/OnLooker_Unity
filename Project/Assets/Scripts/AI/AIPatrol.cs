using UnityEngine;
using System;
using System.Collections.Generic;

namespace Gem
{
    public enum AIPatrolPattern
    {
        WRAP,
        BACK_AND_FORTH,
        
    }
    public enum AIPatrolDirection
    {
        FORWARD,
        BACKWARD
    }
    /// <summary>
    /// An AI Patrol behaviour which patrols a list of points.
    /// </summary>
    [Serializable]
    public class AIPatrol : AIBehaviour
    {
        private enum AIState
        {
            WAITING, //Waiting between goals
            WAITING_GOAL_DELAY, //Waiting at the goal to move to the next goal
            MOVING_TO_GOAL //Moving to the goal

        }

#if UNITY_EDITOR
        [SerializeField]
        private bool m_DrawGizmos = false;
#endif
        /// <summary>
        /// How much distance there must be between the goal and the player to have "reached" the goal
        /// </summary>
        [SerializeField]
        private float m_DistanceThreshHold = 0.1f;
        /// <summary>
        /// The patrol points on the map.
        /// </summary>
        [SerializeField]
        private List<Transform> m_PatrolPoints = new List<Transform>();
        /// <summary>
        /// The current step in the patrol point
        /// </summary>
        [SerializeField]
        private int m_CurrentStep = 0;
        ///// <summary>
        ///// The current goal to go to
        ///// </summary>
        //[SerializeField]
        //private Transform m_CurrentGoal = null;

        [SerializeField]
        private AIPatrolPattern m_Pattern = AIPatrolPattern.BACK_AND_FORTH;
        /// <summary>
        /// The current direction moving through the patrol goal list.
        /// </summary>
        [SerializeField]
        private AIPatrolDirection m_Direction = AIPatrolDirection.FORWARD;
        /// <summary>
        /// The current state of the behaviour
        /// </summary>
        [SerializeField]
        private AIState m_State = AIState.MOVING_TO_GOAL;

        /// <summary>
        /// How much time to wait for
        /// </summary>
        [SerializeField]
        private float m_WaitTime = 1.0f;
        /// <summary>
        /// How much time to wait at the goal
        /// </summary>
        [SerializeField]
        private float m_GoalWaitTime = 1.0f;
        [SerializeField]
        private float m_RunSpeed = 2.0f;

        /// <summary>
        /// Timer variables
        /// </summary>
        private float m_CurrentWaitTime = 0.0f;
        private float m_CurrentGoalWaitTime = 0.0f;
        private Unit m_Unit = null;

        public void Start()
        {
            m_Unit = GetComponent<Unit>();
        }

        public override bool AquireTarget(AIMotor aController)
        {
            if(m_Target == null && m_PatrolPoints.Count > 0)
            {
                //Update next node
                switch(m_Direction)
                {
                    case AIPatrolDirection.FORWARD:
                        m_CurrentStep++;
                        if(m_CurrentStep >= m_PatrolPoints.Count)
                        {
                            if(m_Pattern == AIPatrolPattern.WRAP)
                            {
                                m_CurrentStep = 0;
                            }
                            else
                            {
                                m_Direction = AIPatrolDirection.BACKWARD;
                                m_CurrentStep = m_PatrolPoints.Count - 1;
                            }
                            

                        }
                        break;
                    case AIPatrolDirection.BACKWARD:
                        m_CurrentStep--;
                        if (m_CurrentStep < 0)
                        {
                            m_Direction = AIPatrolDirection.FORWARD;
                            m_CurrentStep = 0;
                        }
                        break;
                }
                ///Set next node
                if(m_CurrentStep >= 0 && m_CurrentStep < m_PatrolPoints.Count)
                {
                    m_Target = m_PatrolPoints[m_CurrentStep];
                }
            }
            return true;
        }
        public override bool MoveToTarget(AIMotor aController)
        {
            switch(m_State)
            {
                case AIState.MOVING_TO_GOAL:
                    m_Unit.movementSpeed = m_RunSpeed;
                    aController.isRunning = false;
                    if (m_Target != null)
                    {
                        float heightOffset = aController.agent.height * 0.5f;
                        Vector3 origin = aController.transform.position;
                        origin.y -= heightOffset;
                        float distanceFromGoal = Vector3.Distance(origin, m_Target.position);
                        if (distanceFromGoal < m_DistanceThreshHold)
                        {
                            m_Target = null;
                            //Skip Goal wait time if there is no delay
                            if(m_GoalWaitTime > 0.0f)
                            {
                                m_CurrentGoalWaitTime = m_GoalWaitTime;
                                m_State = AIState.WAITING_GOAL_DELAY;
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                        {
                            aController.agent.SetDestination(m_Target.position);
                        }
                    }
                    break;
                case AIState.WAITING:
                    if(m_CurrentWaitTime < 0.0f)
                    {
                        m_State = AIState.MOVING_TO_GOAL;
                    }
                    m_CurrentWaitTime -= Time.deltaTime;
                    break;
                case  AIState.WAITING_GOAL_DELAY:
                    if(m_CurrentGoalWaitTime < 0.0f)
                    {
                        m_State = AIState.MOVING_TO_GOAL;
                        return true;
                    }
                    m_CurrentGoalWaitTime -= Time.deltaTime;
                    break;
            }
            
            return false;
        }


        public void Wait(float aSeconds)
        {
            m_WaitTime = aSeconds;
            Wait();
        }
        public void Wait()
        {
            m_CurrentWaitTime = m_WaitTime;
            m_State = AIState.WAITING;
        }

        public bool isWaiting
        {
            get { return m_State == AIState.WAITING; }
        }
        public bool isWaitingAtGoal
        {
            get { return m_State == AIState.WAITING_GOAL_DELAY; }
        }
        public bool isMovingToGoal
        {
            get { return m_State == AIState.MOVING_TO_GOAL; }
        }

        public float waitTime
        {
            get { return m_WaitTime; }
            set { m_WaitTime = value; }
        }
        public float goalWaitTime
        {
            get { return m_GoalWaitTime; }
            set { m_GoalWaitTime = value; }
        }
        public float currentWaitTime
        {
            get { return m_CurrentWaitTime; }
        }
        public float currentGoalWaitTime
        {
            get { return m_CurrentGoalWaitTime; }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (m_DrawGizmos)
            {
                Gizmos.color = Color.blue * 0.75f;
                IEnumerator<Transform> points = m_PatrolPoints.GetEnumerator();
                Vector3 size = new Vector3(m_DistanceThreshHold, m_DistanceThreshHold, m_DistanceThreshHold);
                while (points.MoveNext())
                {
                    if(points.Current == null)
                    {
                        continue;
                    }
                    Gizmos.DrawCube(points.Current.position, size);
                }
            }
        }
#endif
    }
}