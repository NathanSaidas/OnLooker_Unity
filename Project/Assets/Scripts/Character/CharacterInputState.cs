using UnityEngine;
using System;
using System.Collections;

namespace EndevGame
{

    [Serializable]
    public class CharacterInputState : EndevGame.Object
    {
        /// <summary>
        /// This represents the characters forward motion (z axis)
        /// </summary>
        [SerializeField] //Only serialized for debugging purposes.
        private float m_ForwardMotion = 0.0f;


        /// <summary>
        /// This represents the characters side motion. (x axis)
        /// </summary>
        [SerializeField]//Only serialized for debugging purposes.
        private float m_SideMotion = 0.0f;

        /// <summary>
        /// This will be true if jump button is pressed. (First frame only)
        /// </summary>
        [SerializeField]//Only serialized for debugging purposes.
        private bool m_Jump = false;

        /// <summary>
        /// This will be true if crouch button is pressed.(First frame only)
        /// </summary>
        [SerializeField]//Only serialized for debugging purposes.
        private bool m_Crouch = false;

        /// <summary>
        /// This button is also referred to as the 'Use Key' or 'Function Key'.
        /// Any action the character wants to do this button is used.
        /// This will be true if its pressed. (First Frame Only)
        /// </summary>
        [SerializeField]//Only serialized for debugging purposes.
        private bool m_Action = false;

        /// <summary>
        /// This will be true if its pressed. False if not. (All Frames)
        /// </summary>
        [SerializeField]//Only serialized for debugging purposes.
        private bool m_Grow = false;

        /// <summary>
        /// This will be true if its pressed. False if not. (All Frames)
        /// </summary>
        [SerializeField]//Only serialized for debugging purposes.
        private bool m_Shrink = false;
        /// <summary>
        /// This will be true if its pressed. (First frame only)
        /// </summary>
        [SerializeField]//Only serialized for debugging purposes.
        private bool m_Shoot = false;

        /// <summary>
        /// This button will be true if its pressed. (All Frames)
        /// </summary>
        [SerializeField]//Only serialized for debugging purposes.
        private bool m_Sprint = false;

        /// <summary>
        /// This will be positive value for scroll up, negative for scroll down. 0 for no scrolling
        /// </summary>
        [SerializeField]
        private float m_ProjectileType = 0.0f;

        /// <summary>
        /// This will be true if its pressed.(First Frame Only).
        /// </summary>
        [SerializeField]//Only serialized for debugging purposes.
        private bool m_ShootMode = false;


        public float forwardMotion
        {
            get { return m_ForwardMotion; }
            set { m_ForwardMotion = value; }
        }
        public float sideMotion
        {
            get { return m_SideMotion; }
            set { m_SideMotion = value; }
        }
        public bool jump
        {
            get { return m_Jump; }
            set { m_Jump = value; }
        }
        public bool crouch
        {
            get { return m_Crouch; }
            set { m_Crouch = value; }
        }
        public bool action
        {
            get { return m_Action; }
            set { m_Action = value; }
        }
        public bool grow
        {
            get { return m_Grow; }
            set { m_Grow = value; }
        }
        public bool shrink
        {
            get { return m_Shrink; }
            set { m_Shrink = value; }
        }
        public bool shoot
        {
            get { return m_Shoot; }
            set { m_Shoot = value; }
        }
        public bool sprint
        {
            get { return m_Sprint; }
            set { m_Sprint = value; }
        }

        public float projectileType
        {
            get { return m_ProjectileType; }
            set { m_ProjectileType = value; }
        }

        public bool shootMode
        {
            get { return m_ShootMode; }
            set { m_ShootMode = value; }
        }
    }
}