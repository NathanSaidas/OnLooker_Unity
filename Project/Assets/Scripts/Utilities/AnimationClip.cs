using UnityEngine;
using System;
using System.Collections;

namespace EndevGame
{

    #region
    /* October 7 2014, Nathan Hanlan, Added and implemented the class
     * 
     * 
    */
    #endregion
    /// <summary>
    /// This class represents a data structure used to help setup animation clips in editor.
    /// </summary>
    [Serializable]
    public class AnimationClip : EndevGame.Object
    {
        ///Using EndevGame.Object.m_Name as the name of the animation

        /// <summary>
        /// The animation clip to put into the animation
        /// </summary>
        [SerializeField]
        private UnityEngine.AnimationClip m_AnimationClip = null;
        /// <summary>
        /// The specified wrap mode
        /// </summary>
        [SerializeField]
        private WrapMode m_WrapMode = WrapMode.Once;
        

        public UnityEngine.AnimationClip animationClip
        {
            get { return m_AnimationClip; }
            set { m_AnimationClip = value; }
        }
        public WrapMode wrapMode
        {
            get { return m_WrapMode; }
            set { m_WrapMode = value; }
        }
    }
}