using UnityEngine;
using System;
using System.Collections;

namespace Gem
{
    public enum ItemType
    {
        NONE,
        KEY,
        PENDANT,
        SUPER_GUN,
        SWORD,
        RIFLE,
        SHEILD,
        HAT,
        MONEY_CRATE
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
        /// <summary>
        /// The inventory the item belongs to.
        /// </summary>
        private UnitInventory m_Inventory = null;

        public Item()
        {
        }

        protected virtual void OnEnable()
        {

        }

        /// <summary>
        /// Accessor to the name of the item.
        /// </summary>
        public string itemName
        {
            get { return m_ItemName; }
            set { m_ItemName = value; }
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
            set { m_ItemType = value; }
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