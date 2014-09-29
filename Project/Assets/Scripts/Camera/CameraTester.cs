using UnityEngine;
using System.Collections;

public class CameraTester : MonoBehaviour
{
    [SerializeField]
    private Transform m_PositionA = null;
    [SerializeField]
    private Transform m_PositionB = null;

    private Transform m_Target = null;
	// Use this for initialization
	void Start () 
    {
        m_Target = m_PositionA;
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            swap();
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            CameraManager.instance.transitionToFirstPerson(m_Target, CameraMode.LERP, 10.0f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CameraManager.instance.transitionToShoulder(m_Target, CameraMode.LERP, 10.0f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CameraManager.instance.transitionToOrbit(m_Target, CameraMode.LERP, 10.0f);
        }
        if(Input.GetKeyDown(KeyCode.Alpha5))
        {
            CameraManager.instance.disable();
        }
	}

    void swap()
    {
        if(m_Target == m_PositionA)
        {
            m_Target = m_PositionB;
        }
        else
        {
            m_Target = m_PositionA;
        }
    }
}
