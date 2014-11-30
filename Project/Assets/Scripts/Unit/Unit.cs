using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Gem
{
    public enum UnitOrder
    {
        NONE,
    }
    public enum UnitType
    {
        PLAYER,
        ENEMY
    }

    public class UnitOrderParams
    {
        private UnitOrder m_OrderID;

        public UnitOrder orderID
        {
            get { return m_OrderID; }
            set { m_OrderID = value; }
        }
    }

    public class Unit : MonoBehaviour
    {
        
        /// <summary>
        /// The name of the unit
        /// </summary>
        [SerializeField]
        private string m_UnitName = string.Empty;
        /// <summary>
        /// The type of the unit
        /// </summary>
        [SerializeField]
        private UnitType m_UnitType = UnitType.ENEMY;
        /// <summary>
        /// The inventory of the unit.
        /// </summary>
        private UnitInventory m_Inventory = null;
        /// <summary>
        /// Represents a team ID
        /// </summary>
        [SerializeField]
        private int m_Faction = 0;
        /// <summary>
        /// The units ID
        /// </summary>
        private int m_UnitID = 0;

        [SerializeField]
        private float m_MaxHealth = 0.0f;
        [SerializeField]
        private float m_Health = 0.0f;
        [SerializeField]
        private float m_MaxResource = 0.0f;
        [SerializeField]
        private float m_Resource = 0.0f;
        [SerializeField]
        private float m_MovementSpeed = 0.0f;

        [SerializeField]
        private List<Ability> m_Abilities = new List<Ability>();
        private Ability m_SelectedAbility = null;

        [SerializeField]
        private Transform m_LeftHand = null;
        [SerializeField]
        private Transform m_RightHand = null;
        [SerializeField]
        private Transform m_Origin = null;
        [SerializeField]
        private Transform m_Head = null;
        [SerializeField]
        private Transform m_Torso = null;

        UIAbilities m_AbilityHud = null;

        protected virtual void Start()
        {
            UnitManager.Register(this);
            

            List<Ability> abilities = new List<Ability>();
            IEnumerator<Ability> iter = m_Abilities.GetEnumerator();
            while(iter.MoveNext())
            {
                if(iter.Current == null)
                {
                    continue;
                }
                abilities.Add(Instantiate(iter.Current) as Ability);
            }

            m_Abilities = abilities;
            UpdateAbilityReferences();
            if (m_Abilities.Count > 0)
            {
                m_SelectedAbility = m_Abilities[0];
            }
            m_Inventory = GetComponent<UnitInventory>();
        }
        protected virtual void OnDestroy()
        {
            UnitManager.Unregister(this);
        }

        

        private void Update()
        {
            if(m_Health <= 0.0f)
            {
                Kill();
            }

            IEnumerator<Ability> iter = m_Abilities.GetEnumerator();
            while(iter.MoveNext())
            {
                if(iter.Current == null)
                {
                    continue;
                }
                float deltaTime = Time.deltaTime;
                iter.Current.UpdateAbility(deltaTime);
            }

            if(isAlive)
            {
                RestoreResource(UnitResourceType.RESOURCE, 0.05f);
            }
        }

        public void ReceiveOrder(UnitOrderParams aParams)
        {

        }
        public void ReceiveDamage(float aDamage)
        {
            bool wasAlive = isAlive;
            aDamage = Mathf.Abs(aDamage);
            m_Health -= aDamage;
            if (m_Health <= 0.0f)
            {
                Kill();
                if(wasAlive)
                {
                    GameEventManager.InvokeEvent(new GameEventData(Time.time,
                    GameEventID.UNIT_KILLED,
                    GameEventType.UNIT,
                    this,
                    null));
                }
            }
        }
        public void ReceiveHealing(float aHealing)
        {
            aHealing = Mathf.Abs(aHealing);
            m_Health += aHealing;
            if (m_Health > m_MaxHealth)
            {
                m_Health = m_MaxHealth;
            }
        }
        public void UseResource(UnitResourceType aType, float aAmount)
        {
            switch(aType)
            {
                case UnitResourceType.HEALTH:
                    ReceiveDamage(aAmount);
                    break;
                case UnitResourceType.RESOURCE:
                    m_Resource -= aAmount;
                    m_Resource = Mathf.Clamp(m_Resource, 0.0f, m_MaxResource);
                    break;
            }
        }
        public void RestoreResource(UnitResourceType aType, float aAmount)
        {
            switch (aType)
            {
                case UnitResourceType.RESOURCE:
                    m_Resource += aAmount;
                    m_Resource = Mathf.Clamp(m_Resource, 0.0f, m_MaxResource);
                    break;
            }
        }
        public void Revive()
        {
            if(isAlive)
            {
                return;
            }
            bool wasDead = !isAlive;
            m_Health = m_MaxHealth;
            gameObject.SetActive(true);
            if(wasDead)
            {
                GameEventManager.InvokeEvent(new GameEventData(Time.time,
                    GameEventID.UNIT_REVIVED,
                    GameEventType.UNIT,
                    this,
                    null));
            }
        }
        public void Revive(float aHealthPercentage)
        {
            if (isAlive)
            {
                return;
            }
            bool wasDead = !isAlive;
            aHealthPercentage = Mathf.Clamp01(aHealthPercentage);
            m_Health = m_MaxHealth * aHealthPercentage;
            gameObject.SetActive(true);
            if (wasDead)
            {
                GameEventManager.InvokeEvent(new GameEventData(Time.time,
                    GameEventID.UNIT_REVIVED,
                    GameEventType.UNIT,
                    this,
                    null));
            }
        }
        public void Kill()
        {
            bool wasAlive = isAlive;
            m_Health = 0.0f;
            gameObject.SetActive(false);
            if (wasAlive)
            {
                GameEventManager.InvokeEvent(new GameEventData(Time.time,
                    GameEventID.UNIT_KILLED,
                    GameEventType.UNIT,
                    this,
                    null));
            }
        }
        #region AbilitySelection
        public void NextAbility(bool aCheckCooldown)
        {
            int count = m_Abilities.Count;
            if(count == 0 || count == 1)
            {
                return;
            }
            int index = 0;
            Ability ability = m_SelectedAbility;
            if(m_SelectedAbility != null)
            {
                while(index < count && ability != null)
                {
                    ability = ability.next;
                    index++;
                    if (ability.isOnCooldown == true && aCheckCooldown == true)
                    {
                        continue;
                    }
                    m_SelectedAbility = ability;
                    if (m_AbilityHud != null)
                    {
                        m_AbilityHud.SelectAbility(m_SelectedAbility);
                    }
                    return;
                }
            }
            else if(m_Abilities.Count > 0)
            {
                m_SelectedAbility = m_Abilities[0];
            }
        }
        public void PreviousAbility(bool aCheckCooldown)
        {
            int count = m_Abilities.Count;
            if (count == 0 || count == 1)
            {
                return;
            }
            int index = 0;
            Ability ability = m_SelectedAbility;
            if (m_SelectedAbility != null)
            {
                while (index < count && ability != null)
                {
                    ability = ability.previous;
                    index++;
                    if (ability.isOnCooldown == true && aCheckCooldown == true)
                    {
                        continue;
                    }
                    m_SelectedAbility = ability;
                    if (m_AbilityHud != null)
                    {
                        m_AbilityHud.SelectAbility(m_SelectedAbility);
                    }
                    return;
                }
            }
            else if (m_Abilities.Count > 0)
            {
                m_SelectedAbility = m_Abilities[0];
            }
        }
        public void SelectAbility(int aIndex)
        {
            if(aIndex >= 0 && aIndex < m_Abilities.Count)
            {
                m_SelectedAbility = m_Abilities[aIndex];
                if (m_AbilityHud != null)
                {
                    m_AbilityHud.SelectAbility(m_SelectedAbility);
                }
            }
        }
        public void SelectAbility(string aName)
        {
            IEnumerator<Ability> iter = m_Abilities.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current == null)
                {
                    continue;
                }
                if (iter.Current.abilityName == aName)
                {
                    m_SelectedAbility = iter.Current;
                    if (m_AbilityHud != null)
                    {
                        m_AbilityHud.SelectAbility(m_SelectedAbility);
                    }
                    return;
                }
            }
        }
        public void SelectAbility(AbilityType aType)
        {
            IEnumerator<Ability> iter = m_Abilities.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current == null)
                {
                    continue;
                }
                if(iter.Current.abilityType == aType)
                {
                    m_SelectedAbility = iter.Current;
                    if(m_AbilityHud != null)
                    {
                        m_AbilityHud.SelectAbility(m_SelectedAbility);
                    }
                    return;
                }
            }
        }
        #endregion

        public void ExecuteAbility()
        {
            if(m_SelectedAbility != null && m_SelectedAbility.isOnCooldown == false)
            {
                if(m_SelectedAbility.CheckResource())
                {
                    m_SelectedAbility.Execute();
                }
                else
                {
                    DebugUtils.Log("Not enough resource");
                }
            }
        }
        public void ExecuteAbilityIgnoreCooldown()
        {
            if(m_SelectedAbility != null)
            {
                m_SelectedAbility.Execute();
            }
        }
            
        public void AddAbility(Ability aAbility)
        {
            if(aAbility == null)
            {
                return;
            }
            if ( m_Abilities.Any(Elemenet => Elemenet.abilityName == aAbility.abilityName))
            {
                DebugUtils.LogWarning("Unit " + unitName + " already knows " + aAbility.abilityName);
                return;
            }
            aAbility = Instantiate(aAbility) as Ability;
            m_Abilities.Add(aAbility);
            UpdateAbilityReferences();
            if(m_Abilities.Count > 0 && m_SelectedAbility == null)
            {
                m_SelectedAbility = m_Abilities[0];
                if(m_AbilityHud != null)
                {
                    m_AbilityHud.SelectAbility(m_SelectedAbility);
                }
            }
        }
        public void RemoveAbility(AbilityType aType)
        {
            IEnumerator<Ability> iter = m_Abilities.GetEnumerator();
            while(iter.MoveNext())
            {
                if(iter.Current == null)
                {
                    continue;
                }
                if(aType == iter.Current.abilityType)
                {
                    if(iter.Current == m_SelectedAbility)
                    {
                        if(m_Abilities.Count == 1)
                        {
                            m_SelectedAbility = null;
                        }
                        else
                        {
                            NextAbility(false);
                        }
                    }
                    m_Abilities.Remove(iter.Current);
                    break;
                }
            }
            UpdateAbilityReferences();
        }

        public void UpdateAbilityReferences()
        {
            for (int i = 1; i < m_Abilities.Count; i++)
            {
                m_Abilities[i].previous = m_Abilities[i - 1];
                m_Abilities[i - 1].next = m_Abilities[i];
                m_Abilities[i].owner = this;
                m_Abilities[i].UpdateReference();
            }

            if (m_Abilities.Count > 0)
            {
                m_Abilities[0].owner = this;
                m_Abilities[0].previous = m_Abilities[m_Abilities.Count - 1];
                m_Abilities[m_Abilities.Count - 1].next = m_Abilities[0];
                m_Abilities[0].UpdateReference();
            }

        }
        

        public string unitName
        {
            get { return m_UnitName; }
            set { m_UnitName = value; }
        }
        public UnitType unitType
        {
            get { return m_UnitType; }
            set { m_UnitType = value; }
        }
        public UnitInventory inventory
        {
            get { return m_Inventory; }
        }
        public int faction
        {
            get { return m_Faction; }
            set { m_Faction = value; }
        }
        public int unitID
        {
            get { return m_UnitID; }
            set { m_UnitID = value; }
        }
        public bool isAlive
        {
            get { return m_Health > 0.0f; }
        }
        public float maxHealth
        {
            get { return m_MaxHealth; }
            set { m_MaxHealth = value; }
        }
        public float maxResource
        {
            get { return m_MaxResource; }
            set { m_MaxResource = value; }
        }
        public float health
        {
            get { return m_Health; }
            set { m_Health = value; }
        }
        public float resource
        {
            get { return m_Resource; }
            set { m_Resource = value; }
        }
        public float movementSpeed
        {
            get { return m_MovementSpeed; }
            set { m_MovementSpeed = value; }
        }
        public Ability selectedAbility
        {
            get { return m_SelectedAbility; }
            set { m_SelectedAbility = value; }
        }
        public UIAbilities abilityHud
        {
            get { return m_AbilityHud; }
            set { m_AbilityHud = value; }
        }

        public Transform leftHand
        {
            get { return m_LeftHand; }
        }
        public Transform rightHand
        {
            get { return m_RightHand; }
        }
        public Transform origin
        {
            get { return m_Origin; }
        }
        public Transform head
        {
            get { return m_Head; }
        }
        public Transform torso
        {
            get { return m_Torso; }
        }
        
    }
}