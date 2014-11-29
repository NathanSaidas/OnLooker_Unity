using UnityEngine;
using System;

#region CHANGE LOG
/* November,2,2014 - Nathan Hanlan, Added Set method to UIBoarder
 * November,14,2014 - Nathan Hanlan, Hid the fields and exposed them through properties.
 */
#endregion
namespace Gem
{
    /// <summary>
    /// Represents a data structure that can hold data to represents an area. 
    /// </summary>
    [Serializable]
    public struct UIBoarder
    {
        [SerializeField]
        private float m_Left;
        [SerializeField]
        private float m_Right;
        [SerializeField]
        private float m_Top;
        [SerializeField]
        private float m_Bottom;

        

        public UIBoarder(float aLeft, float aRight, float aTop, float aBottom)
        {
            m_Left = aLeft;
            m_Right = aRight;
            m_Top = aTop;
            m_Bottom = aBottom;
        }
        public void Set(float aLeft, float aRight, float aTop, float aBottom)
        {
            left = aLeft;
            right = aRight;
            top = aTop;
            bottom = aBottom;
        }

        public float left
        {
            get { return m_Left; }
            set { m_Left = value; }
        }
        public float right
        {
            get { return m_Right; }
            set { m_Right = value; }
        }
        public float top
        {
            get { return m_Top; }
            set { m_Top = value; }
        }
        public float bottom
        {
            get { return m_Bottom; }
            set { m_Bottom = value; }
        }

    }
}