using UnityEngine;
using System.Collections;

namespace Gem
{
    public class CharacterSpawner : MonoBehaviour
    {
        [SerializeField]
        GameObject m_PlayerPrefab = null;
        [SerializeField]
        GameObject m_PlayerUI = null;

        [SerializeField]
        Ability[] m_StartingAbilities = null;
        [SerializeField]
        ItemType[] m_StartingItems = null;

        void Start()
        {
            GameObject character = Instantiate(m_PlayerPrefab, transform.position, transform.rotation) as GameObject;
            GameObject go = (GameObject)Instantiate(m_PlayerUI, transform.position, transform.rotation);
            UIBar healthBar = go.GetComponent<UIBar>();
            Unit unit = character.GetComponent<Unit>();
            if(unit != null)
            {
                IEnumerator abilities = m_StartingAbilities.GetEnumerator();
                while(abilities.MoveNext())
                {
                    Ability current = abilities.Current as Ability;
                    if(current != null)
                    {
                        unit.AddAbility(current);
                    }
                }
                UnitInventory inventory = unit.inventory;
                if(inventory != null)
                {
                    IEnumerator item = m_StartingItems.GetEnumerator();
                    if(item.MoveNext())
                    {
                        ItemType current = (ItemType)item.Current;
                        inventory.AddItem(ItemDatabase.QueryItem(current));
                    }
                }
            }
        }


#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.position, Vector3.one);
            Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        }
#endif
    }
}