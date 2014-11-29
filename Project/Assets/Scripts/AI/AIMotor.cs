using UnityEngine;
using System.Collections;

namespace Gem
{
    public enum AttackType
    {
        NONE,
        SWORD,
        WAND
    }

    /// <summary>
    /// This class makes an AI do something based on their behaviour.
    /// This class also drives the animation state machine.
    /// </summary>
    public class AIMotor : MonoBehaviour
    {

        /// <summary>
        /// This class defines its own AI state.
        /// </summary>
        private enum AIState
        {
            /// <summary>
            /// The AI is in idle
            /// </summary>
            IDLE,
            /// <summary>
            /// The AI is searching for a target based on their behaviour
            /// </summary>
            AQUIRE_TARGET,
            /// <summary>
            /// The AI is doing something to their target based on the behaviour
            /// </summary>
            TARGET_ACTION
        }

        /// <summary>
        /// The current state of the AI
        /// </summary>
        [SerializeField]
        private AIState m_State = AIState.IDLE;
        /// <summary>
        /// Determines if the AI is running or not (for animations only)
        /// </summary>
        [SerializeField]
        private bool m_IsRunning = false;
        /// <summary>
        /// The behaviour the AI is executing
        /// </summary>
        [SerializeField]
        private AIBehaviour m_Behaviour = null;
        /// <summary>
        /// The current type of attack animation. (SWORD / WAND)
        /// </summary>
        [SerializeField]
        private AttackType m_AttackType = AttackType.NONE;
        [SerializeField]
        private float m_AttackMotion = 1.0f;
        /// <summary>
        /// The nav mesh agent responsible for calculating a path and movement
        /// </summary>
        private NavMeshAgent m_Agent = null;
        /// <summary>
        /// The animation control
        /// </summary>
        private Animator m_Animator = null;
        private Unit m_Unit = null;
        #region ANIMATION HASH IDS
        private int m_ForwardID = 0;
        private int m_IsAttackingID = 0;
        private int m_AttackSwordID = 0;
        private int m_AttackWandID = 0;
        private int m_AttackMotionID = 0;
        

        private const string ANIMATION_FORWARD = "ForwardSpeed";
        private const string ANIMATION_IS_ATTACKING = "IsAttacking";
        private const string ANIMATION_ATTACK_SWORD = "AttackSword";
        private const string ANIMATION_ATTACK_WAND = "AttackWand";
        private const string ANIMATION_ATTACK_MOTION = "AttackMotion";
        #endregion
        /// <summary>
        /// Current movement speed of the AI
        /// </summary>
        private float m_CurrentSpeed = 0.0f;
        /// <summary>
        /// The last posiiton of the AI
        /// </summary>
        private Vector3 m_LastPosition = Vector3.zero;

        
        void Start()
        {
            m_State = AIState.AQUIRE_TARGET;
            m_Agent = GetComponent<NavMeshAgent>();
            m_Unit = GetComponent<Unit>();
            m_Animator = GetComponentInChildren<Animator>();
            if(m_Animator != null)
            {
                m_ForwardID = Animator.StringToHash(ANIMATION_FORWARD);
                m_IsAttackingID = Animator.StringToHash(ANIMATION_IS_ATTACKING);
                m_AttackSwordID = Animator.StringToHash(ANIMATION_ATTACK_SWORD);
                m_AttackWandID = Animator.StringToHash(ANIMATION_ATTACK_WAND);
                m_AttackMotionID = Animator.StringToHash(ANIMATION_ATTACK_MOTION);
            }

            m_LastPosition = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            switch (m_State)
            {
                case AIState.AQUIRE_TARGET:
                    if(m_Behaviour != null)
                    {
                        if(m_Behaviour.AquireTarget(this))
                        {
                            m_State = AIState.TARGET_ACTION;
                        }
                    }
                    break;
                case AIState.TARGET_ACTION:
                    if(m_Behaviour != null)
                    {
                        if(m_Behaviour.MoveToTarget(this))
                        {
                            m_State = AIState.AQUIRE_TARGET;
                        }
                    }
                    break;
            }
            m_Agent.speed = m_Unit.movementSpeed;
            UpdateAnimations();
        }

        private void UpdateAnimations()
        {

            float fps = 1.0f / Time.deltaTime;
            Vector3 velocity = (m_LastPosition - transform.position) * fps;
            m_CurrentSpeed = velocity.magnitude / m_Agent.speed;
            m_LastPosition = transform.position;
            if (m_Animator != null)
            {
                if(m_IsRunning == false)
                {
                    m_Animator.SetFloat(m_ForwardID, m_CurrentSpeed);
                }
                else
                {
                    m_Animator.SetFloat(m_ForwardID, m_CurrentSpeed * 2.0f);
                }
                switch (m_AttackType)
                {
                    case AttackType.NONE:
                        m_Animator.SetBool(m_IsAttackingID, false);
                        m_Animator.SetBool(m_AttackWandID, false);
                        m_Animator.SetBool(m_AttackSwordID, false);
                        break;
                    case AttackType.SWORD:
                        m_Animator.SetBool(m_IsAttackingID, true);
                        m_Animator.SetBool(m_AttackWandID, false);
                        m_Animator.SetBool(m_AttackSwordID, true);
                        m_Animator.SetFloat(m_AttackMotionID, m_AttackMotion);
                        break;
                    case AttackType.WAND:
                        m_Animator.SetBool(m_IsAttackingID, true);
                        m_Animator.SetBool(m_AttackWandID, true);
                        m_Animator.SetBool(m_AttackSwordID, false);
                        m_Animator.SetFloat(m_AttackMotionID, m_AttackMotion);
                        break;
                }
                
            }
        }

        public void ResetState()
        {
            m_State = AIState.AQUIRE_TARGET;
        }

        public NavMeshAgent agent
        {
            get { return m_Agent; }
            set { m_Agent = null; }
        }
        public AIBehaviour aiBehaviour
        {
            get { return m_Behaviour; }
            set { if (value != m_Behaviour) { ResetState(); } m_Behaviour = value; }
        }
        public bool isRunning
        {
            get { return m_IsRunning;}
            set { m_IsRunning = value; }
        }
        public AttackType attackType
        {
            get { return m_AttackType; }
            set { m_AttackType = value; }
        }
        public float attackMotion
        {
            get { return m_AttackMotion; }
            set { m_AttackMotion = value; }
        }
    }
}