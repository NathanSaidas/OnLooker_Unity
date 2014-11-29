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

            if(Input.GetMouseButtonDown(0) && m_AttackTime < 0.0f)
            {
                StartAttack();
                m_Motor.attackType = m_Unit.selectedAbility.attackType;
                m_AttackTime = m_Unit.selectedAbility.abilityCooldown;
            }
            if(InputManager.GetButton("Sprint"))
            {
                m_Unit.movementSpeed = m_SprintSpeed;
            }
            else
            {
                m_Unit.movementSpeed = m_RunSpeed;
            }
            
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                m_Unit.SelectAbility(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                m_Unit.SelectAbility(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                m_Unit.SelectAbility(2);
            }
        }

        private void StartAttack()
        {
            StartCoroutine(Attack());
        }

        IEnumerator Attack()
        {
            m_Motor.attackMotion = 1.0f;
            //TODO: Check Ability Cast Time
            yield return new WaitForSeconds(m_AttackWindup);
            m_Motor.attackMotion = 0.0f;
            CharacterCamera cam = Game.gameplayCamera.GetComponent<CharacterCamera>();
            if (cam != null)
            {
                cam.ShakeCamera(0.10f, CameraShakeMode.DECREASE, new Vector3(0.05f, 0.05f, 0.05f));
            }
            m_Motor.attackType = AttackType.NONE;
            m_Unit.ExecuteAbility();
        }
    }
}