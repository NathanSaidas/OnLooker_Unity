using UnityEngine;
using System.Collections;
#region CHANGE LOG
/* October,27,2014 - Nathan Hanlan, Added and implemented Class UIImage
 * 
 */
#endregion

namespace Gem
{

    /// <summary>
    /// This class generates a mesh and renders a texture onto it using the material and shader given.
    /// </summary>
    [ExecuteInEditMode()]
    [RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
    public class UIImage : MonoBehaviour
    {
        /// <summary>
        /// The MeshFilter Component Required by the UIImage for rendering
        /// </summary>
        private MeshFilter m_MeshFilter = null;
        /// <summary>
        /// The MeshRenderer Component Require by the UIImage for rendering
        /// </summary>
        private MeshRenderer m_MeshRenderer = null;

        /// <summary>
        /// The name of the Mesh
        /// </summary>
        [SerializeField]
        private string m_MeshName = string.Empty;
        /// <summary>
        /// The width of the Mesh
        /// </summary>
        [SerializeField]
        private float m_Width = 1.0f;
        /// <summary>
        /// The height of the mesh
        /// </summary>
        [SerializeField]
        private float m_Height = 1.0f;
        /// <summary>
        /// The mesh vertex boarder
        /// </summary>
        [SerializeField]
        private UIBoarder m_MeshBoarder = new UIBoarder(0.1f,0.9f,0.9f,0.1f);
        /// <summary>
        /// The outer UV boarder
        /// </summary>
        [SerializeField]
        private UIBoarder m_OuterUVBoarder = new UIBoarder(0.1f, 0.9f, 0.9f, 0.1f);
        /// <summary>
        /// The inner UV boarder
        /// </summary>
        [SerializeField]
        private UIBoarder m_InnerUVBoarder = new UIBoarder(0.1f, 0.9f, 0.9f, 0.1f);

        /// <summary>
        /// The procedurally generated mesh
        /// </summary>
        [SerializeField]
        private Mesh m_Mesh = new Mesh();
        /// <summary>
        /// The default material copied.
        /// </summary>
        [SerializeField]
        private Material m_Material = null;
        /// <summary>
        /// The texture to render
        /// </summary>
        [SerializeField]
        private Texture m_Texture = null;
        /// <summary>
        /// The shader to use, Accepts only UI Shaders will default to Unlit UI if invalid shader given
        /// </summary>
        [SerializeField]
        private Shader m_Shader = null;
        /// <summary>
        /// A color tint property to apply to the material.
        /// </summary>
        [SerializeField]
        private Color m_Color = Color.white;

        /// <summary>
        /// Initializes the component at runtime
        /// </summary>
        private void Start()
        {
            m_MeshFilter = GetComponent<MeshFilter>();
            m_MeshRenderer = GetComponent<MeshRenderer>();
            //if(m_MeshRenderer != null && m_Material != null)
            //{
            //    //m_Material = new Material(m_Material);
            //    m_MeshRenderer.material = m_Material;
            //    if(m_Shader != null)
            //    {
            //         if(!UIUtilities.IsUIShader(m_Shader.name))
            //         {
            //             m_Shader = UIManager.defaultShader;
            //         }
            //         m_Material.shader = m_Shader;
            //    }
            //}
        }
        private void OnDestroy()
        {
            if(m_Material != null)
            {
                if(!Application.isPlaying)
                {
                    DestroyImmediate(m_Material);
                }
                else
                {
                    Destroy(m_Material);
                }
            }
        }
        public void GenerateMaterial()
        {
            m_Material = new Material(m_Shader);
            m_MeshRenderer = GetComponent<MeshRenderer>();
            m_MeshRenderer.material = m_Material;
            if(m_Shader == null)
            {
                m_Shader = UIManager.defaultShader;
            }
            else
            {
                if(!UIUtilities.IsUIShader(m_Shader.name))
                {
                    m_Shader = UIManager.defaultShader;
                }
            }
            m_Material.shader = m_Shader;
        }
        /// <summary>
        /// Generates a mesh to display on screen. Use this at edit time.
        /// </summary>
        public void GenerateMesh()
        {
            if(m_MeshFilter == null)
            {
                m_MeshFilter = GetComponent<MeshFilter>();
            }
            if (m_MeshFilter != null)
            {
                m_Mesh.name = m_MeshName;
                m_MeshFilter.mesh = m_Mesh;
                m_Mesh = UIUtilities.GenerateUniformPlane(m_Mesh, m_Width, m_Height, m_MeshBoarder, m_OuterUVBoarder, m_InnerUVBoarder);
            }
        }
        /// <summary>
        /// Updates the texture in shader
        /// </summary>
        public void SetTexture()
        {
            m_Material.SetTexture("_Texture", m_Texture);
        }
        /// <summary>
        /// Updates the color in the shader.
        /// </summary>
        public void SetColor()
        {
            m_Material.SetColor("_Color", m_Color);
        }
        /// <summary>
        /// The MeshFilter Component Required by the UIImage for rendering
        /// </summary>
        public MeshFilter meshFilter
        {
            get { return m_MeshFilter; }
        }
        /// <summary>
        /// The MeshRenderer Component Require by the UIImage for rendering
        /// </summary>
        public MeshRenderer meshRenderer
        {
            get { return m_MeshRenderer; }
        }
        /// <summary>
        /// The name of the Mesh
        /// </summary>
        public string meshName
        {
            get { return m_MeshName; }
            set { m_MeshName = value; }
        }
        /// <summary>
        /// The width of the Mesh
        /// </summary>
        public float width
        {
            get { return m_Width; }
            set { m_Width = value; }
        }
        /// <summary>
        /// The height of the mesh
        /// </summary>
        public float height
        {
            get { return m_Height; }
            set { m_Height = value; }
        }
        /// <summary>
        /// The mesh vertex boarder
        /// </summary>
        public UIBoarder meshBoarder
        {
            get { return m_MeshBoarder; }
            set { m_MeshBoarder = value; }
        }
        /// <summary>
        /// The outer UV boarder
        /// </summary>
        public UIBoarder outerUVBoarder
        {
            get { return m_OuterUVBoarder; }
            set { m_OuterUVBoarder = value; }
        }
        /// <summary>
        /// The inner UV boarder
        /// </summary>
        public UIBoarder innerUVBoarder
        {
            get { return m_InnerUVBoarder; }
            set { m_InnerUVBoarder = value; }
        }
        /// <summary>
        /// The procedurally generated mesh
        /// </summary>
        public Mesh mesh
        {
            get { return m_Mesh; }
            set { m_Mesh = value; }
        }
        /// <summary>
        /// The default material copied.
        /// </summary>
        public Material material
        {
            get { return m_Material; }
        }
        /// <summary>
        /// The texture to render
        /// </summary>
        public Texture texture
        {
            get { return m_Texture; }
            set { bool change = m_Texture != value; m_Texture = value; if (change) { SetTexture(); } }
        }
        /// <summary>
        /// The shader to use, Accepts only UI Shaders will default to Unlit UI if invalid shader given
        /// </summary>
        public Shader shader
        {
            get { return m_Shader; }
            set { m_Shader = value; }
        }
        /// <summary>
        /// A color tint property to apply to the material.
        /// </summary>
        public Color color
        {
            get { return m_Color;}
            set { m_Color = value; }
        }
    }

}