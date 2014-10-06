﻿using UnityEngine;
using System.Collections;

namespace EndevGame
{

    public class LedgeGrab : EndevBehaviour
    {
        [SerializeField]
        private BoxCollider m_MainCollider = null;
        [SerializeField]
        private BoxCollider m_BoundsLeft = null;
        [SerializeField]
        private BoxCollider m_BoundsRight = null;

        [SerializeField]
        private LedgeGrab m_LeftNeighbour = null;
        [SerializeField]
        private LedgeGrab m_RightNeighbour = null;


        [SerializeField]
        private float m_LedgeGrabCooldown = 0.5f;
        [SerializeField]
        private float m_CurrentTime = 0.0f;

        [SerializeField]
        CharacterLedgeGrab m_TriggeringCharacter = null;
        [SerializeField]
        private bool m_InUse = false;

        protected override void Update()
        {
            base.Update();
            //Reduce time by delta time to 
            //float prevTime = m_CurrentTime;
            //m_CurrentTime -= Time.deltaTime;
            ////if the new time is less than 0 but the previous was greater than. Reset the collider to not be atrigger.
            //if(m_CurrentTime <= 0.0f && prevTime > 0.0f)
            //{
            //    if(m_MainCollider != null)
            //    {
            //        m_MainCollider.isTrigger = false;
            //    }
            //}
            m_CurrentTime -= Time.deltaTime;
            if(m_CurrentTime < 0.0f && m_InUse == false)
            {
                m_TriggeringCharacter = null;
            }
        }

        private void FixedUpdate()
        {

        }

        /// <summary>
        /// Invoke this function to have a character or something grab onto a ledge.
        /// </summary>
        public bool grab(CharacterLedgeGrab aCharacter)
        {


            //If there is no one currently attached the ledge (One Character only)
            if(m_TriggeringCharacter == null)
            {
                Vector3 objectInFront = aCharacter.manager.transform.position;
                Vector3 objectView = transform.position;
                Vector3 direction = (objectInFront - objectView).normalized;
                float facing = Vector3.Dot(direction, transform.forward);
                if(facing <= 0.0f)
                {
                    Debug.Log("Character was behind");
                    return false;
                }

                m_TriggeringCharacter = aCharacter;
                m_InUse = true;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Invoke this function to have a character or something release themselves from the ledge.
        /// </summary>
        public void release(CharacterLedgeGrab aCharacter)
        {
            //m_CurrentTime = m_ResetTime;
            m_CurrentTime = m_LedgeGrabCooldown;
            m_InUse = false;            
        }

        /// <summary>
        /// Checks the left side of the left based on the characters position
        /// </summary>
        /// <param name="aCharacter">The character thats moving.</param>
        /// <param name="aDistance">The distance that the character can move</param>
        /// <returns></returns>
        public bool checkSide(Transform aCharacter, float aDistance, bool aLeft, out Vector3 aTarget)
        {
            aTarget = Vector3.zero;
            if(aCharacter == null)
            {
                return false;
            }
            Vector3 origin = aCharacter.position;
            Vector3 direction = Vector3.zero;
            if (aLeft == true)
            {
                direction = aCharacter.rotation * Vector3.left;
            }
            else
            {
                direction = aCharacter.rotation * Vector3.right;
            }
            direction.Normalize();
            float distance = Mathf.Clamp(aDistance,1.1f,300.0f);
            Debug.Log("Distance: " + distance);
            aTarget = origin + direction * distance;
            aTarget.y = origin.y;
            int layerMask = 1 << GameManager.CLIMB_LAYER;
            RaycastHit hit;
            origin.y += m_TriggeringCharacter.characterHeight;
            if(Physics.Raycast(origin,direction,out hit,distance,layerMask ))
            {
                aTarget = hit.point;
                aTarget.y -= m_TriggeringCharacter.characterHeight;
                return false;
            }
            
            return true;
        }

        

        /*private bool checkSide(Vector3 aOffset)
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
        }*/



        private void OnDrawGizmos()
        {

        }


        public Vector3 origin
        {
            get { return m_MainCollider == null ? Vector3.zero : m_MainCollider.center; }
        }
        public Vector3 size
        {
            get { return m_MainCollider == null ? Vector3.zero : m_MainCollider.size; }
        }
    }
}