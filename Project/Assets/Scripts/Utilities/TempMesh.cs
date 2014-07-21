using UnityEngine;
using System.Collections;

public class TempMesh : MonoBehaviour {

    [SerializeField]
    float width = 1.0f;
    [SerializeField]
    float height = 1.0f;

    [SerializeField]
    Vector3 m_PositionOffset;
    [SerializeField]
    Vector3 m_RotationOffset;
    public Transform m_Camera;

    public MeshFilter m_MeshFilter = null;
	// Use this for initialization
	void Start () 
    {
        m_MeshFilter = GetComponent<MeshFilter>();
	}
	
	// Update is called once per frame
	void LateUpdate () 
    {
        if (m_Camera == null || m_MeshFilter == null)
        {
            Debug.Log("Invalid 0");
            return;
        }

        Mesh mesh = m_MeshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        int[] indices = new int[6];
        if (vertices.Length == 4)
        {
            vertices[0] = new Vector3(-width, -height, 0.0f); //top left
            vertices[1] = new Vector3(width, -height, 0.0f); //top right
            vertices[2] = new Vector3(width, height, 0.0f); //bottom right
            vertices[3] = new Vector3(-width, height, 0.0f); //bottom left
            
            indices[0] = 0;
            indices[1] = 3;
            indices[2] = 1;

            indices[3] = 1;
            indices[4] = 3;
            indices[5] = 2;

            mesh.vertices = vertices;
            mesh.triangles = indices;
        }
        else
        {
            Debug.Log("Invalid Mesh");
        }


	}

}
