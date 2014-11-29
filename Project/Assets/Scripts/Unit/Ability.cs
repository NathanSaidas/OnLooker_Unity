using UnityEngine;
using System;
using System.Collections;

namespace Gem
{
    public enum AbilityType
    {
        BASIC_ATTACK,
        BASIC_ATTACK_RANGED,
        ACTION
    }

    [Serializable]
    public class Ability : ScriptableObject
    {

        [SerializeField]
        private Unit m_Owner = null;
        [SerializeField]
        private string m_AbilityName = string.Empty;
        [SerializeField]
        private AbilityType m_AbilityType = AbilityType.BASIC_ATTACK;
        [SerializeField]
        private AttackType m_AttackType = AttackType.NONE;
        [SerializeField]
        private float m_AbilityCooldown = 0.0f;

        private Ability m_Next = null;
        private Ability m_Previous = null;
        private float m_CurrentTime = 0.0f;

        protected virtual void OnDestroy()
        {
            
        }
        protected virtual void OnEnable()
        {
            
        }
        /// <summary>
        /// Updates the cooldown timer.
        /// </summary>
        /// <param name="aTime"></param>
        public virtual void UpdateAbility(float aTime)
        {
            m_CurrentTime -= aTime;
        }
        /// <summary>
        /// Executes the ability
        /// </summary>
        public virtual void Execute()
        {
            m_CurrentTime = m_AbilityCooldown;
        }
        
        public string abilityName
        {
            get { return m_AbilityName; }
            set { m_AbilityName = value; }
        }
        public AbilityType abilityType
        {
            get { return m_AbilityType; }
            set { m_AbilityType = value; }
        }
        public Unit owner
        {
            get { return m_Owner; }
            set { m_Owner = value; }
        }
        public float abilityCooldown
        {
            get { return m_AbilityCooldown; }
            set { m_AbilityCooldown = value; }
        }
        public float currentTime
        {
            get { return m_CurrentTime; }
            set { m_CurrentTime = value; }
        }
        public bool isOnCooldown
        {
            get { return m_CurrentTime > 0.0; }
        }
        public Ability next
        {
            get { return m_Next; }
            set { m_Next = value; }
        }
        public Ability previous
        {
            get { return m_Previous; }
            set { m_Previous = value; }
        }
        public AttackType attackType
        {
            get { return m_AttackType; }
            set { m_AttackType = value; }
        }

    }
}