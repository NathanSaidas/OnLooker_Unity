using UnityEngine;
using System;
using System.Collections;


namespace EndevGame
{

    [Serializable]
    public class CutsceneLookAt : Object
    {
        [SerializeField]
        private Vector3 m_Position = Vector3.zero;
        [SerializeField]
        private int m_StartFrame = 0;
        [SerializeField]
        private int m_EndFrame = 0;

        public Vector3 position { get { return m_Position; } set { m_Position = value; } }
        public int startFrame { get { return m_StartFrame; } set { m_StartFrame = value; } }
        public int endFrame { get { return m_EndFrame; } set { m_EndFrame = value; } }
    }
}