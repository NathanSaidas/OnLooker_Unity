using UnityEngine;
using System;
using System.Collections;

namespace Gem
{
    public enum AbilityType
    {
        BASIC_ATTACK,
        BASIC_ATTACK_RANGED,
        ACTION,
        CHANNELED
    }

    [Serializable]
    public class Ability : ScriptableObject
    {
        /// <summary>
        /// The owner of the ability
        /// </summary>
        [SerializeField]
        private Unit m_Owner = null;
        /// <summary>
        /// The name of the ability
        /// </summary>
        [SerializeField]
        private string m_AbilityName = string.Empty;
        /// <summary>
        /// The type of ability
        /// </summary>
        [SerializeField]
        private AbilityType m_AbilityType = AbilityType.BASIC_ATTACK;
        [SerializeField]
        private AttackType m_AttackType = AttackType.NONE;
        /// <summary>
        /// The cool down time of the ability
        /// </summary>
        [SerializeField]
        private float m_AbilityCooldown = 0.0f;
        /// <summary>
        /// The required resources to use the ability
        /// </summary>
        [SerializeField]
        private float m_ResourceCost = 0.0f;

        private Ability m_Next = null;
        private Ability m_Previous = null;
        private float m_CurrentTime = 0.0f;
        private bool m_InCast = false;

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
            m_InCast = true;
        }
        /// <summary>
        /// Ends the ability cast
        /// </summary>
        public virtual void EndExecute()
        {
            m_InCast = false;
        }
        /// <summary>
        /// Returns true if the owner of the ability has the required resources
        /// </summary>
        /// <returns></returns>
        public virtual bool CheckResource()
        {
            if(m_Owner == null)
            {
                return false;
            }
            return m_Owner.resource >= m_ResourceCost;
        }

        /// <summary>
        /// Gets called when the owner is set
        /// </summary>
        public virtual void UpdateReference()
        {

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
        public float resourceCost
        {
            get { return m_ResourceCost; }
            set { m_ResourceCost = value; }
        }

        public float currentTime
        {
            get { return m_CurrentTime; }
            set { m_CurrentTime = value; }
        }
        public bool inCast
        {
            get { return m_InCast; }
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