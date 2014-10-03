using UnityEngine;
using System.Collections;


namespace EndevGame
{

    /*
    *   Class: PickUpBounce
    *   Base Class: MonoBehaviour
    *   Interfaces: None
    *   Description:The purpose of this class is to control an objects transform by moving it up and down and rotating it.
    *   Date Reviewed: Sept 26th 2014 by Nathan Hanlan
    */
    //This class requires a MeshRenderer and a MeshFilter
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class PickUpBounce : MonoBehaviour
    {
        private MeshRenderer m_MeshRenderer = null;
        private MeshFilter m_MeshFilter = null;

        // The boolean to determine the bouncing state of this object
        //[SerializeField]
        private bool m_IsBouncing = false;
        // A default rotation for the object to start at
        //[SerializeField]
        private Vector3 m_DefaultRotation = Vector3.zero;
        //The start y value of the object
        //[SerializeField]
        private float m_StartY = 0.0f;


        //These are the important variables users should edit inside the editor.
        //How far up and down the object should bounce
        [SerializeField]
        private float m_BounceLength = 2.0f;
        //The speed at which the object should rotate
        [SerializeField]
        private float m_RotationSpeed = 5.0f;
        //The direction the object should rotate around.
        [SerializeField]
        private Vector3 m_RotationDirection = new Vector3(0.0f, 1.0f, 0.0f);
        //How far off the ground should the start position in world space on the y axis should the bounce start from the raycast hit point.
        [SerializeField]
        private float m_GroundOffset = 1.0f;
        //The mesh and material to assign the object when it begins bouncing
        [SerializeField]
        private Mesh m_Mesh = null;
        [SerializeField]
        private Material m_Material = null;


        /// <summary>
        /// //Checks to make sure the required components are set
        /// </summary>
        void OnEnable()
        {
            if((m_MeshRenderer = GetComponent<MeshRenderer>()) == null)
            {
                Debug.Log("Missing component \'MeshRenderer\'");
                enabled = false;
                return;
            }
            if((m_MeshFilter = GetComponent<MeshFilter>()) == null)
            {
                Debug.Log("Missing component \'MeshFilter");
                enabled = false;
                return;
            }
        }

        
        /// <summary>
        /// If bouncing is enabled this component rotates and moves the object
        /// </summary>
        void Update()
        {
            
            if (m_IsBouncing == true)
            {
                transform.position = new Vector3(transform.position.x, m_StartY + Mathf.Sin(Time.time) * m_BounceLength, transform.position.z);
                transform.rotation = transform.rotation * Quaternion.Euler(m_RotationDirection.x * m_RotationSpeed, m_RotationDirection.y * m_RotationSpeed, m_RotationDirection.z * m_RotationSpeed);
            }
        }


        //A helper method to start bouncing the object
        public void startBounce()
        {
            if (enabled == false)
            {
                return;
            }

            //The material / mesh accordingly
            m_MeshRenderer.material = m_Material;
            m_MeshFilter.mesh = m_Mesh;

            //Force ground offset to be positive
            m_GroundOffset = Mathf.Abs(m_GroundOffset);


            //Do a single raycast to calculate where the start point should be using the ground offset variable as well
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1000.0f))
            {
                m_StartY = hit.point.y + m_GroundOffset;
                transform.position = new Vector3(transform.position.x, m_StartY, transform.position.z);
                transform.rotation = Quaternion.identity * Quaternion.Euler(m_DefaultRotation);
            }
            else
            {
                Debug.LogWarning("Missing a raycast in PickUpBounce.startBounce().");
            }
            //Finally turn the is bouncing flag on
            m_IsBouncing = true;

        }

        //Helper to end bouncing
        public void endBounce()
        {
            m_IsBouncing = false;
        }

        public bool isBouncing
        {
            get { return m_IsBouncing == true && enabled == true; }
        }
        public float startY
        {
            get { return m_StartY; }
        }
        public float bounceLength
        {
            get { return m_BounceLength; }
            set { m_BounceLength = value; }
        }
        public float rotationSpeed
        {
            get { return m_RotationSpeed; }
            set { m_RotationSpeed = value; }
        }
        public Vector3 rotationDirection
        {
            get { return m_RotationDirection; }
            set { m_RotationDirection = value; }
        }
        public Mesh mesh
        {
            get { return m_Mesh;}
            set { m_Mesh = value; }
        }
        public Material material
        {
            get { return m_Material; }
            set { m_Material = value; }
        }
    }
}