using UnityEngine;
using System.Collections.Generic;

namespace Gem
{
    public class ItemPickUp : MonoBehaviour , IGameListener
    {
        [SerializeField]
        private ItemType m_ItemType = ItemType.NONE;

        private void Start()
        {
            Game.Register(this);
        }
        private void OnDestroy()
        {
            Game.Unregister(this);
        }

        public ItemType itemType
        {
            get { return m_ItemType; }
        }

        public void OnGamePaused()
        {

        }

        public void OnGameUnpaused()
        {

        }

        public void OnGameReset()
        {
            gameObject.SetActive(true);
        }
    }
}