using UnityEngine;
using System.Collections.Generic;

namespace EndevGame
{
    //Attach this to the player
    /// <summary>
    /// Character interation drives the interation between the player and Interactive Objects.
    /// </summary>
    public class CharacterInteraction : CharacterComponent
    {
        [SerializeField]
        private List<Interactive> m_InteractiveObjects = new List<Interactive>();

        [SerializeField]
        private Interactive m_FocusedObject = null;
        [SerializeField]
        private Interactive m_ObjectInUse = null;

        [SerializeField]
        private float m_SearchDistance = 5.0f;



        void Start()
        {
            manager = GetComponent<CharacterManager>();
        }

        protected override void Update()
        {
            base.Update();
        }
        protected override void SlowUpdate()
        {
            base.SlowUpdate();
        }
    }
}