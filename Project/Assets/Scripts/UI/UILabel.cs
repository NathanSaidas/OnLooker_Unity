using UnityEngine;
using System.Collections;

namespace Gem
{
    [RequireComponent(typeof(MeshRenderer),typeof(TextMesh))]
    public class UILabel : MonoBehaviour
    {

        private MeshRenderer m_MeshRenderer = null;
        private TextMesh m_TextMesh = null;


        [SerializeField]
        private string m_Text = "Text";
        [SerializeField]
        private int m_FontSize = 100;
        [SerializeField]
        private Font m_Font = null;
        [SerializeField]
        private Color m_Color = Color.white;
        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Gets called when text has changed.
        /// </summary>
        void OnTextChanged()
        {

        }
        /// <summary>
        /// Updates the box collider bounds
        /// </summary>
        void UpdateBounds()
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
        public Color color
        {
            get { return m_Color; }
            set { m_Color = value; if (m_TextMesh != null) { m_TextMesh.color = value; } }
        }
    }
}