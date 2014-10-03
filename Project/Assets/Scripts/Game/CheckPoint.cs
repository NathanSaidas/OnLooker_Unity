using UnityEngine;
using System.Collections;



    public class CheckPoint : MonoBehaviour
    {
        [SerializeField]
        private DebugMode m_DebugMode = DebugMode.OFF;
        [SerializeField]
        private int m_ID = 0;

        //private PlayerProfile m_playerProfile;

        private void Start()
        {
            //GameManager
            GameManager.register(this);
        }
        private void OnDestroy()
        {
            GameManager.unregister(this);
        }


        public void checkPointReached()
        {
            //TODO: Add any special effects for when the player reaches a check point=p
        }

        //Position of the checkpoint
        public Vector3 position
        {   
            get { return transform.position; }
        }
        public Quaternion rotation
        {
            get { return transform.rotation; }
        }
        public int id
        {
            get { return m_ID; }
        }
            

        private void OnDrawGizmos()
        {
            if (m_DebugMode == DebugMode.ON)
            {
                Matrix4x4 rotMat = Matrix4x4.identity;
                rotMat.SetTRS(transform.position, transform.rotation, transform.localScale);

                Gizmos.color = Color.green;
                Gizmos.matrix = rotMat;
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                Gizmos.DrawLine(Vector3.zero,  Vector3.forward * 5.0f);
            }
        }
        private void OnDrawGizmosSelected()
        {
            if (m_DebugMode == DebugMode.ON_SELECTED)
            {
                Matrix4x4 rotMat = Matrix4x4.identity;
                rotMat.SetTRS(transform.position, transform.rotation, transform.localScale);

                Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.5f);
                Gizmos.matrix = rotMat;
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
                Gizmos.DrawLine(Vector3.zero, Vector3.forward * 5.0f);
            }
        }
        private void OnTriggerEnter(Collider aCollider)
        {
            //if(aCollider.GetComponent<CharacterManager>() == null)
            //{
            //    return;
            //}
            //GameManager.checkPointReached(this);
              
            //Debug.Log("Checkpoint Reached!");
        }
    }