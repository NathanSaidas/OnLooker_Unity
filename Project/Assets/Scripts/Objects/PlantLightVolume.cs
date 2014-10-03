using UnityEngine;
using System.Collections.Generic;

namespace EndevGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlantLightVolume : PlantComponent
    {
        [SerializeField]
        private bool m_CorruptedLight = false;
        [SerializeField]
        private DebugMode m_Debug;

        [SerializeField]
        private float m_Length;
        [SerializeField]
        private float m_Width;
        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            Rigidbody body = GetComponent<Rigidbody>();
            if (body != null)
            {
                body.isKinematic = true;
                body.useGravity = false;
            }

        }

        // Update is called once per frame
        void Update()
        {
      
        }

        void OnDrawGizmosSelected()
        {
            if(m_Debug == DebugMode.ON_SELECTED)
            {
                Gizmos.color = Color.red;
                Matrix4x4 matrix = new Matrix4x4();
                matrix.SetTRS(transform.position, transform.rotation, transform.localScale);
                Gizmos.matrix = matrix;
                CapsuleCollider capCol = (CapsuleCollider)collider;

                //switch(capCol.direction)
                //{
                //    case 0:
                //        Gizmos.DrawCube(transform.position, new Vector3(1.0f * transform.localScale.x, 1.0f * transform.localScale.y, capCol.height * transform.localScale.z));
                //        break;
                //    case 1:
                //        Gizmos.DrawCube(transform.position, new Vector3(1.0f * transform.localScale.x, capCol.height * transform.localScale.y, 1.0f * transform.localScale.z));
                //        break;
                //    case 2:
                //        Gizmos.DrawCube(transform.position, new Vector3(capCol.height * transform.localScale.x, 1.0f * transform.localScale.y, 1.0f * transform.localScale.z));
                //        break;
                //}
                Gizmos.DrawCube(Vector3.one, Vector3.one);

            }
        }
        

        public bool corruptedLight
        {
            get { return m_CorruptedLight; }
            set { m_CorruptedLight = value; }
        }
    }
}