using UnityEngine;
using System.Collections;

namespace Gem
{
    
    public class PowerSurge : Ability
    {
        [SerializeField]
        private GameObject m_ProjectilePrefab = null;
        [SerializeField]
        private float m_Damage = 1.0f;


        public override bool CheckResource()
        {
            return base.CheckResource();
        }
        public override void Execute()
        {
            if (!inCast && m_ProjectilePrefab != null && owner != null)
            {
                GameObject obj = (GameObject)Instantiate(m_ProjectilePrefab, owner.transform.position + owner.transform.forward, owner.transform.rotation);
                PowerSurgeEffect powerSurge = obj.GetComponent<PowerSurgeEffect>();
                if (powerSurge != null)
                {
                    powerSurge.owner = owner;
                    powerSurge.damage = m_Damage;
                }
                owner.UseResource(UnitResourceType.RESOURCE, resourceCost);
            }
            base.Execute();
        }

        public override void UpdateAbility(float aTime)
        {
            base.UpdateAbility(aTime);
        }

        public override void UpdateReference()
        {
            base.UpdateReference();
        }

        public override void EndExecute()
        {
            base.EndExecute();
        }
    }
}