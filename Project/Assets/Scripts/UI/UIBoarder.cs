using UnityEngine;
using System;

#region CHANGE LOG
/* November,2,2014 - Nathan Hanlan, Added Set method to UIBoarder
 * 
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
        public float left;
        public float right;
        public float top;
        public float bottom;

        public UIBoarder(float aLeft, float aRight, float aTop, float aBottom)
        {
            left = aLeft;
            right = aRight;
            top = aTop;
            bottom = aBottom;
        }
        public void Set(float aLeft, float aRight, float aTop, float aBottom)
        {
            left = aLeft;
            right = aRight;
            top = aTop;
            bottom = aBottom;
        }
    }
}