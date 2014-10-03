using UnityEngine;
using System;
using System.Collections.Generic;

namespace EndevGame
{
    [Serializable]
    public class CutsceneBezier : CutsceneAction
    {
        [SerializeField]
        private Vector3 m_ControlPointA = Vector3.zero;
        [SerializeField]
        private Vector3 m_ControlPointB = Vector3.zero;
        [SerializeField]
        private Bezier m_Bezier = new Bezier();
        [SerializeField]
        private int m_Segments = 30;

        public CutsceneBezier()
            : base()
        {

        }
        public CutsceneBezier(Vector3 aStart, Vector3 aControlPointA, Vector3 aControlPointB, Vector3 aEnd)
            : base()
        {
            startPosition = aStart;
            endPosition = aEnd;
            m_ControlPointA = aControlPointA;
            m_ControlPointB = aControlPointB;
            m_Bezier.setPoints(startPosition, aControlPointA, aControlPointB, aEnd);
        }
        public override Vector3[] getPath()
        {
            if (m_Segments <= 0)
            {
                return null;
            }
            float time = 0.0f;
            float increment = 1.0f / m_Segments;

            m_Bezier.setPoints(startPosition, controlPointA, controlPointB, endPosition);

            Vector3[] points = new Vector3[m_Segments];
            for (int i = 0; i < points.Length; i++)
            {

                points[i] = m_Bezier.getPoint(time);
                //Debug.Log("Creating point:" + points[i]);
                time += increment;
            }
            return points;
        }
        public Vector3 controlPointA
        {
            get { return m_ControlPointA; }
            set { m_ControlPointA = value; }
        }
        public Vector3 controlPointB
        {
            get { return m_ControlPointB; }
            set { m_ControlPointB = value; }
        }
        public int segments
        {
            get { return m_Segments; }
            set { m_Segments = value; }
        }
    }
}