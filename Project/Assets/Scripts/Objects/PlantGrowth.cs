using UnityEngine;
using System.Collections.Generic;

namespace EndevGame
{

    public enum PlantLightRequirement
    {
        REQUIRES_LIGHT, //requires light to grow
        REQUIRES_DARKNESS, //requires darkness to grow
        NO_REQUIREMENT //doesnt matter grows in all light and doesnt decay.
    }

    public enum PlantStage
    {
        SEED, //where current scale <= 0.0f
        FIRST, //where current scale > 0.0f && <= 0.33f
        SECOND, //where current scale > 0.33f && <= 0.66f
        THIRD //where current scale > 0.66f
    }

    public delegate void OnPlantChangeCallback(PlantStage aStage);



    /*
    *   Class: PlantGrowth
    *   Base Class: PlantComponent
    *   Interfaces: None
    *   Description: The purpose of this class is to control the growth of plants by registering light volumes and calculating if they are actually lighting the plant or not using raycasts.
    *   Date Reviewed: Sept 26th 2014 by Nathan Hanlan
    */
    [RequireComponent(typeof(Rigidbody))]
    public class PlantGrowth : PlantComponent
    {
        //The speed at which the plant scales at
        [SerializeField]
        private float m_ScaleSpeed = 2.0f;
        //How much of the scale percentage should the plant to into account for its scaling
        [SerializeField]
        private float m_ScaleFactor = 2.0f;
        //The minimum amount the plant can scale to
        [SerializeField]
        private float m_MinScale = 0.3f;
        //The maximum amount the plant can scale to
        [SerializeField]
        private float m_MaxScale = 2.0f;
        //The current scale of the plant -1 to 1 value
        [SerializeField]
        private float m_CurrentScale = 0.0f;
        //The currently level of corruption layed down on the plant
        [SerializeField]
        private int m_CorruptionLevel = 0;

        //A boolean flag to determine if the plant is static or not
        [SerializeField]
        private bool m_IsStatic = false;
        [SerializeField]
        private bool m_StaticGrow = false;
        [SerializeField]
        private bool m_StaticShrink = false;
        //The light requirement for the plant. See the enum for more information
        [SerializeField]
        private PlantLightRequirement m_LightRequirement = PlantLightRequirement.REQUIRES_LIGHT;
        //The current stage state the plant is in
        [SerializeField]
        private PlantStage m_PlantStage = PlantStage.FIRST;

        //Event callbacks
        private event OnPlantChangeCallback m_ExitEvent;
        private event OnPlantChangeCallback m_ChangeEvent;

        //How much light the plant currently has from the directional light
        [SerializeField]
        private float m_DirectionLightInfluence = 0.0f;
        //How much light the plant currently has from light volumes
        [SerializeField]
        private float m_LightInfluence = 0.0f;
        //How fast the plant should lose / gain its light influence
        [SerializeField]
        private float m_LightSpeed = 3.0f;

        //A list of triggering light volumes. This is not a confirmed light source but the PlantGrowth uses these later to process a light influence
        [SerializeField]
        private List<PlantLightVolume> m_LightVolumes = new List<PlantLightVolume>();
        private PlantLightVolume m_LastLightVolume = null;

        //The transform that should be scaled
        [SerializeField]
        private Transform m_AffectedTransform = null;

        //Used for scaling calculations
        private float m_StartY = 0.0f;
        [SerializeField]
        private float m_Offset = 0.0f;

        //This script requires a rigidbody and a collider however they must be setup to a certain state.
        protected override void Start()
        {
            base.Start();

            //Make the collider a trigger
            Collider anyCollider = GetComponent<Collider>();
            if (anyCollider != null)
            {
                anyCollider.isTrigger = true;
            }

            //Make the rigidbody kinematic and not use gravity
            Rigidbody body = GetComponent<Rigidbody>();
            if (body != null)
            {
                body.useGravity = false;
                body.isKinematic = true;
            }

            //Find the ground
            RaycastHit hit;
            if(Physics.Raycast(transform.position,Vector3.down,out hit,30.0f))
            {
                m_StartY = hit.point.y;
                //Debug.Log("Start Y" + m_StartY);
            }
            else
            {
                Debug.LogWarning("Missed a raycast on Start");
            }
            
            reset();
        }

        /// <summary>
        /// When a light volume collides with the plant trigger we can add it to the list of light volumes to check for light 
        /// </summary>
        /// <param name="aCollider"></param>
        void OnTriggerEnter(Collider aCollider)
        {
            PlantLightVolume lightVolume = null;
            if ((lightVolume = aCollider.GetComponent<PlantLightVolume>()) != null)
            {
                if(lightVolume.manager == manager)
                {
                    return;
                }
                if (lightVolume.corruptedLight == true)
                {
                    m_CorruptionLevel++;
                }
                m_LightVolumes.Add(lightVolume);
            }
        }
        /// <summary>
        /// When a light volume stops colliding with the plant it needs to be removed. 
        /// </summary>
        /// <param name="aCollider"></param>
        void OnTriggerExit(Collider aCollider)
        {
            PlantLightVolume lightVolume = null;
            if ((lightVolume = aCollider.GetComponent<PlantLightVolume>()) != null)
            {
                
                if (lightVolume.corruptedLight == true)
                {
                    m_CorruptionLevel--;
                }
                m_LightVolumes.Remove(lightVolume);
                if(m_LastLightVolume == lightVolume)
                {
                    m_LastLightVolume = null;
                }
            }
        }


        //Decays the plant if the conditions are met to decay it
        void Update()
        {
            if (hasLight == false && m_LightRequirement == PlantLightRequirement.REQUIRES_LIGHT)
            {
                if (isStatic == true && m_StaticShrink == true)
                {
                    decay();
                }
            }
            else if (hasLight == true && m_LightRequirement == PlantLightRequirement.REQUIRES_DARKNESS)
            {
                if (isStatic == true && m_StaticShrink == true)
                {
                    decay();
                }
            }
            else
            {
                if (isStatic == true && m_StaticGrow == true)
                {
                    grow();
                }
            }
            
            //Adjust the plants position based on the scale
            transform.position = new Vector3(transform.position.x, m_StartY + Mathf.Clamp(m_CurrentScale * m_ScaleFactor * 0.5f, m_MinScale,m_MaxScale)  + m_Offset, transform.position.z);
        }

        //Calculate light influence with raycasts
        void FixedUpdate()
        {
            
            if(LightManager.directionalLight != null)
            {
                calculateDirectionLight();
            }
            else
            {
                m_DirectionLightInfluence = Mathf.Clamp(m_DirectionLightInfluence - m_LightSpeed * Time.deltaTime,0.0f,Mathf.Infinity);
            }

            if(m_LightVolumes != null && m_LightVolumes.Count > 0)
            {
                calculateLightVolumeLight();
            }
            else
            {
                m_LightInfluence = Mathf.Clamp(m_LightInfluence - m_LightSpeed * Time.deltaTime, 0.0f, Mathf.Infinity);
            }
        }

        void calculateDirectionLight()
        {

            Transform directionLight = LightManager.directionalLight;
            RaycastHit hit;
            int layerMask = 1 << GameManager.PLANT_LIGHT_LAYER;
            if(Physics.Linecast(transform.position,directionLight.position,out hit,layerMask))
            {
                if(hit.collider == collider)
                {
                    //Ignore our own collider and increase the light value
                    m_DirectionLightInfluence = Mathf.Clamp01(m_DirectionLightInfluence + m_LightSpeed * Time.deltaTime);
                }
                else
                {
                    //Some surface was thus we must decrease the light
                    m_DirectionLightInfluence = Mathf.Clamp01(m_DirectionLightInfluence - m_LightSpeed * Time.deltaTime);
                }
            }
            else
            {
                //No colliders were hit thus the light value increases
                m_DirectionLightInfluence = Mathf.Clamp01(m_DirectionLightInfluence + m_LightSpeed * Time.deltaTime);
            }

        }
        void calculateLightVolumeLight()
        {

            int count = m_LightVolumes.Count;
            int light = 0;

            RaycastHit hit;
            int layerMask = 1 << GameManager.PLANT_LIGHT_LAYER;

            //Before checking the entire list with raycast check the last light volume that lit the plant and see if its still lighting the plant
            if(m_LastLightVolume != null && Physics.Linecast(transform.position, m_LastLightVolume.transform.position,out hit, layerMask))
            {
                //if it was increase light
                if(hit.collider == collider)
                {
                    light++;
                }
            }
            //check to make sure there was a light volume but the line cast didnt hit any objects
            else if(m_LastLightVolume != null)
            {
                light++;
            }
            else //worst case scenario do a line cast from all light volumes until one is lighting the plant
            {
                //Line cast from all the plants until 
                for (int i = 0; i < count; i++)
                {
                    if (Physics.Linecast(transform.position, m_LightVolumes[i].transform.position, out hit, layerMask))
                    {
                        if (hit.collider == collider)
                        {
                            //m_LightInfluence = Mathf.Clamp01(m_LightInfluence + m_LightSpeed * Time.deltaTime);
                            light++;
                            m_LastLightVolume = m_LightVolumes[i];
                            break;
                        }
                    }
                    else
                    {
                        light++;
                        m_LastLightVolume = m_LightVolumes[i];
                        break;
                    }
                }
            }

            
            //if has light then increase the light influence else decrease it
            if(light > 0)
            {
                m_LightInfluence = Mathf.Clamp01(m_LightInfluence + m_LightSpeed * Time.deltaTime);
            }
            else
            {
                m_LightInfluence = Mathf.Clamp01(m_LightInfluence - m_LightSpeed * Time.deltaTime);
            }

        }


        

        //gets called by the user to grow the plant
        public void grow(CharacterManager aTriggeringCharacter)
        {
            if(aTriggeringCharacter == null)
            {
                Debug.LogWarning("Null character tried to grow plant");
                return;
            }
            if(m_IsStatic == true)
            {
                return;
            }
            //TO FROM
            float distance = Vector3.Distance(aTriggeringCharacter.transform.position, transform.position);
            Vector3 direction = aTriggeringCharacter.transform.position - transform.position;
            RaycastHit hit;
            if(Physics.Raycast(transform.position,direction,out hit, distance + 5.0f))
            {
                if(hit.collider != aTriggeringCharacter.collider)
                {
                    Debug.Log("Character was not in line of sight of the collider");
                    return;
                }
            }

            if(isCorrupted == true)
            {
                return;
            }
            else if(m_LightRequirement == PlantLightRequirement.NO_REQUIREMENT)
            {
                growPlant();
            }
            else if(hasLight == true && m_LightRequirement == PlantLightRequirement.REQUIRES_LIGHT)
            {
                growPlant();
            }
            else if(hasLight == false && m_LightRequirement == PlantLightRequirement.REQUIRES_DARKNESS)
            {
                growPlant();
            }
        }
        private void grow()
        {
            if (isCorrupted == true)
            {
                return;
            }
            else if (m_LightRequirement == PlantLightRequirement.NO_REQUIREMENT)
            {
                growPlant();
            }
            else if (hasLight == true && m_LightRequirement == PlantLightRequirement.REQUIRES_LIGHT)
            {
                growPlant();
            }
            else if (hasLight == false && m_LightRequirement == PlantLightRequirement.REQUIRES_DARKNESS)
            {
                growPlant();
            }
        }


        //gets called by the user to shrink the plant
        public void shrink(CharacterManager aTriggeringCharacter)
        {
            //Debug.Log("Plant Shrink");
            if (aTriggeringCharacter == null)
            {
                Debug.LogWarning("False character tried to shrink plant");
                return;
            }
            if(m_IsStatic == true)
            {
                return;
            }
            RaycastHit hit;
            float distance = Vector3.Distance(aTriggeringCharacter.transform.position, transform.position);
            Vector3 direction = aTriggeringCharacter.transform.position - transform.position;
            if (Physics.Raycast(transform.position, direction, out hit, distance + 5.0f))
            {
                if (hit.collider != aTriggeringCharacter.collider)
                {
                    Debug.Log("Character was not in line of sight of the collider");
                    return;
                }
            }

            if(isCorrupted == true)
            {
                return;
            }
            shrinkPlant();
        }
        public void shrink()
        {

            if (isCorrupted == true)
            {
                return;
            }
            shrinkPlant();
        }
        //gets called on updates 
        void decay()
        {
            if(isCorrupted == true)
            {
                return;
            }
            else
            {
                decayPlant();
            }
        }

        //helper method to shrink the plant
        void shrinkPlant()
        {
            float prevScale = m_CurrentScale;
            if(m_IsStatic == true)
            {
                m_CurrentScale = Mathf.Clamp(m_CurrentScale - m_ScaleSpeed * Time.deltaTime, 0.0f, 1.0f);
            }
            else
            {
                m_CurrentScale = Mathf.Clamp(m_CurrentScale - m_ScaleSpeed * Time.deltaTime, -0.1f, 1.0f);
            }
            setScale(-1.0f);
            handleStateChange(prevScale);
        }

        //helper method to grow the plant
        void growPlant()
        {
            float prevScale = m_CurrentScale;
            m_CurrentScale = Mathf.Clamp(m_CurrentScale + m_ScaleSpeed * Time.deltaTime, -0.1f, 1.0f);
            if(m_CurrentScale >= 0.0f && prevScale < 0.0f)
            {
                m_CurrentScale = prevScale;
                return;
            }
            setScale(1.0f);
            handleStateChange(prevScale);
        }
        //helper method to decay the plant
        void decayPlant()
        {
            shrinkPlant();
        }
        
        // aDirection is obsolete due to changes in other growPlant() and shrinkPlant()
        void setScale(float aDirection)
        {
            
            float localScale = Mathf.Clamp(m_CurrentScale * m_ScaleFactor, m_MinScale, m_MaxScale);
            if (m_AffectedTransform == null)
            {
                transform.localScale = new Vector3(localScale, localScale, localScale);
                //transform.position += new Vector3(0.0f, localScale * 0.1f, 0.0f) * aDirection;
            }
            else
            {
                m_AffectedTransform.localScale = new Vector3(localScale, localScale, localScale);
                //transform.position += new Vector3(0.0f, localScale * 0.1f, 0.0f) * aDirection;
            }
        }
            
            

        void handleStateChange(float aPreviousScale)
        {
            //Store the previous stage
            PlantStage prevStage = m_PlantStage;

            //evaluate the current stage
            if(m_CurrentScale <= 0.0 && m_IsStatic == false)
            {
                m_PlantStage = PlantStage.SEED;
            }
            else if(m_CurrentScale > 0.0f && m_CurrentScale <= 0.33f)
            {
                m_PlantStage = PlantStage.FIRST;
            }
            else if(m_CurrentScale > 0.33f && m_CurrentScale <= 0.66f)
            {
                m_PlantStage = PlantStage.SECOND;
            }
            else if (m_CurrentScale > 0.66f)
            {
                m_PlantStage = PlantStage.THIRD;
            }

            //if the prev is not equal the current there was a change, release the old, replace with the new
            if(m_PlantStage != prevStage)
            {
                sendExitEvent(prevStage);
                sendStageChangeEvent(m_PlantStage);
            }

        }

        void sendExitEvent(PlantStage aStage)
        {
            if(m_ExitEvent != null)
            {
                m_ExitEvent.Invoke(aStage);
            }
        }
        void sendStageChangeEvent(PlantStage aStage)
        {
            if(m_ChangeEvent != null)
            {
                m_ChangeEvent.Invoke(aStage);
            }
        }

        public void registerEvent(OnPlantChangeCallback aExitCallback, OnPlantChangeCallback aChangeEventCallback)
        {
            if(aExitCallback != null)
            {
                m_ExitEvent += aExitCallback;
            }
            if(aChangeEventCallback != null)
            {
                m_ChangeEvent += aChangeEventCallback;
            }
        }
        public void unregisterEvent(OnPlantChangeCallback aExitCallback, OnPlantChangeCallback aChangeEventCallback)
        {
            if(aExitCallback != null && m_ExitEvent != null)
            {
                m_ExitEvent -= aExitCallback;
            }
            if(aChangeEventCallback != null && m_ChangeEvent != null)
            {
                m_ChangeEvent -= aChangeEventCallback;
            }
        }

        public void reset()
        {
            m_CurrentScale = 0.15f;
            setScale(1);
        }

        
        public float scaleSpeed
        {
            get { return m_ScaleSpeed; }
            set { m_ScaleSpeed = value; }
        }
        public float scaleFactor
        {
            get { return m_ScaleFactor; }
            set { m_ScaleFactor = value; }
        }
        public float minScale
        {
            get { return m_MinScale; }
            set { m_MaxScale = value; }
        }
        public float currentScale
        {
            get { return m_CurrentScale; }
        }
        public bool isCorrupted
        {
            get { return m_CorruptionLevel > 0; }
        }
        public override bool isStatic
        {
            get { return m_IsStatic; }
        }
        public override PlantLightRequirement lightRequirement
        {
            get { return m_LightRequirement; }
        }
        public override PlantStage plantStage
        {
            get { return m_PlantStage; }
        }
        public float directionLightInfluence
        {
            get { return m_DirectionLightInfluence; }
        }
        public float lightInfluence
        {
            get { return m_LightInfluence; }
        }
        public float lightSpeed
        {
            get { return m_LightSpeed; }
            set { m_LightSpeed = value; }
        }
        public override bool hasLight
        {
            get { return m_DirectionLightInfluence + m_LightInfluence > 0.0f; }
        }

    }


}
