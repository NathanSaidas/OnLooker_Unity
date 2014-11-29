using UnityEngine;
using System;
using System.Collections.Generic;

namespace Gem
{
    [Serializable]
    public class BasicAttack : Ability
    {
        [SerializeField]
        private float m_Damage = 5.0f;
        [SerializeField]
        private float m_Distance = 1.0f;
        [SerializeField]
        private float m_Arc = 180.0f;

        public override void Execute()
        {
            base.Execute();
            IEnumerable<Unit> units =  UnitManager.GetAllUnits();
            if(units == null)
            {
                return;
            }
            IEnumerator<Unit> iter = units.GetEnumerator();
            while(iter.MoveNext())
            {
                if(owner == null)
                {
                    continue;
                }
                if(iter.Current == null)
                {
                    continue;
                }
                if(iter.Current.faction == owner.faction)
                {
                    continue;
                }
                if(CheckUnit(iter.Current) == true)
                {
                    iter.Current.ReceiveDamage(m_Damage);
                    break;
                }
            }
        }

        protected bool CheckUnit(Unit aUnit)
        {
            Vector3 direction = (aUnit.transform.position - owner.transform.position).normalized;
            float cosAngle = Mathf.Cos(m_Arc * 0.5f);
            float angle = Vector3.Dot(direction, owner.transform.forward);

            if(angle > cosAngle && Vector3.Distance(aUnit.transform.position,owner.transform.position) < m_Distance)
            {
                return true;
            }
            return false;
        }

        public float damage
        {
            get { return m_Damage; }
            set { m_Damage = value; }
        }
        public float distance
        {
            get { return m_Distance; }
            set { m_Distance = value; }
        }
        public float arc
        {
            get { return m_Arc; }
            set { m_Arc = value; }
        }

    }
}