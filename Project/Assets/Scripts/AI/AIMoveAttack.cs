using UnityEngine;
using System.Collections;

namespace Gem
{
    /// <summary>
    /// A AI behaviour which moves to a unit and attacks it.
    /// </summary>
    public class AIMoveAttack : AIBehaviour
    {
        [SerializeField]
        private float m_RunSpeed = 3.5f;
        [SerializeField]
        private float m_TurnSpeed = 5.0f;
        [SerializeField]
        private float m_AttackRange = 1.0f;
        [SerializeField]
        private float m_AttackWindup = 1.0f;
        private float m_AttackTime = 0.0f;

        [SerializeField]
        private bool m_IsAttacking = false;

        private AIMotor m_Motor = null;
        private Unit m_Unit = null;

        private void Start()
        {
            m_Motor = GetComponent<AIMotor>();
            m_Unit = GetComponent<Unit>();
        }

        private void Update()
        {
            m_AttackTime -= Time.deltaTime;
            if(m_IsAttacking)
            {
                Quaternion lookRotation = Quaternion.LookRotation((m_Target.position - transform.position).normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, m_TurnSpeed * Time.deltaTime);
            }
        }

        public override bool AquireTarget(AIMotor aMotor)
        {
            m_Motor = aMotor;
            m_Motor.isRunning = true;
            m_Unit.movementSpeed = m_RunSpeed;
            return m_Target != null;
        }

        public override bool MoveToTarget(AIMotor aMotor)
        {
            if (m_Target == null)
            {
                aMotor.ResetState();
                return true;
            }
            float heightOffset = aMotor.agent.height * 0.5f;
            Vector3 origin = aMotor.transform.position;
            origin.y -= heightOffset;
            float distanceFromGoal = Vector3.Distance(origin, m_Target.position);
            if(distanceFromGoal < m_AttackRange)
            {
                if(m_AttackTime < 0.0f)
                {
                    StartAttack();
                    aMotor.agent.Stop();
                    aMotor.attackType = m_Unit.selectedAbility.attackType;
                    m_AttackTime = m_Unit.selectedAbility.abilityCooldown;
                }
            }
            else
            {
                m_IsAttacking = false;
                m_Motor.attackType = AttackType.NONE;
                m_Motor.attackMotion = Mathf.Lerp(m_Motor.attackMotion, 0.0f, Time.deltaTime);
                aMotor.agent.SetDestination(m_Target.position);
            }
            return false;
        }

        void StartAttack()
        {
            m_IsAttacking = true;
            StartCoroutine(Attack());
        }

        IEnumerator Attack()
        {
            m_Motor.attackMotion = 1.0f;
            yield return new WaitForSeconds(m_AttackWindup);
            m_Motor.attackMotion = 0.0f ;
            CharacterCamera cam = Game.gameplayCamera.GetComponent<CharacterCamera>();
            if(cam != null)
            {
                cam.ShakeCamera(0.25f, CameraShakeMode.DECREASE, new Vector3(0.1f, 0.1f, 0.1f));
            }
            m_Motor.attackType = AttackType.NONE;
            m_Unit.ExecuteAbility();
        }
    }
}