using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gem
{


    [Serializable]
    public class Gravity : Ability
    {
        [SerializeField]
        private ParticleSystem m_ParticleSystem = null;
        [SerializeField]
        private int m_Count = 2;
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
        [SerializeField]
        private List<string> m_AcceptedTags = new List<string>();

        public override void UpdateAbility(float aTime)
        {
            base.UpdateAbility(aTime);
            m_LifeTime += InputManager.GetAxis("Mouse ScrollWheel") * Time.deltaTime * m_RangeSpeed;
            m_LifeTime = Mathf.Clamp(m_LifeTime, m_MinLifeTime, m_MaxLifeTime);
            m_Range = m_LifeTime * 2.0f;
        }

        public override bool CheckResource()
        {
            return base.CheckResource();
        }
        public override void EndExecute()
        {
            base.EndExecute();
            if (m_Target != null)
            {
                if (m_Target.rigidbody != null)
                {
                    m_Target.rigidbody.useGravity = true;
                }
                m_Target = null;
            }
        }
        public override void Execute()
        {
            base.Execute();
            if(m_ParticleSystem != null && owner != null)
            {
                m_ParticleSystem.transform.rotation = UIManager.cameraWorld.transform.rotation;
                m_ParticleSystem.startLifetime = m_LifeTime;
                m_ParticleSystem.Emit(m_Count);

                if (m_Target != null)
                {
                    if (m_Target.rigidbody != null)
                    {
                        m_Target.rigidbody.useGravity = false;
                    }
                    m_Target.position = Vector3.Lerp(m_Target.position, owner.transform.position + UIManager.cameraWorld.transform.forward * m_Range, Time.deltaTime * m_ClampSpeed);
                }
                else
                {
                    Ray ray = UIManager.cameraWorld.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, m_Range))
                    {
                        if(m_AcceptedTags.Any(Element => Element == hit.transform.tag))
                        {
                            m_Target = hit.transform;
                        }
                    }
                }
                owner.UseResource(UnitResourceType.RESOURCE, resourceCost);
            }


        }

        public override void UpdateReference()
        {
            
            if(owner != null && owner.rightHand != null)
            {
                if (m_ParticleSystem == null)
                {
                    m_ParticleSystem = owner.rightHand.GetComponentInChildren<ParticleSystem>();
                    if(m_ParticleSystem != null)
                    {
                        m_ParticleSystem.Stop();
                    }
                }
            }

            if(m_ParticleSystem != null)
            {
                DebugUtils.Log("Found Particle System");
            }
            else
            {
                DebugUtils.Log("Did not find Particle System");
            }
        }
    }
}