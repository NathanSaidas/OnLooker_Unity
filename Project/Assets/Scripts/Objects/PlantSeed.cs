using UnityEngine;
using System.Collections;

namespace EndevGame
{
   
    
    public class PlantSeed : PlantComponent
    {

        //[SerializeField]
        //private Mesh m_SeedMesh = null;
        //[SerializeField]
        //private Material m_SeedMaterial = null;
        [SerializeField]
        private Transform m_SeedTransform = null;
        [SerializeField]
        private Transform m_PlantTransform = null;
        [SerializeField]
        private float m_AbsorbSpeed = 1.0f;
        [SerializeField]
        private float m_PickUpDistance = 3.0f;

        protected override void Start()
        {
            base.Start();
            PlantGrowth plantGrowth = GetComponent<PlantGrowth>();
            if(plantGrowth != null)
            {
                plantGrowth.registerEvent(onExitStage, onStageEvent);
            }
            if(m_SeedTransform != null)
            {
                m_SeedTransform.gameObject.SetActive(false);
            }
        }

        private void onExitStage(PlantStage aStage)
        {

        }

        private void onStageEvent(PlantStage aStage)
        {
            
            if(aStage == PlantStage.SEED)
            {
                
                PlantGrowth plantGrowth = GetComponent<PlantGrowth>();
                if(plantGrowth != null)
                {
                    if(plantGrowth.isStatic)
                    {
                        Debug.Log("Become Seed");
                        return;
                    }
                }
                if(m_SeedTransform != null && m_PlantTransform != null)
                {
                    Debug.Log("Become Seed");
                    m_PlantTransform.gameObject.SetActive(false);
                    m_SeedTransform.gameObject.SetActive(true);
                    PickUpBounce bounce = m_SeedTransform.GetComponent<PickUpBounce>();
                    if(bounce != null)
                    {
                        bounce.startBounce();
                    }
                }
            }
        }

        private void OnTriggerStay(Collider aCollider)
        {
            PlantGrowth plantGrowth = GetComponent<PlantGrowth>();
            CharacterManager charManager = null;
            if((charManager = aCollider.GetComponent<CharacterManager>()) != null && plantGrowth.currentScale < 0.0f)
            {
                
                if (Vector3.Distance(charManager.transform.position, transform.position) < m_PickUpDistance)
                {
                    //charManager.pickSeed("SeedName", manager);
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, charManager.transform.position, Time.deltaTime * m_AbsorbSpeed);
                }
            }
        }
        
    }
}