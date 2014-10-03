using UnityEngine;
using System;
using System.Collections;

namespace EndevGame
{

    [Serializable]
    public abstract class CutsceneAction : EndevGame.Object
    {
        //The start position of the cutscene action
        [SerializeField]
        private Vector3 m_StartPosition = Vector3.zero;
        //The end position of the cutscene action
        [SerializeField]
        private Vector3 m_EndPosition = Vector3.zero;


        public CutsceneAction()
        {

        }


        //Returns the points to get from start to end.
        public abstract Vector3[] getPath();

        public Vector3 startPosition
        {
            get { return m_StartPosition; }
            set { m_StartPosition = value; }
        }
        public Vector3 endPosition
        {
            get { return m_EndPosition; }
            set { m_EndPosition = value; }
        }



    }
}