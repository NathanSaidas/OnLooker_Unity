using UnityEngine;
using System;
using System.Collections;

namespace Gem
{

    [Serializable]
    public class GameScene
    {
        [SerializeField]
        private string m_ScenePath = string.Empty;
        [SerializeField]
        private string m_SceneName = string.Empty;
        [SerializeField]
        private float m_LoadDelay = 0.0f;
        [SerializeField]
        private Texture m_LoadTexture = null;

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

        public void SomeCrazyFunction()
        {

        }
    }
}