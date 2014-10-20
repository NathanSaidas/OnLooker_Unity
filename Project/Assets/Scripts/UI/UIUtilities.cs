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
    public UIBoarder m_MeshBoarder = new UIBoarder();
    public UIBoarder m_OuterUVBoarder = new UIBoarder();
    public UIBoarder m_InnerUVBoarder = new UIBoarder();

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
            m_Meshes.Add(generateUniformPlane(m_MeshName, m_Width, m_Height,m_MeshBoarder,m_OuterUVBoarder,m_InnerUVBoarder));
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
        return generateUniformPlane(aName, aWidth, aHeight, aTextureBoarder, new UIBoarder(0.1f, 0.9f, 0.9f, 0.1f), new UIBoarder(0.1f,0.9f,0.9f,0.1f));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="aName">The name of the mesh</param>
    /// <param name="aWidth">The overall width of the plane</param>
    /// <param name="aHeight">The overall height of the plane</param>
    /// <param name="aMeshBoarder">The inner boarder percent of the plane</param>
    /// <param name="aOutterUV">The outter UV percentages</param>
    /// <param name="aInnerUV"></param>
    /// <returns></returns>
	public static Mesh generateUniformPlane(string aName, float aWidth, float aHeight, UIBoarder aMeshBoarder, UIBoarder aOutterUV, UIBoarder aInnerUV)
    {
        ///Outer edges
        UIBoarder meshInnerBoarder = new UIBoarder(-aWidth * 0.5f, aWidth * 0.5f, aHeight * 0.5f, -aHeight * 0.5f);
        ///Inner edges
        UIBoarder meshOutterBoarder = new UIBoarder(
            meshInnerBoarder.left + aWidth * aMeshBoarder.left, 
            meshInnerBoarder.right - aWidth * (1.0f - aMeshBoarder.right),
            meshInnerBoarder.top - aHeight * (1.0f - aMeshBoarder.top),
            meshInnerBoarder.bottom + aHeight * aMeshBoarder.bottom);

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[36];
        Vector2[] texCoords = new Vector2[36];
        Color[] colors = new Color[36];
        
        int[] indices = new int[54];


        //top left face
        vertices[0] = new Vector3(meshInnerBoarder.left, meshInnerBoarder.top, 0.0f);
        vertices[1] = new Vector3(meshOutterBoarder.left, meshInnerBoarder.top, 0.0f);
        vertices[2] = new Vector3(meshOutterBoarder.left, meshOutterBoarder.top, 0.0f);
        vertices[3] = new Vector3(meshInnerBoarder.left, meshOutterBoarder.top, 0.0f);

        //top middle face
        vertices[4] = new Vector3(meshOutterBoarder.left, meshInnerBoarder.top, 0.0f);
        vertices[5] = new Vector3(meshOutterBoarder.right, meshInnerBoarder.top, 0.0f);
        vertices[6] = new Vector3(meshOutterBoarder.right, meshOutterBoarder.top, 0.0f);
        vertices[7] = new Vector3(meshOutterBoarder.left, meshOutterBoarder.top, 0.0f);
        
        //top right face
        vertices[8] = new Vector3(meshOutterBoarder.right, meshInnerBoarder.top, 0.0f);
        vertices[9] = new Vector3(meshInnerBoarder.right, meshInnerBoarder.top, 0.0f);
        vertices[10] = new Vector3(meshInnerBoarder.right, meshOutterBoarder.top, 0.0f);
        vertices[11] = new Vector3(meshOutterBoarder.right, meshOutterBoarder.top, 0.0f);

        //middle left face
        vertices[12] = new Vector3(meshInnerBoarder.left, meshOutterBoarder.top, 0.0f);
        vertices[13] = new Vector3(meshOutterBoarder.left, meshOutterBoarder.top, 0.0f);
        vertices[14] = new Vector3(meshOutterBoarder.left, meshOutterBoarder.bottom, 0.0f);
        vertices[15] = new Vector3(meshInnerBoarder.left, meshOutterBoarder.bottom, 0.0f);

        //middle middle face
        vertices[16] = new Vector3(meshOutterBoarder.left, meshOutterBoarder.top, 0.0f);
        vertices[17] = new Vector3(meshOutterBoarder.right, meshOutterBoarder.top, 0.0f);
        vertices[18] = new Vector3(meshOutterBoarder.right, meshOutterBoarder.bottom, 0.0f);
        vertices[19] = new Vector3(meshOutterBoarder.left, meshOutterBoarder.bottom, 0.0f);

        //middle right face
        vertices[20] = new Vector3(meshOutterBoarder.right, meshOutterBoarder.top, 0.0f);
        vertices[21] = new Vector3(meshInnerBoarder.right, meshOutterBoarder.top, 0.0f);
        vertices[22] = new Vector3(meshInnerBoarder.right, meshOutterBoarder.bottom, 0.0f);
        vertices[23] = new Vector3(meshOutterBoarder.right, meshOutterBoarder.bottom, 0.0f);

        //bottom left face
        vertices[24] = new Vector3(meshInnerBoarder.left, meshOutterBoarder.bottom, 0.0f);
        vertices[25] = new Vector3(meshOutterBoarder.left, meshOutterBoarder.bottom, 0.0f);
        vertices[26] = new Vector3(meshOutterBoarder.left, meshInnerBoarder.bottom, 0.0f);
        vertices[27] = new Vector3(meshInnerBoarder.left, meshInnerBoarder.bottom, 0.0f);

        //bottom middle face
        vertices[28] = new Vector3(meshOutterBoarder.left, meshOutterBoarder.bottom, 0.0f);
        vertices[29] = new Vector3(meshOutterBoarder.right, meshOutterBoarder.bottom, 0.0f);
        vertices[30] = new Vector3(meshOutterBoarder.right, meshInnerBoarder.bottom, 0.0f);
        vertices[31] = new Vector3(meshOutterBoarder.left, meshInnerBoarder.bottom, 0.0f);

        //bottom right face
        vertices[32] = new Vector3(meshOutterBoarder.right, meshOutterBoarder.bottom, 0.0f);
        vertices[33] = new Vector3(meshInnerBoarder.right, meshOutterBoarder.bottom, 0.0f);
        vertices[34] = new Vector3(meshInnerBoarder.right, meshInnerBoarder.bottom, 0.0f);
        vertices[35] = new Vector3(meshOutterBoarder.right, meshInnerBoarder.bottom, 0.0f);

        //Top left UV
        texCoords[0] = new Vector2(0.0f, 1.0f);
        texCoords[1] = new Vector2(aOutterUV.left, 1.0f);
        texCoords[2] = new Vector2(aOutterUV.left, aOutterUV.top);
        texCoords[3] = new Vector2(0.0f, aOutterUV.top);

        //Top Center UV
        texCoords[4] = new Vector2(aOutterUV.left, 1.0f);
        texCoords[5] = new Vector2(aOutterUV.right, 1.0f);
        texCoords[6] = new Vector2(aOutterUV.right, aOutterUV.top);
        texCoords[7] = new Vector2(aOutterUV.left, aOutterUV.top);

        //Top Right UV
        texCoords[8] = new Vector2(aOutterUV.right, 1.0f);
        texCoords[9] = new Vector2(1.0f, 1.0f);
        texCoords[10] = new Vector2(1.0f, aOutterUV.top);
        texCoords[11] = new Vector2(aOutterUV.right, aOutterUV.top);


        //Middle Left UV
        texCoords[12] = new Vector2(0.0f,aOutterUV.top);
        texCoords[13] = new Vector2(aOutterUV.left, aOutterUV.top);
        texCoords[14] = new Vector2(aOutterUV.left, aOutterUV.bottom);
        texCoords[15] = new Vector2(0.0f, aOutterUV.bottom);

        //Middle Center UV
        texCoords[16] = new Vector2(aInnerUV.left, aInnerUV.top);
        texCoords[17] = new Vector2(aInnerUV.right, aInnerUV.top);
        texCoords[18] = new Vector2(aInnerUV.right, aInnerUV.bottom);
        texCoords[19] = new Vector2(aInnerUV.left, aInnerUV.bottom);

        //Middle Right UV
        texCoords[20] = new Vector2(aOutterUV.right, aOutterUV.top);
        texCoords[21] = new Vector2(1.0f, aOutterUV.top);
        texCoords[22] = new Vector2(1.0f, aOutterUV.bottom);
        texCoords[23] = new Vector2(aOutterUV.right, aOutterUV.bottom);

        //Bottom Left UV
        texCoords[24] = new Vector2(0.0f, aOutterUV.bottom);
        texCoords[25] = new Vector2(aOutterUV.left, aOutterUV.bottom);
        texCoords[26] = new Vector2(aOutterUV.left, 0.0f);
        texCoords[27] = new Vector2(0.0f, 0.0f);

        //Bottom Center UV
        texCoords[28] = new Vector2(aOutterUV.left, aOutterUV.bottom);
        texCoords[29] = new Vector2(aOutterUV.right, aOutterUV.bottom);
        texCoords[30] = new Vector2(aOutterUV.right, 0.0f);
        texCoords[31] = new Vector2(aOutterUV.left, 0.0f);

        //Bottom Right UV
        texCoords[32] = new Vector2(aOutterUV.right, aOutterUV.bottom);
        texCoords[33] = new Vector2(1.0f, aOutterUV.bottom);
        texCoords[34] = new Vector2(1.0f, 0.0f);
        texCoords[35] = new Vector2(aOutterUV.right, 0.0f);

        for (int i = 0; i < 36; i++)
        {
            colors[i] = Color.white;
        }

        int face = 0;
        for (int i = 0; i < 53; i += 6)
        {
            indices[i + 0] = face + 0;
            indices[i + 1] = face + 1;
            indices[i + 2] = face + 3;

            indices[i + 3] = face + 1;
            indices[i + 4] = face + 2;
            indices[i + 5] = face + 3;

            face += 4;
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
