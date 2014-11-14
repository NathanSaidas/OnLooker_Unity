using UnityEngine;
using System.Collections;

#region CHANGE LOG
/* November,13,2014 - Nathan Hanlan, Added support for updating components. Added FontTexture to pass to the shader.
 * 
 */
#endregion

namespace Gem
{

    /// <summary>
    /// This class renders text out on the screen using TextMesh and MeshRenderer components.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer),typeof(TextMesh))]
    public class UILabel : MonoBehaviour
    {

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


        /// <summary>
        /// Hidden material
        /// </summary>
        [HideInInspector]
        [SerializeField]
        private Material m_Material = null;

        /// <summary>
        /// Gets called when text has changed.
        /// </summary>
        void OnTextChanged()
        {

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
                    m_Material = m_MeshRenderer.material;
                }
                if(m_Material == null || m_MeshRenderer.material == null)
                {
                    m_Material = new Material(Shader.Find(UIUtilities.SHADER_TEXT));
                    m_MeshRenderer.material = m_Material;
                }
                m_Material.SetTexture(UIUtilities.SHADER_TEXTURE, m_FontTexture);
                m_Material.SetColor(UIUtilities.SHADER_COLOR, m_Color);
            }

            UpdateBounds();
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
    }
}