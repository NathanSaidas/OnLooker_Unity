using UnityEngine;
using System.Collections.Generic;

namespace EndevGame
{

    /*
    *   Class: PlantManager
    *   Base Class: MonoBehaviour
    *   Interfaces: None
    *   Description: The purpose of this class is the manage the data that will be shared by the plants as well as provide helper functions to have them communicate with each other
    *   Date Reviewed: Sept 26th 2014 by Nathan Hanlan
    */
    public class PlantManager : MonoBehaviour
    {
        //The interactive plant component the plant manager has that allows a object in the world to interact with this object
        private InteractivePlant m_PlantInteractionComponent = null;
        //The plant growth component in charge of growing / shrinking the plant
        private PlantGrowth m_PlantGrowthComponent = null;
        //The plant seed component in charge of handling all seed functionality when it comes to pickups
        private PlantSeed m_PlantSeedComponent = null;
        //The type of plant this plant manager is considered
        private PlantType m_PlantType = PlantType.NONE;

        

        //private SunflowerBehavior m_SunflowerBehaviour = null;


        void Start()
        {
            m_PlantGrowthComponent = GetComponent<PlantGrowth>();
            m_PlantInteractionComponent = GetComponentInChildren<InteractivePlant>();
            m_PlantSeedComponent = GetComponent<PlantSeed>();
        }


        //Helpers to get the core components of a plant
        public InteractivePlant plantInteractionComponent
        {
            get { if (m_PlantInteractionComponent == null) { m_PlantInteractionComponent = GetComponentInChildren<InteractivePlant>(); } return m_PlantInteractionComponent; }
            //set { m_PlantInteractionComponent = value; }
        }
        public PlantGrowth plantGrowthComponent
        {
            get { if (m_PlantGrowthComponent == null) { m_PlantGrowthComponent = GetComponent<PlantGrowth>(); } return m_PlantGrowthComponent; }
            //set { m_PlantGrowthComponent = value; }
        }
        public PlantSeed plantSeedComponent
        {
            get { if (m_PlantSeedComponent == null) { m_PlantSeedComponent = GetComponent<PlantSeed>(); } return m_PlantSeedComponent; }
            //set { m_PlantSeedComponent = value; }
        }

        //Wrapper to determine if the plant is flagged as static or not
        public bool isStatic
        {
            get { return plantGrowthComponent == null ? false : m_PlantGrowthComponent.isStatic; }
        }
        //Wrapper to determine the light requirement property of the plant
        public PlantLightRequirement lightRequirement
        {
            get { return plantGrowthComponent == null ? PlantLightRequirement.REQUIRES_LIGHT : m_PlantGrowthComponent.lightRequirement; }
        }
        //Wrapper to determine the current stage the plant is in
        public PlantStage plantStage
        {
            get { return plantGrowthComponent == null ? PlantStage.FIRST : m_PlantGrowthComponent.plantStage; }
        }
        //Wrapper to determine if the plant has light or not
        public bool hasLight
        {
            get { return plantGrowthComponent == null ? false : m_PlantGrowthComponent.hasLight; }
        }
        //Wrapper to register for grow / shrink events - Requires Plant Growth
        public void registerGrowthEvent(OnPlantChangeCallback aExitCallback, OnPlantChangeCallback aChangeEventCallback )
        {
            if(m_PlantGrowthComponent != null)
            {
                m_PlantGrowthComponent.registerEvent(aExitCallback, aChangeEventCallback);
            }
        }
        //Wrapper to unregister for grow / shrink events. - Requires Plant Growth
        public void unregisterGrowthEvent(OnPlantChangeCallback aExitCallback, OnPlantChangeCallback aChangeEventCallback )
        {
            if (m_PlantGrowthComponent != null)
            {
                m_PlantGrowthComponent.unregisterEvent(aExitCallback, aChangeEventCallback);
            }
        }
        //Wrapper to register for interaction events - Requires InteractivePlant
        public void registerInteractiveEvent(OnInteractiveCallback aCallback)
        {
            if(aCallback != null && m_PlantInteractionComponent != null)
            {
                m_PlantInteractionComponent.register(aCallback);
            }
        }
        //Wrapper to unregister for interaction events - Requires Interactive Plant
        public void unregisterInteractiveEvent(OnInteractiveCallback aCallback)
        {
            if(aCallback != null && m_PlantInteractionComponent != null)
            {
                m_PlantInteractionComponent.unregister(aCallback);
            }
        }

        //Returns the type of plant this is
        public PlantType plantType
        {
            get { return m_PlantType; }
            set { m_PlantType = value; }
        }
            
    }
}