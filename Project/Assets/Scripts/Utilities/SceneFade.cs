using UnityEngine;
using System.Collections;


namespace EndevGame
{

    [RequireComponent(typeof(MeshRenderer))]
    public class SceneFade : MonoBehaviour
    {

        private MeshRenderer m_MeshRenderer = null;
        private Material m_Material;

        [SerializeField]
        private Texture m_Texture = null;
        private Color m_Color = Color.white;
        [SerializeField]
        private float m_Time = 5.0f;
        private float m_StartTime = 5.0f;
        // Use this for initialization
        void Start()
        {
            m_MeshRenderer = GetComponent<MeshRenderer>();
            m_Material = new Material(m_MeshRenderer.sharedMaterial);

            m_Material.SetTexture("_MainTex", m_Texture);
            m_Material.SetColor("_Color", m_Color);
            m_MeshRenderer.material = m_Material;

            m_StartTime = m_Time;
        }

        // Update is called once per frame
        void Update()
        {
            if (m_Time > 0.0f)
            {
                m_Time -= Time.deltaTime;
            }
            else
            {
                m_Time = 0.0f;
                //GameManager.loadScene("main_menu_scene");
            }

            float percent = m_Time / m_StartTime;
            m_Color.a = percent;
            m_Color.r = percent;
            m_Color.g = percent;
            m_Color.b = percent;
            m_Material.SetColor("_Color", m_Color);

        }
    }
}