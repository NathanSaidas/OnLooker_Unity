using UnityEngine;
using System.Collections.Generic;


    //This class was written by Loran on the Unity Forums.
    //I made slight modifications
    //http://forum.unity3d.com/threads/bezier-curve.5082/

    /// <summary>
    /// 
    /// </summary>
public class Bezier : NativeObject
{
    [SerializeField]
    private Vector3[] m_Points = new Vector3[4];
    [SerializeField]
    private Vector3[] m_BPoints = new Vector3[4];
    [SerializeField]
    private Vector3 m_PositionA;
    [SerializeField]
    private Vector3 m_PositionB;
    [SerializeField]
    private Vector3 m_PositionC;

    public Bezier()
    {
        m_Points[0] = Vector3.zero;
        m_Points[1] = Vector3.zero;
        m_Points[2] = Vector3.zero;
        m_Points[3] = Vector3.zero;
    }

    public Bezier(Vector3 aPoint1, Vector3 aPoint2, Vector3 aPoint3, Vector3 aPoint4)
    {
        m_Points[0] = aPoint1;
        m_Points[1] = aPoint2;
        m_Points[2] = aPoint3;
        m_Points[3] = aPoint4;
    }
    public void setPoints(Vector3 aPoint1, Vector3 aPoint2, Vector3 aPoint3, Vector3 aPoint4)
    {
        m_Points[0] = aPoint1;
        m_Points[1] = aPoint2;
        m_Points[2] = aPoint3;
        m_Points[3] = aPoint4;
    }

    public Vector3 getPoint(float aTime)
    {
        checkConstant();
        float time2 = aTime * aTime;
        float time3 = aTime * aTime * aTime;

        float x = m_PositionA.x * time3 + m_PositionB.x * time2 + m_PositionC.x * aTime + m_Points[0].x;
        float y = m_PositionA.y * time3 + m_PositionB.y * time2 + m_PositionC.y * aTime + m_Points[0].y;
        float z = m_PositionA.z * time3 + m_PositionB.z * time2 + m_PositionC.z * aTime + m_Points[0].z;

        return new Vector3(x, y, z);
    }

    private void setConstant()
    {
        m_PositionC.x = 3 * ((m_Points[0].x + m_Points[1].x) - m_Points[0].x);
        m_PositionB.x = 3 * ((m_Points[3].x + m_Points[2].x) - (m_Points[0].x + m_Points[1].x)) - m_PositionC.x;
        m_PositionA.x = m_Points[3].x - m_Points[0].x - m_PositionC.x - m_PositionB.x;

        m_PositionC.y = 3 * ((m_Points[0].y + m_Points[1].y) - m_Points[0].y);
        m_PositionB.y = 3 * ((m_Points[3].y + m_Points[2].y) - (m_Points[0].y + m_Points[1].y)) - m_PositionC.y;
        m_PositionA.y = m_Points[3].y - m_Points[0].y - m_PositionC.y - m_PositionB.y;

        m_PositionC.z = 3 * ((m_Points[0].z + m_Points[1].z) - m_Points[0].z);
        m_PositionB.z = 3 * ((m_Points[3].z + m_Points[2].z) - (m_Points[0].z + m_Points[1].z)) - m_PositionC.z;
        m_PositionA.z = m_Points[3].z - m_Points[0].z - m_PositionC.z - m_PositionB.z;
    }

    private void checkConstant()
    {
        if (m_Points[0] != m_BPoints[0] || m_Points[1] != m_BPoints[1] || m_Points[2] != m_BPoints[2] || m_Points[3] != m_BPoints[3])
        {
            setConstant();
            m_BPoints[0] = m_Points[0];
            m_BPoints[1] = m_Points[1];
            m_BPoints[2] = m_Points[2];
            m_BPoints[3] = m_Points[3];
        }
    }
}