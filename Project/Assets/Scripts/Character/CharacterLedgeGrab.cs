using UnityEngine;
using System.Collections;

namespace EndevGame
{

    /// <summary>
    /// Original Design by Justin
    /// </summary>
    public class CharacterLedgeGrab : EndevBehaviour
    {
        [SerializeField]
        private BoxCollider m_MainCollider = null;
        [SerializeField]
        private BoxCollider m_BoundsLeft = null;
        [SerializeField]
        private BoxCollider m_BoundsRight = null;

        [SerializeField]
        private CharacterLedgeGrab m_LeftNeighbour = null;
        [SerializeField]
        private CharacterLedgeGrab m_RightNeighbour = null;

        [SerializeField]
        Vector3 m_Origin = Vector3.zero;
        [SerializeField]
        Vector3 m_Direction = Vector3.zero;
        [SerializeField]
        float m_Length = 0.0f;

        [SerializeField]
        private Transform m_Player = null;
        [SerializeField]
        private float m_PlayerOffset = 0.0f;
        [SerializeField]
        private bool m_IsAtttached = true;
        [SerializeField]
        private float m_MovementSpeed = 1.0f;

        private bool m_LeftHit = false;
        private bool m_RightHit = false;

        private void OnTriggerEnter(Collider aOther)
        {
            WorldObject ledge = aOther.gameObject.GetComponent<WorldObject>();
            if(ledge == null || ledge.objectType != WorldObjectType.LEDGE)
            {
                return;
            }
        }
        private void OnTriggerStay(Collider aOther)
        {
            WorldObject ledge = aOther.gameObject.GetComponent<WorldObject>();
            if (ledge == null || ledge.objectType != WorldObjectType.LEDGE)
            {
                return;
            }
        }
        private void OnTriggerExit(Collider aOther)
        {
            WorldObject ledge = aOther.gameObject.GetComponent<WorldObject>();
            if (ledge == null || ledge.objectType != WorldObjectType.LEDGE)
            {
                return;
            }
        }
        protected override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                m_IsAtttached = !m_IsAtttached;
            }

            if(m_IsAtttached == true && m_Player != null)
            {
                m_Player.position = transform.position + transform.rotation * (m_MainCollider.center + new Vector3(m_PlayerOffset, 0.0f, 0.0f));
                //Vector3 direction = transform.position - m_Player.position;
                m_Player.rotation = transform.rotation;
            }

            if (Input.GetKey(KeyCode.Alpha1))
            {
                //check left
                m_LeftHit = true;

            }
            if (Input.GetKey(KeyCode.Alpha2))
            {
                //check right
                m_RightHit = true;
            }
        }

        private void FixedUpdate()
        {
            if(m_LeftHit)
            {
                //check left
                if(checkSide(m_BoundsLeft.center))
                {
                    m_Player.position = Vector3.Lerp(m_Player.position, m_Player.position + m_Direction, Time.fixedDeltaTime * m_MovementSpeed);
                    m_PlayerOffset = (m_Player.position - transform.position + m_MainCollider.center).normalized.magnitude;
                    
                }
                m_LeftHit = false;
            }
            if(m_RightHit)
            {
                //check right
                if(checkSide(m_BoundsRight.center))
                {
                    m_Player.position = Vector3.Lerp(m_Player.position, m_Player.position + m_Direction, Time.fixedDeltaTime * m_MovementSpeed);
                    //m_PlayerOffset = Vector3.Distance(m_Player.position, transform.position + m_MainCollider.center);
                    m_PlayerOffset = (m_Player.position - transform.position + m_MainCollider.center).normalized.magnitude;
                }
                m_RightHit = false;
            }
            
            if(Input.GetKeyDown(KeyCode.Alpha4))
            {
                if(m_Player != null)
                {
                    m_Player.position = new Vector3(Random.Range(0.0f, 12.0f), Random.Range(0.0f, 12.0f), Random.Range(0.0f, 12.0f));
                }
            }
        }

        private bool checkSide(Vector3 aOffset)
        {
            //When calculating the offsets I have to use negative values or else I get the origin and direction on the other side.
            //Calculate positions
            Vector3 mainColliderPosition = -m_MainCollider.center;
            Vector3 playerPosition = Vector3.zero;
            playerPosition.x = -m_PlayerOffset;
            playerPosition.z = -m_MainCollider.center.z;
            playerPosition = transform.position - transform.rotation * playerPosition;
            Vector3 sideColliderPosition = -aOffset;
            sideColliderPosition.y = 0.0f;
            sideColliderPosition = (transform.position - transform.rotation * sideColliderPosition);
            //Calculate origin and direction
            Vector3 origin = playerPosition;//transform.position - transform.rotation * mainOffset;
            Vector3 direction = (transform.position - transform.rotation * sideColliderPosition) - origin;

            direction.Normalize();
            // Vector3.Distance(m_Player.position, (transform.position - transform.rotation * sideOffset));

            int layerMask = 1 << GameManager.CLIMB_LAYER;
            //Debug
            m_Origin = origin;
            m_Direction = direction;

            RaycastHit hit;
            if(Physics.Raycast(origin,direction,out hit,m_Length, layerMask ))
            {
                //Cant Climb
                Debug.Log("Cant Climb");
                return false;
            }
            else
            {
                //Can Climb
                //Debug.Log("Can Climb");
                m_Player.position = Vector3.Lerp(m_Player.position, m_Player.position + direction, Time.deltaTime * m_MovementSpeed);

                //float playerDistance = Vector3.Distance(m_Player.position, transform.position - transform.rotation * mainOffset);
                ///float mainDistance = Vector3.Distance(transform.position - transform.rotation * mainOffset, transform.position - transform.rotation * mainOffset);
                //Debug.Log(playerDistance);
               // Debug.Log(mainDistance);
                //m_PlayerOffset = playerDistance - mainDistance;
                
                //Calc Distance between main point and Collider
                // m_Player.position = transform.position + transform.rotation * (m_MainCollider.center + new Vector3(m_PlayerOffset, 0.0f, 0.0f));
                
                //m_PlayerOffset = (m_Player.position - transform.position + m_MainCollider.center).normalized.magnitude;
                //Calc Distance between player and collider
            }
            return true;
        }

        

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(m_Origin, m_Origin + m_Direction * m_Length);

        }
    }
}