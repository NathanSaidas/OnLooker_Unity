using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Gem
{
    public class UnitInventory : MonoBehaviour, IGameListener
    {
        /// <summary>
        /// The items the unit starts with
        /// </summary>
        [SerializeField]
        private ItemType[] m_StartingItems = null;      
        /// <summary>
        /// The maxmimum items a unit may carry
        /// </summary>
        [SerializeField]
        private int m_MaxItemCount = 8;

        [SerializeField]
        private List<ItemType> m_AcceptedTypes = new List<ItemType>();


        private List<Item> m_Items = new List<Item>();

        private void Start()
        {
            Game.Register(this);
            IEnumerator iter = m_StartingItems.GetEnumerator();
            while(iter.MoveNext())
            {
                ItemType type = (ItemType)iter.Current;
                if(type == ItemType.NONE)
                {
                    continue;
                }
                AddItem(ItemDatabase.QueryItem(type));
               
            }
        }
        private void OnDestroy()
        {
            Game.Unregister(this);
        }

        private void OnTriggerEnter(Collider aCollider)
        {
            ItemPickUp pickup = aCollider.GetComponent<ItemPickUp>();
            if(pickup != null && pickup.itemType != ItemType.NONE)
            {
                if(!m_AcceptedTypes.Any(Element => Element == pickup.itemType))
                {
                    return;
                }

                Item item = GetItem(pickup.itemType);
                if(item != null)
                {
                    if (item.isStackable && item.isFull == false)
                    {
                        item.AddStack();
                    }
                    else
                    {
                        DebugUtils.Log("Item is not stabkle or is full");
                    }
                }
                else
                {
                    AddItem(ItemDatabase.QueryItem(pickup.itemType));
                }

                pickup.gameObject.SetActive(false);
            }
        }

        public bool AddItem(Item aItem)
        {
            if(m_Items.Count >= m_MaxItemCount || aItem == null)
            {
                return false;
            }
            aItem.inventory = this;
            m_Items.Add(aItem);
            return true;
        }
        public bool RemoveItem(Item aItem)
        {
            if(aItem == null)
            {
                return false;
            }
            
            if( m_Items.Remove(aItem))
            {
                aItem.inventory = null;
                return true;
            }
            return false;
        }
        public bool RemoveItem(int aIndex)
        {
            if(aIndex > m_MaxItemCount)
            {
                return false;
            }
            if (aIndex >= 0 && aIndex < m_Items.Count)
            {
                Item item = m_Items[aIndex];
                item.inventory = null;
                m_Items.RemoveAt(aIndex);
                return true;
            }
            return false;
        }
        public bool RemoveItem(string aItemName)
        {
            if(aItemName.Length == 0 || m_Items.Count == 0)
            {
                return false;
            }
            IEnumerator<Item> iter = m_Items.GetEnumerator();
            while(iter.MoveNext())
            {
                if(iter.Current == null)
                {
                    continue;
                }
                if(iter.Current.itemName == aItemName)
                {
                    iter.Current.inventory = null;
                    m_Items.Remove(iter.Current);
                    return true;
                }
            }
            return false;
        }
        public Item GetItem(int aIndex)
        {
            if (aIndex > m_MaxItemCount)
            {
                return null;
            }
            if (aIndex >= 0 && aIndex < m_Items.Count)
            {
                m_Items.RemoveAt(aIndex);
                return m_Items[aIndex];
            }
            return null;
        }
        public Item GetItem(string aItemName)
        {
            if (aItemName.Length == 0 || m_Items.Count == 0)
            {
                return null;
            }
            IEnumerator<Item> iter = m_Items.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current == null)
                {
                    continue;
                }
                if (iter.Current.itemName == aItemName)
                {
                    return iter.Current;
                }
            }
            return null;
        }
        public Item GetItem(ItemType aType)
        {
            if (aType == ItemType.NONE || m_Items.Count == 0)
            {
                return null;
            }
            IEnumerator<Item> iter = m_Items.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current == null)
                {
                    continue;
                }
                if (iter.Current.itemType == aType)
                {
                    return iter.Current;
                }
            }
            return null;
        }



        public void OnGamePaused()
        {
            
        }

        public void OnGameUnpaused()
        {
            
        }

        public void OnGameReset()
        {
            m_Items.Clear();
            IEnumerator iter = m_StartingItems.GetEnumerator();
            while (iter.MoveNext())
            {
                ItemType type = (ItemType)iter.Current;
                if (type == ItemType.NONE)
                {
                    continue;
                }
                AddItem(ItemDatabase.QueryItem(type));
            }
        }
    }
}
