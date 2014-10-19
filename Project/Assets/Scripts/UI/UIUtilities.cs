using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct UIBoarder
{
    public float left;
    public float right;
    public float top;
    public float bottom;

    public UIBoarder(float aLeft, float aRight, float aTop, float aBottom)
    {
        left = aLeft;
        right = aRight;
        top = aTop;
        bottom = aBottom;
    }
}

public class UIUtilities : MonoBehaviour 
{
    public string m_MeshName = string.Empty;
    public float m_Width = 0.0f;
    public float m_Height = 0.0f;
    public UIBoarder m_TexBoarder = new UIBoarder();
    public UIBoarder m_UVBoarder = new UIBoarder();

    public List<Mesh> m_Meshes = new List<Mesh>();
    private MeshFilter m_MeshFilter = null;

    private Mesh m_ActiveMesh = null;

    private void Start()
    {
        m_MeshFilter = GetComponent<MeshFilter>();
    }

    private void OnGUI()
    {
        if(GUI.Button(new Rect(0.0f,0.0f,100.0f,100.0f),"Generate Mesh"))
        {
            m_Meshes.Add(generateUniformPlane(m_MeshName, m_Width, m_Height,m_TexBoarder,m_UVBoarder));
            if(m_MeshFilter != null)
            {
                m_MeshFilter.mesh = m_Meshes[m_Meshes.Count - 1];
                m_ActiveMesh = m_MeshFilter.mesh;
            }
        }
    }

    public static Mesh generateUniformPlane(string aName, float aWidth, float aHeight)
    {
        return generateUniformPlane(aName, aWidth, aHeight, new UIBoarder(0.1f, 0.9f, 0.9f, 0.1f));
    }

    public static Mesh generateUniformPlane(string aName, float aWidth, float aHeight, UIBoarder aTextureBoarder)
    {
        return generateUniformPlane(aName, aWidth, aHeight, aTextureBoarder, new UIBoarder(0.1f, 0.9f, 0.9f, 0.1f));
    }

	public static Mesh generateUniformPlane(string aName, float aWidth, float aHeight, UIBoarder aTextureBoarder, UIBoarder aUVBoarder)
    {
        //TODO: Generate Separate UV's for the center plane.
        float halfWidth = aWidth * 0.5f;
        float halfHeight = aHeight * 0.5f;
        float boarderWidth = aWidth * 0.1f;
        float boarderHeight = aHeight * 0.1f;
        UIBoarder vertBoarder = new UIBoarder(aWidth * aTextureBoarder.left,
            aWidth * (1.0f - aTextureBoarder.right),
            aHeight * (1.0f - aTextureBoarder.top),
            aHeight * aTextureBoarder.bottom);

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[16];
        Vector2[] texCoords = new Vector2[16];
        Color[] colors = new Color[16];
        int[] indices = new int[54];

        vertices[0] = new Vector3(-halfWidth, halfHeight, 0.0f);
        vertices[1] = new Vector3(-halfWidth + vertBoarder.left, halfHeight, 0.0f);
        vertices[2] = new Vector3(halfWidth - vertBoarder.right, halfHeight, 0.0f);
        vertices[3] = new Vector3(halfWidth, halfHeight, 0.0f);

        vertices[4] = new Vector3(-halfWidth, halfHeight - vertBoarder.top, 0.0f);
        vertices[5] = new Vector3(-halfWidth + vertBoarder.left, halfHeight - vertBoarder.top, 0.0f);
        vertices[6] = new Vector3(halfWidth - vertBoarder.right, halfHeight - vertBoarder.top, 0.0f);
        vertices[7] = new Vector3(halfWidth, halfHeight - vertBoarder.top, 0.0f);

        vertices[8] = new Vector3(-halfWidth, -halfHeight + vertBoarder.bottom, 0.0f);
        vertices[9] = new Vector3(-halfWidth + vertBoarder.left, -halfHeight + vertBoarder.bottom, 0.0f);
        vertices[10] = new Vector3(halfWidth - vertBoarder.right, -halfHeight + vertBoarder.bottom, 0.0f);
        vertices[11] = new Vector3(halfWidth, -halfHeight + vertBoarder.bottom, 0.0f);

        vertices[12] = new Vector3(-halfWidth, -halfHeight, 0.0f);
        vertices[13] = new Vector3(-halfWidth + vertBoarder.left, -halfHeight, 0.0f);
        vertices[14] = new Vector3(halfWidth - vertBoarder.right, -halfHeight, 0.0f);
        vertices[15] = new Vector3(halfWidth, -halfHeight, 0.0f);


        texCoords[0] = new Vector2(0.0f, 1.0f);
        texCoords[1] = new Vector2(aUVBoarder.left, 1.0f);
        texCoords[2] = new Vector2(aUVBoarder.right, 1.0f);
        texCoords[3] = new Vector2(1.0f, 1.0f);

        texCoords[4] = new Vector2(0.0f, aUVBoarder.top);
        texCoords[5] = new Vector2(aUVBoarder.left, aUVBoarder.top);
        texCoords[6] = new Vector2(aUVBoarder.right, aUVBoarder.top);
        texCoords[7] = new Vector2(1.0f, aUVBoarder.top);

        texCoords[8] = new Vector2(0.0f, aUVBoarder.bottom);
        texCoords[9] = new Vector2(aUVBoarder.left, aUVBoarder.bottom);
        texCoords[10] = new Vector2(aUVBoarder.right, aUVBoarder.bottom);
        texCoords[11] = new Vector2(1.0f, aUVBoarder.bottom);

        texCoords[12] = new Vector2(0.0f, 0.0f);
        texCoords[13] = new Vector2(aUVBoarder.left, 0.0f);
        texCoords[14] = new Vector2(aUVBoarder.right, 0.0f);
        texCoords[15] = new Vector2(1.0f, 0.0f);

        for (int i = 0; i < 16; i++)
        {
            colors[i] = Color.white;
        }

        int face = 0;
        for (int i = 0; i < 53; i += 6)
        {
            if(face == 3 || face == 7)
            {
                face++;
            }
            Debug.Log("Drawing face: " + face);
            indices[i + 0] = face + 0;
            indices[i + 1] = face + 1;
            indices[i + 2] = face + 4;

            indices[i + 3] = face + 1;
            indices[i + 4] = face + 5;
            indices[i + 5] = face + 4;
            
            face++;
        }
        mesh.name = aName;
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.uv = texCoords;
        mesh.uv1 = texCoords;
        mesh.uv2 = texCoords;
        mesh.triangles = indices;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
