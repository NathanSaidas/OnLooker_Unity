using UnityEngine;
using System.Collections;

namespace Gem
{

    public class GravityGun : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem m_ParticleSystem = null;
        [SerializeField]
        private int m_Count = 15;
        [SerializeField]
        private float m_MaxLifeTime = 1.5f;
        [SerializeField]
        private float m_MinLifeTime = 0.45f;
        [SerializeField]
        private float m_RangeSpeed = 10.0f;
        [SerializeField]
        private float m_LifeTime = 1.0f;

        [SerializeField]
        private Transform m_Target = null;

        [SerializeField]
        private float m_Range = 3.0f;
        [SerializeField]
        private float m_ClampSpeed = 3.0f;

        void Start()
        {
            m_ParticleSystem.Stop();
            enabled = false;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (Input.GetMouseButton(0))
            {
                m_ParticleSystem.transform.rotation = UIManager.cameraWorld.transform.rotation;
                m_ParticleSystem.startLifetime = m_LifeTime;
                m_ParticleSystem.Emit(m_Count);

                if(m_Target != null)
                {
                    if(m_Target.rigidbody != null)
                    {
                        m_Target.rigidbody.useGravity = false;
                    }
                    m_Target.position = Vector3.Lerp(m_Target.position, transform.position + UIManager.cameraWorld.transform.forward * m_Range, Time.deltaTime * m_ClampSpeed);
                }
                else
                {
                    Ray ray = UIManager.cameraWorld.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if(Physics.Raycast(ray,out hit, m_Range))
                    {
                        if(hit.transform.name == "Rigid Cube")
                        {
                            m_Target = hit.transform;
                        }
                    }
                }
            }
            else
            {
                if(m_Target != null)
                {
                    if( m_Target.rigidbody != null)
                    {
                        m_Target.rigidbody.useGravity = true;
                    }
                    m_Target = null;
                }
            }
            m_LifeTime += InputManager.GetAxis("Mouse ScrollWheel") * Time.deltaTime * m_RangeSpeed;
            m_LifeTime = Mathf.Clamp(m_LifeTime, m_MinLifeTime, m_MaxLifeTime);
            m_Range = m_LifeTime * 2.0f;
        }
        
    }
}