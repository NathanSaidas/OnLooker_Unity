using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour 
{
    public float m_BoundEdges = 20.0f;
    public float m_CameraSpeed = 30.0f;
	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        Vector3 mouse = Input.mousePosition;

        if (mouse.x > -1.0f && mouse.x < Screen.width + 1.0f)
        {
            if (mouse.x < 0.0f + m_BoundEdges)
            {
                transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.left * m_CameraSpeed, 5.0f * Time.deltaTime);
            }
            else if (mouse.x > Screen.width - m_BoundEdges)
            {
                transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.right * m_CameraSpeed, 5.0f * Time.deltaTime);
            }
        }

        if (mouse.y > -1.0f && mouse.y < Screen.height + 1.0f)
        {
            if (mouse.y < 0.0f + m_BoundEdges)
            {
                transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.back * m_CameraSpeed, 5.0f * Time.deltaTime);
            }
            else if (mouse.y > Screen.height - m_BoundEdges)
            {
                transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.forward * m_CameraSpeed, 5.0f * Time.deltaTime);
            }
        }
        

        

	}
}
