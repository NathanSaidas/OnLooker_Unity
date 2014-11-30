using UnityEngine;
using System;
using System.Collections;

namespace Gem
{
    public enum ItemType
    {
        NONE,
        BATTERY,
        CIRCUIT,
        WIRE,
        SILVER_ACCESS_CARD,
        GOLD_ACCESS_CARD
    }

    [Serializable]
    public class Item : ScriptableObject
    {
        /// <summary>
        /// The name of the item.
        /// </summary>
        [SerializeField]
        protected string m_ItemName = string.Empty;
        /// <summary>
        /// The id of the item
        /// </summary>
        [SerializeField]
        private int m_ItemID = 0;
        /// <summary>
        /// The type of item
        /// </summary>
        [SerializeField]
        protected ItemType m_ItemType = ItemType.NONE;
        [SerializeField]
        private bool m_Stackable = false;
        [SerializeField]
        private int m_Stacks = 1;
        [SerializeField]
        private int m_MaxStacks = 1;
        /// <summary>
        /// The inventory the item belongs to.
        /// </summary>
        private UnitInventory m_Inventory = null;

        

        protected virtual void OnEnable()
        {
            if(m_Stackable == true)
            {
                m_Stacks = 1;
            }
        }

        public void AddStack()
        {
            if(m_Stackable == true)
            {
                m_Stacks++;
                m_Stacks = Mathf.Clamp(m_Stacks, 1, m_MaxStacks);
            }
            else
            {
                m_Stacks = 1;
            }
        }
        public void RemoveStack()
        {
            if (m_Stackable == true)
            {
                m_Stacks--;
                m_Stacks = Mathf.Clamp(m_Stacks, 1, m_MaxStacks);
            }
            else
            {
                m_Stacks = 1;
            }
        }

        /// <summary>
        /// Accessor to the name of the item.
        /// </summary>
        public string itemName
        {
            get { return m_ItemName; }
        }
        /// <summary>
        /// Accessor of the item ID
        /// </summary>
        public int itemID
        {
            get { return m_ItemID;}
        }
        /// <summary>
        /// Accessor of the item type
        /// </summary>
        public ItemType itemType
        {
            get { return m_ItemType; }
        }
        public bool isStackable
        {
            get { return m_Stackable; }
        }
        public bool isFull
        {
            get { return isStackable && m_Stacks == m_MaxStacks; }
        }
        public int stacks
        {
            get { return m_Stacks; }
        }
        public int maxStacks
        {
            get { return m_MaxStacks; }
        }
        /// <summary>
        /// Accessor to the inventory the item is contained within.
        /// </summary>
        public UnitInventory inventory
        {
            get { return m_Inventory; }
            set { m_Inventory = value; }
        }       
        public void HashItem()
        {
            if(m_ItemID == 0)
            {
                m_ItemID = ItemDatabase.HashItem(this);
            }
        }
    }
}