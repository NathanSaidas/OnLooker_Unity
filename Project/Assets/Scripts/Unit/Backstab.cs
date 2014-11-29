using UnityEngine;
using System.Collections.Generic;

namespace Gem
{

    public class Backstab : BasicAttack
    {
        public override void Execute()
        {
            IEnumerable<Unit> units = UnitManager.GetAllUnits();
            if (units == null)
            {
                return;
            }
            IEnumerator<Unit> iter = units.GetEnumerator();
            while (iter.MoveNext())
            {
                if (owner == null)
                {
                    continue;
                }
                if (iter.Current == null)
                {
                    continue;
                }
                if (iter.Current.faction == owner.faction)
                {
                    continue;
                }
                if (CheckUnit(iter.Current) == true)
                {
                    Vector3 targetLookDirection = (owner.transform.position - iter.Current.transform.position).normalized;
                    float angle = Vector3.Dot(targetLookDirection,iter.Current.transform.forward);
                    if(angle > 0.0f)
                    {
                        iter.Current.ReceiveDamage(damage);
                    }
                    else
                    {
                        Debug.Log("Back stab");
                        iter.Current.ReceiveDamage(damage * 2.0f);
                    }

                    
                    break;
                }
            }
         
        }
    }
}