using UnityEngine;
using System.Collections;

namespace EndevGame
{


    public class LoadScreen : MonoBehaviour
    {
        /// <summary>
        /// This is the texture to display in the background of the loading screen
        /// </summary>
        [SerializeField]
        Texture m_TextureToDisplay = null;


        /// <summary>
        /// This is a reference to the loading screen text in the back
        /// </summary>
        [SerializeField]
        TextMesh m_LoadingText = null;

        MeshRenderer m_MeshRenderer = null;
        // Use this for initialization
        void Start()
        {
            gameObject.SetActive(false);
        }
        void OnEnable()
        {
            foreach(Transform child in transform)
            {
                //Search for child Loading Background to get the mesh renderer for the background texture
                if(child.name == "Loading Background")
                {
                    m_MeshRenderer = child.GetComponent<MeshRenderer>();
                }
                if(child.name == "Loading Percent")
                {
                    m_LoadingText = child.GetComponent<TextMesh>();
                }
            }

            if(m_MeshRenderer == null)
            {
                Debug.LogError("Missing a gameobject with the name \'Loading Background\' and a \'MeshRenderer\' component.");
                gameObject.SetActive(false);
                return;
            }
            if(m_LoadingText == null)
            {
                Debug.LogError("Missing a gameobject with the name \'Loading Text\' and a \'TextMesh\' component.");
                gameObject.SetActive(false);
                return;
            }
            Material material = new Material(m_MeshRenderer.sharedMaterial);
            material.SetTexture("_MainTex", m_TextureToDisplay);
            m_MeshRenderer.material = material;
        }

        /// <summary>
        /// Updates the percentage of time.
        /// </summary>
        void Update()
        {
            if (m_LoadingText != null)
            {

                float time = 0.0f; // GameManager.currentLoadTimePercent * 100;
                string text = time.ToString();

                if (time >= 100.0f)
                {
                    text = text.Remove(3);
                    m_LoadingText.text = text;
                }
                else if (time > 0.0f)
                {
                    text = text.Remove(2);
                    m_LoadingText.text = text;
                }
            }
        }

        public void setTexture(Texture aTexture)
        {
            if (m_MeshRenderer == null)
            {
                Debug.LogError("Missing a gameobject with the name \'Loading Background\' and a \'MeshRenderer\' component.");
                return;
            }
            if (m_LoadingText == null)
            {
                Debug.LogError("Missing a gameobject with the name \'Loading Text\' and a \'TextMesh\' component.");
                return;
            }
            m_TextureToDisplay = aTexture;
            m_MeshRenderer.material.SetTexture("_MainTex", m_TextureToDisplay);
        }
    }
}