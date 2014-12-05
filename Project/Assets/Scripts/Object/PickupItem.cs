using UnityEngine;
using System.Collections;

namespace Gem
{

    public class PickupItem : MonoBehaviour
    {
        [SerializeField]
        private string m_UnitName = string.Empty;
        [SerializeField]
        private ItemType m_ItemType = ItemType.NONE;
        [SerializeField]
        private float m_RespawnTime = 0.0f;
        private bool m_IsActive = false;
        

        void OnTriggerEnter(Collider aCollider)
        {
            if(m_ItemType == ItemType.NONE)
            {
                return;
            }
            Unit unit = aCollider.GetComponent<Unit>();
            if(unit != null && unit.unitName == m_UnitName)
            {
                UnitInventory inventory = unit.inventory;
                if(inventory != null && !inventory.isFull)
                {
                    if(m_RespawnTime == 0.0f)
                    {
                        inventory.AddItem(ItemDatabase.QueryItem(m_ItemType));
                        Destroy(gameObject);
                    }
                    else if(m_IsActive == true)
                    {
                        inventory.AddItem(ItemDatabase.QueryItem(m_ItemType));
                        m_IsActive = false;
                        StartCoroutine(RespawnRoutine());
                    }
                }
            }
        }   

        /// <summary>
        /// Waits to respawn the item.
        /// </summary>
        /// <returns></returns>
        IEnumerator RespawnRoutine()
        {
            yield return new WaitForSeconds(m_RespawnTime);
            m_IsActive = true;
        }
    }
}
