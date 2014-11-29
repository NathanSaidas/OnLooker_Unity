using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

#region CHANGE LOG
/* November,13,2014 - Nathan Hanlan, Added support for updating components. Added FontTexture to pass to the shader.
 * November,14,2014 - Nathan Hanlan, UILabel now executes in editor mode and releases material resource
 * November,14,2014 - Nathan Hanlan, Fixed issue where material created didnt always contain the appropriate shader.
 * 
 */
#endregion

namespace Gem
{
    public delegate void TextCallback(string aText);
    /// <summary>
    /// This class renders text out on the screen using TextMesh and MeshRenderer components.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer),typeof(TextMesh))]
    [ExecuteInEditMode]
    public class UILabel : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("CONTEXT/UILabel/Create Material")]
        private static void CreateMaterial(MenuCommand aCommand)
        {
            UILabel label = aCommand.context as UILabel;
            if(label != null)
            {
                label.m_MeshRenderer = label.GetComponent<MeshRenderer>();
                label.m_Material = new Material(Shader.Find(UIUtilities.SHADER_TEXT));
                label.m_MeshRenderer.material = label.m_Material;
            }
        }

        [MenuItem("CONTEXT/UILabel/Update Components")]
        private static void UpdateComponents(MenuCommand aCommand)
        {
            UILabel label = aCommand.context as UILabel;
            if (label != null)
            {
                label.UpdateComponents();
            }
        }
#endif

        private MeshRenderer m_MeshRenderer = null;
        private TextMesh m_TextMesh = null;

#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("The text to display in the image.")]
#endif
        [SerializeField]
        private string m_Text = "Text";
#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("The size of the font.")]
#endif
        [SerializeField]
        private int m_FontSize = 100;
#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("The font to render.")]
#endif
        [SerializeField]
        private Font m_Font = null;
#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("The texture of the font provided. (This is usually rooted within the font asset heirarchy)")]
#endif
        [SerializeField]
        private Texture m_FontTexture = null;
#if UNITY_EDITOR && (UNITY_4_5 || UNITY_4_6)
        [Tooltip("The color offset of the rendered font.")]
#endif
        [SerializeField]
        private Color m_Color = Color.white;

        private TextCallback m_TextChanged = null;


        /// <summary>
        /// Hidden material
        /// </summary>
        [HideInInspector]
        [SerializeField]
        private Material m_Material = null;

        private void Start()
        {
            m_MeshRenderer = GetComponent<MeshRenderer>();
            m_TextMesh = GetComponent<TextMesh>();
        }

        void OnDestroy()
        {
            if(Application.isPlaying)
            {
                Destroy(m_Material);
            }
            else
            {
                DestroyImmediate(m_Material);
            }    
        }
        /// <summary>
        /// Gets called when text has changed.
        /// </summary>
        void OnTextChanged()
        {
            if(textChanged != null)
            {
                textChanged.Invoke(m_Text);
            }
        }
        /// <summary>
        /// Updates the box collider bounds
        /// </summary>
        public void UpdateBounds()
        {
            Quaternion rotation = transform.rotation;
            transform.rotation = Quaternion.identity;
            BoxCollider collider = GetComponent<BoxCollider>();
            if (collider != null)
            {
                collider.center = transform.InverseTransformPoint(renderer.bounds.center);
                collider.size = renderer.bounds.size;
            }
            transform.rotation = rotation;
        }
        public void UpdateBounds(BoxCollider aCollider, float aMinWidth)
        {
            if(aCollider == null)
            {
                return;
            }
            Quaternion rotation = transform.rotation;
            transform.rotation = Quaternion.identity;
            aCollider.center = transform.InverseTransformPoint(renderer.bounds.center);
            Vector3 size = renderer.bounds.size;
            size.x = Mathf.Max(aMinWidth, size.x);
            aCollider.size = size;
            transform.rotation = rotation;
        }
        /// <summary>
        /// Updates all components
        /// </summary>
        public void UpdateComponents()
        {
            if(m_MeshRenderer == null)
            {
                m_MeshRenderer = GetComponent<MeshRenderer>();
            }
            if(m_TextMesh == null)
            {
                m_TextMesh = GetComponent<TextMesh>();
            }
            if (m_TextMesh != null)
            {
                m_TextMesh.characterSize = 0.1f;
                m_TextMesh.anchor = TextAnchor.MiddleCenter;
                m_TextMesh.alignment = TextAlignment.Center;

                m_TextMesh.text = m_Text;
                m_TextMesh.fontSize = m_FontSize;
                m_TextMesh.font = m_Font;
            }
            if(m_MeshRenderer != null)
            {
                if(m_Material == null && m_MeshRenderer.material != null)
                {
                    m_Material = m_MeshRenderer.sharedMaterial;
                }
                if(m_Material == null || m_MeshRenderer.material == null)
                {
                    m_Material = new Material(Shader.Find(UIUtilities.SHADER_TEXT));
                    m_MeshRenderer.material = m_Material;
                }
                else
                {
                    if(!(UIUtilities.IsUIShader(m_Material.shader.name)))
                    {
                        m_Material.shader = Shader.Find(UIUtilities.SHADER_TEXT);
                    }
                }
                m_Material.SetTexture(UIUtilities.SHADER_TEXTURE, m_FontTexture);
                m_Material.SetColor(UIUtilities.SHADER_COLOR, m_Color);
            }

            UpdateBounds();
        }

        public MeshRenderer meshRenderer
        {
            get { return m_MeshRenderer; }
        }
        public TextMesh textMesh
        {
            get { return m_TextMesh; }
        }

        /// <summary>
        /// The text to display.
        /// </summary>
        public string text
        {
            get { return m_Text; }
            set
            {
                bool updateBounds = m_Text != value;
                m_Text = value;
                if(m_TextMesh != null)
                {
                    m_TextMesh.text = value;
                }
                if(updateBounds)
                {
                    OnTextChanged();
                    UpdateBounds();
                }
            }
        }
        /// <summary>
        /// The size of the font.
        /// </summary>
        public int fontSize
        {
            get { return m_FontSize; }
            set
            {
                bool updateBounds = m_FontSize != value;
                m_FontSize = value;
                if (m_TextMesh != null)
                {
                    m_TextMesh.fontSize = value;
                }
                if (updateBounds)
                {
                    UpdateBounds();
                }
            }
        }
        /// <summary>
        /// The font to render in the world.
        /// </summary>
        public Font font
        {
            get { return m_Font; }
            set
            {
                bool updateBounds = (m_Font != value && value != null);
                m_Font = value;
                if(m_TextMesh != null)
                {
                    m_TextMesh.font = value;
                }
                if(updateBounds)
                {
                    UpdateBounds();
                }
            }
        }
        /// <summary>
        /// The texture atlas for the font that gets passed into the shader.
        /// </summary>
        public Texture fontTexture
        {
            get { return m_FontTexture; }
            set { m_FontTexture = value; if (m_Material != null) { m_Material.SetTexture(UIUtilities.SHADER_TEXTURE,value); } }
        }

        /// <summary>
        /// The color offset of the UILabel
        /// </summary>
        public Color color
        {
            get { return m_Color; }
            set { m_Color = value; m_Color.a = 0.0f; if (m_Material != null) { m_Material.SetColor(UIUtilities.SHADER_COLOR, m_Color); } }
        }

        public Material material
        {
            get { return m_Material; }
        }

        public TextCallback textChanged
        {
            get { return m_TextChanged; }
            set { m_TextChanged = value; }
        }
    }
}