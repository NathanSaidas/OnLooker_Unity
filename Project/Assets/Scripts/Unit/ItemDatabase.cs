using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Gem
{


    public class ItemDatabase : MonoBehaviour
    {
        #region SINGLETON
        /// <summary>
        /// A singleton instance of ItemDatabase
        /// </summary>
        private static ItemDatabase s_Instance = null;
        /// <summary>
        /// An accessor which creates an instance ofItemDatabase if it is null
        /// </summary>
        private static ItemDatabase instance
        {
            get { if (s_Instance == null) { CreateInstance(); } return s_Instance; }
        }
        /// <summary>
        /// Creates an instance of the ItemDatabase if it was missing.
        /// </summary>
        private static void CreateInstance()
        {
            if(Game.isClosing)
            {
                return;
            }
            GameObject persistant = GameObject.Find(Game.PERSISTANT_GAME_OBJECT_NAME);
            if (persistant == null)
            {
                persistant = new GameObject(Game.PERSISTANT_GAME_OBJECT_NAME);
                persistant.transform.position = Vector3.zero;
                persistant.transform.rotation = Quaternion.identity;
            }
            s_Instance = persistant.GetComponent<ItemDatabase>();
            if (s_Instance == null)
            {
                s_Instance = persistant.AddComponent<ItemDatabase>();
            }

        }
        /// <summary>
        /// Sets the instance of the ItemDatabase to the instance given.
        /// </summary>
        /// <param name="aInstance">The instance to make singleton</param>
        /// <returns></returns>
        private static bool SetInstance(ItemDatabase aInstance)
        {
            if (s_Instance != null && s_Instance != aInstance)
            {
                return false;
            }
            s_Instance = aInstance;
            return true;
        }
        /// <summary>
        /// Removes the instance of the ItemDatabase if the instance being destroyed is the the same as the singleton.
        /// </summary>
        /// <param name="aInstance"></param>
        private static void DestroyInstance(ItemDatabase aInstance)
        {
            if (s_Instance == aInstance)
            {
                s_Instance = null;
            }
        }
        #endregion

        private const string NO_ITEM_DATABASE = "Created item database without any items";
        private const string ITEM_DATA_BASE_FAILURE = "Failed to create item database";

        [SerializeField]
        private List<Item> m_ItemList = new List<Item>();
        private HashSet<Item> m_Items = null;
        private UniqueNumberGenerator m_IDGenerator = null;

        private void Awake()
        {
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            m_Items = new HashSet<Item>();
            m_IDGenerator = new UniqueNumberGenerator(1);
            IEnumerator<Item> iter = m_ItemList.GetEnumerator();;
            while(iter.MoveNext())
            {
                if(iter.Current == null)
                {
                    continue;
                }

                Item item = iter.Current;
                item.HashItem();
                m_Items.Add(item);
            }

            if(m_Items != null && m_Items.Count == 0)
            {
                DebugUtils.LogWarning(NO_ITEM_DATABASE);
            }
            if(m_Items == null )
            {
                DebugUtils.LogWarning(ITEM_DATA_BASE_FAILURE);
            }
            m_ItemList = null;
        }
        private void InitializeDatabase(IEnumerable<Item> aItems)
        {
            m_Items = new HashSet<Item>();
            m_IDGenerator = new UniqueNumberGenerator(1);
            IEnumerator<Item> iter = aItems.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current == null)
                {
                    continue;
                }

                Item item = iter.Current;
                item.HashItem();
            }
        }

        private void AppendDatabase(IEnumerable<Item> aItems)
        {
            IEnumerator<Item> iter = aItems.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current == null)
                {
                    continue;
                }

                Item item = iter.Current;
                item.HashItem();
            }
        }

        /// <summary>
        /// Returns the first item of matching name
        /// </summary>
        /// <param name="aName"></param>
        /// <returns></returns>
        private Item GetItemInstance(string aName)
        {
            if(m_Items == null || aName.Length == 0)
            {
                return null;
            }
            Item item = m_Items.First(Element => Element.itemName == aName);
            if(item != null)
            {
                item = (Item)Instantiate(item);
            }
            return item;
        }
        /// <summary>
        /// Returns the first item of the ID
        /// </summary>
        /// <param name="aID"></param>
        /// <returns></returns>
        private Item GetItemInstance(int aID)
        {
            if (m_Items == null || aID == 0)
            {
                return null;
            }
            Item item = m_Items.First(Element => Element.itemID == aID);
            if (item != null)
            {
                item = (Item)Instantiate(item);
            }
            return item;
        }
        /// <summary>
        /// Returns the first item of the type
        /// </summary>
        /// <param name="aType"></param>
        /// <returns></returns>
        private Item GetItemInstance(ItemType aType)
        {
            if (m_Items == null || aType == ItemType.NONE)
            {
                return null;
            }
            Item item = m_Items.First(Element => Element.itemType == aType);
            if (item != null)
            {
                item = (Item)Instantiate(item);
            }
            return item;
        }

        /// <summary>
        /// Gets the ID of the specified item string
        /// </summary>
        /// <param name="aItemName"></param>
        /// <returns></returns>
        private int GetItemID(string aItemName)
        {
            if (m_Items == null || aItemName.Length == 0)
            {
                return 0;
            }
            Item item = m_Items.First(Element => Element.itemName == aItemName);
            if (item != null)
            {
                return item.itemID;
            }
            return 0;
        }
        /// <summary>
        /// Gets the first item whos name matches and creates an instance of it.
        /// </summary>
        /// <param name="aName"></param>
        /// <returns></returns>
        public static Item QueryItem(string aName)
        {
            return instance.GetItemInstance(aName);
        }
        /// <summary>
        /// Gets the first item whos ID matches and creates an instance of it
        /// </summary>
        /// <param name="aId"></param>
        /// <returns></returns>
        public static Item QueryItem(int aId)
        {
            return instance.GetItemInstance(aId);
        }
        /// <summary>
        /// Gets the first item whos type matches and creates an instance of it.
        /// </summary>
        /// <param name="aType"></param>
        /// <returns></returns>
        public static Item QueryItem(ItemType aType)
        {
            return instance.GetItemInstance(aType);
        }
        /// <summary>
        /// Gets the ID of the item based on the search string parameter.
        /// </summary>
        /// <param name="aName"></param>
        /// <returns></returns>
        public static int QueryItemID(string aName)
        {
            return instance.GetItemID(aName);
        }


        public static int HashItem(Item aItem)
        {
            if(aItem == null || s_Instance == null)
            {
                return 0; //Invalid Item
            }
            return instance.m_IDGenerator.Get();
        }
    }
}
