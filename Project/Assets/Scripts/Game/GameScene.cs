using UnityEngine;
using System;
using System.Collections;
#region CHANGE LOG
/* November,2,2014 - Nathan Hanlan - Added a flag for determining if the scene is a menu scene or not.
 * 
 */
#endregion
namespace Gem
{
    /// <summary>
    /// This class holds all data for a GameScene
    /// </summary>
    [Serializable]
    public class GameScene
    {
        /// <summary>
        /// The full file path of the game scene
        /// </summary>
        [SerializeField]
        private string m_ScenePath = string.Empty;
        /// <summary>
        /// The name of the scene
        /// </summary>
        [SerializeField]
        private string m_SceneName = string.Empty;
        /// <summary>
        /// The amount of delay to have for loading between scenes.
        /// </summary>
        [SerializeField]
        private float m_LoadDelay = 0.0f;
        /// <summary>
        /// The texture to display while loading.
        /// </summary>
        [SerializeField]
        private Texture m_LoadTexture = null;
        /// <summary>
        /// Determines if the scene is a level scene or a menu scene
        /// </summary>
        [SerializeField]
        private bool m_IsMenuScene = false;


        public GameScene(string aSceneName, string aPath)
        {
            m_SceneName = aSceneName;
            m_ScenePath = aPath;
        }

        public string scenePath
        {
            get { return m_ScenePath; }
            set { m_ScenePath = value; }
        }
        public string sceneName
        {
            get { return m_SceneName; }
            set { m_SceneName = value; }
        }
        public float loadDelay
        {
            get { return m_LoadDelay; }
            set { m_LoadDelay = value; }
        }
        public Texture loadTexture
        {
            get { return m_LoadTexture; }
            set { m_LoadTexture = value; }
        }
        public bool isMenuScene
        {
            get { return m_IsMenuScene; }
            set { m_IsMenuScene = value; }
        }
    }
}