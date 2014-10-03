using UnityEngine;
using System.Collections;


namespace EndevGame
{

    public class PlantComponent : MonoBehaviour
    {
        [SerializeField]
        private PlantManager m_Manager;

        // Use this for initialization
        protected virtual void Start()
        {
            m_Manager = GetComponent<PlantManager>();
            getManagerInParent();

            if (m_Manager == null)
            {
                m_Manager = GetComponent<PlantManager>();
            }
        }

        private void getManagerInParent()
        {
            if(m_Manager != null)
            {
                return;
            }

            Transform parent = transform;
            for(int i = 0 ; i < 7; i++)
            {
                parent = parent.parent;
                m_Manager = parent.GetComponent<PlantManager>();
                if(m_Manager != null)
                {
                    break;
                }
            }
        }

        public PlantManager manager
        {
            get { if (m_Manager == null) { Debug.LogWarning("Manager not found. Invoke PlantComponent.Start()"); } return m_Manager; }
        }

        public InteractivePlant plantInteraction
        {
            get { return manager == null ? null : manager.plantInteractionComponent; }
        }
        public PlantGrowth plantGrowth
        {
            get { return manager == null ? null : manager.plantGrowthComponent; }
        }
        public PlantSeed plantSeed
        {
            get { return manager == null ? null : manager.plantSeedComponent; }
        }


        public virtual bool isStatic
        {
            get { return plantGrowth == null ? false : plantGrowth.isStatic; }
        }
        public virtual PlantLightRequirement lightRequirement
        {
            get { return plantGrowth == null ? PlantLightRequirement.REQUIRES_LIGHT : plantGrowth.lightRequirement; }
        }
        public virtual PlantStage plantStage
        {
            get { return plantGrowth == null ? PlantStage.FIRST : plantGrowth.plantStage; }
        }
        public virtual bool hasLight
        {
            get { return plantGrowth == null ? false : plantGrowth.hasLight; }
        }

        public void registerGrowthEvent(OnPlantChangeCallback aExitCallback, OnPlantChangeCallback aChangeEventCallback)
        {
            if (plantGrowth != null)
            {
                plantGrowth.registerEvent(aExitCallback, aChangeEventCallback);
            }
        }
        public void unregisterGrowthEvent(OnPlantChangeCallback aExitCallback, OnPlantChangeCallback aChangeEventCallback)
        {
            if (plantGrowth != null)
            {
                plantGrowth.unregisterEvent(aExitCallback, aChangeEventCallback);
            }
        }

        public void registerInteractiveEvent(OnInteractiveCallback aCallback)
        {
            if (aCallback != null && plantInteraction != null)
            {
                plantInteraction.register(aCallback);
            }
        }
        public void unregisterInteractiveEvent(OnInteractiveCallback aCallback)
        {
            if (aCallback != null && plantInteraction != null)
            {
                plantInteraction.unregister(aCallback);
            }
        }

        public void DestroyPlant()
        {
            Destroy(gameObject);
        }
    }
}