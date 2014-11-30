using UnityEngine;
using System.Collections;

namespace Gem
{

    public class CharacterAction : MonoBehaviour
    {

        private CharacterMotor m_Motor;
        private Unit m_Unit = null;


        [SerializeField]
        private float m_AttackWindup = 1.0f;
        private float m_AttackTime = 0.0f;

        [SerializeField]
        private float m_RunSpeed = 5.0f;
        [SerializeField]
        private float m_SprintSpeed = 6.5f;

        private Ability m_TriggeredAbility = null;


        private float m_CurrentTime = 0.0f;

        
        // Use this for initialization
        void Start()
        {
            m_Motor = GetComponent<CharacterMotor>();
            m_Unit = GetComponent<Unit>();
        }

        // Update is called once per frame
        void Update()
        {
            m_AttackTime -= Time.deltaTime;

            ///Check Sprint
            if(InputManager.GetButton(GameConstants.INPUT_SPRINT))
            {
                m_Unit.movementSpeed = m_SprintSpeed;
            }
            else
            {
                m_Unit.movementSpeed = m_RunSpeed;
            }


            if(InputManager.GetButtonDown(GameConstants.INPUT_NEXT))
            {
                m_Unit.NextAbility(false);
            }
            if (InputManager.GetButtonDown(GameConstants.INPUT_PREVIOUS))
            {
                m_Unit.PreviousAbility(false);
            }

            if(InputManager.GetButton(GameConstants.INPUT_ATTACK))
            {
                Attack(); 
            }
            else
            {
                StopAttack();
            }
            
        }

        private void Attack()
        {
            ///Get current ability
            if(m_Unit == null)
            {
                return;
            }
            Ability currentAbility = m_Unit.selectedAbility;
            if(currentAbility == null)
            {
                DebugUtils.LogWarning("Unit is missing an ability to attack with.");
                return;
            }
            ///Check the last ability with the current ability
            if(currentAbility != m_TriggeredAbility)
            {
                ///Cancelled attack?
                if(m_TriggeredAbility != null)
                {
                    GameEventManager.InvokeEvent(new GameEventData(Time.time, GameEventID.UNIT_ATTACK_CANCELLED, GameEventType.UNIT, this, m_TriggeredAbility));
                }
                m_CurrentTime = 0.0f;
            }
            ///Update time and ability.
            m_TriggeredAbility = currentAbility;
            ///Ability first started
            if(m_CurrentTime == 0.0f)
            {
                if (m_TriggeredAbility != null)
                {
                    GameEventManager.InvokeEvent(new GameEventData(Time.time, GameEventID.UNIT_ATTACK_BEGIN, GameEventType.UNIT, this, m_TriggeredAbility));
                }
            }
            m_CurrentTime += Time.deltaTime;


            ///Start executing ability
            if(m_CurrentTime > m_AttackWindup)
            {
                ///Execute ability event
                if (m_TriggeredAbility != null)
                {
                    GameEventManager.InvokeEvent(new GameEventData(Time.time, GameEventID.UNIT_ATTACK_EXECUTE, GameEventType.UNIT, this, m_TriggeredAbility));
                }
                DebugUtils.Log("Unit is executing an ability");
                m_Unit.ExecuteAbility();
                if(m_TriggeredAbility.abilityType != AbilityType.CHANNELED)
                {
                    m_Motor.attackMotion = 0.0f;
                    m_Motor.attackType = AttackType.NONE;
                    if (m_TriggeredAbility != null)
                    {
                        GameEventManager.InvokeEvent(new GameEventData(Time.time, GameEventID.UNIT_ATTACK_FINISHED, GameEventType.UNIT, this, m_TriggeredAbility));
                    }
                }
            }
            else
            {
                m_Motor.attackMotion = 1.0f;
                m_Motor.attackType = m_Unit.selectedAbility.attackType;
            }
        }
        /// <summary>
        /// Stops the attack and resets the timer.
        /// </summary>
        private void StopAttack()
        {
            if(m_CurrentTime < m_AttackWindup)
            {
                //Cancelled attack
                if (m_TriggeredAbility != null)
                {
                    GameEventManager.InvokeEvent(new GameEventData(Time.time, GameEventID.UNIT_ATTACK_CANCELLED, GameEventType.UNIT, this, m_TriggeredAbility));
                }
            }
            else
            {
                if (m_TriggeredAbility != null)
                {
                    if(m_TriggeredAbility.abilityType != AbilityType.CHANNELED)
                    {
                        GameEventManager.InvokeEvent(new GameEventData(Time.time, GameEventID.UNIT_ATTACK_STOPPED, GameEventType.UNIT, this, m_TriggeredAbility));
                    }
                    else
                    {
                        GameEventManager.InvokeEvent(new GameEventData(Time.time, GameEventID.UNIT_ATTACK_FINISHED, GameEventType.UNIT, this, m_TriggeredAbility));
                    }
                }
            }
            m_CurrentTime = 0.0f;
            if(m_Motor != null)
            {
                m_Motor.attackMotion = 0.0f;
                m_Motor.attackType = AttackType.NONE;
            }
            if(m_TriggeredAbility != null)
            {
                m_TriggeredAbility.EndExecute();
            }
        }
    }
}