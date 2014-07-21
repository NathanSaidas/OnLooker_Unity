using UnityEngine;
using System.Collections;

public class TempBounds : MonoBehaviour {

    public TextMesh m_Text = null;
    public BoxCollider m_BoxCollider = null;

    public float TEXT_PADDING_X = 1.0f;
    public float TEXT_PADDING_Y = 1.0f;
	// Use this for initialization
	void Start () {
        m_Text = GetComponent<TextMesh>();
        m_BoxCollider = GetComponent<BoxCollider>();
	}
	
	// Update is called once per frame
	void Update () {
        if (m_Text != null)
        {

        }
        if (m_BoxCollider != null)
        {
            SizeCollider();
            //m_BoxCollider.size = m_Text.renderer.bounds.size;
        }
	}

    void SizeCollider()
    {
        Renderer render = m_Text.renderer;
       // m_BoxCollider.center = new Vector3(render.bounds.extents.x - render.bounds.size.x / 2, render.bounds.extents.y - render.bounds.size.y / 2, transform.position.z);
        m_BoxCollider.size = new Vector3(render.bounds.size.x + TEXT_PADDING_X, render.bounds.size.y + TEXT_PADDING_Y, 1);
    }

    void OnDrawGizmos()
    {

    }
}
