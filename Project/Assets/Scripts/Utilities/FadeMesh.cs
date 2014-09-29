using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
public class FadeMesh : MonoBehaviour 
{
    MeshRenderer m_MeshRenderer = null;
    Material m_Material = null;

    [SerializeField]
    float m_Alpha = 0.0f;
	// Use this for initialization
	void Start () 
    {
        init();
	}
    void init()
    {
        if (m_Material != null)
        {
            return;
        }

        m_MeshRenderer = GetComponent<MeshRenderer>();
        m_Material = new Material(m_MeshRenderer.sharedMaterial);
        m_Material.SetColor("_Color", new Color(0.0f, 0.0f, 0.0f, 0.0f));
        m_MeshRenderer.material = m_Material;
    }
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public float alpha
    {
        set { if (m_Material == null)init(); m_Material.SetColor("_Color", new Color(0.0f, 0.0f, 0.0f, value)); m_Alpha = value; }
    }
}
