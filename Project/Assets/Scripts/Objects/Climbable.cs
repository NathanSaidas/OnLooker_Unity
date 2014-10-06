using UnityEngine;
using System.Collections;


namespace EndevGame
{
    

    public class Climbable : Interactive
    {
        [SerializeField]
        private BoxCollider m_MainCollider = null;
        [SerializeField]
        private BoxCollider m_BoundsLeft = null;
        [SerializeField]
        private BoxCollider m_BoundsRight = null;
        [SerializeField]
        private BoxCollider m_BoundsTop = null;
        [SerializeField]
        private BoxCollider m_BoundsBottom = null;

        [SerializeField]
        private Climbable m_LeftNeighbour = null;
        [SerializeField]
        private Climbable m_RightNeighbour = null;
        [SerializeField]
        private Climbable m_TopNeighbour = null;
        [SerializeField]
        private Climbable m_BottomNeighbour = null;


        [SerializeField]
        private float m_GrabCooldown = 0.5f;
        [SerializeField]
        private float m_CurrentTime = 0.0f;


        [SerializeField]
        private CharacterClimbing m_TriggeringCharacter = null;
        [SerializeField]
        private bool m_InUse = false;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            m_CurrentTime -= Time.deltaTime;
            if(m_CurrentTime < 0.0f && m_InUse == false)
            {
                m_TriggeringCharacter = null;
            }
        }

        /// <summary>
        /// Tell the character to start climbing.
        /// </summary>
        /// <param name="aPlayer"></param>
        public override void onUse(CharacterInteraction aPlayer)
        {
           if(aPlayer == null || aPlayer.characterClimbing == null || m_InUse == true)
           {
               return;
           }
           Debug.Log("Start Climbing");
           CharacterClimbing triggeringPlayer = aPlayer.characterClimbing;
           triggeringPlayer.startClimbing(this);

        }
        public override void onUseEnd(CharacterInteraction aPlayer, out bool aOverride)
        {
            aOverride = true;
        }

        /// <summary>
        /// The condition to use for trying to use this object.
        /// Only use this object if the player is facing the climbable surface and this is not currently being used.
        /// </summary>
        /// <param name="aCharacter"></param>
        /// <param name="aOther"></param>
        /// <returns></returns>
        public override bool condition(Transform aCharacter, Transform aOther)
        {
            Vector3 objectInFront = aOther.position;
            Vector3 objectView = aCharacter.position;
            Vector3 direction = (objectInFront - objectView).normalized;

            

            float facing = Vector3.Dot(direction, aCharacter.forward);
            Debug.Log("Character was facing: " + facing);
            if (facing <= 0.0f || m_TriggeringCharacter != null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets called by the character to let the climable surface know its being used
        /// </summary>
        /// <param name="aCharacter"></param>
        /// <returns></returns>
        public bool grab(CharacterClimbing aCharacter)
        {
            if(m_TriggeringCharacter == null)
            {
                m_TriggeringCharacter = aCharacter;
                m_InUse = true;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Gets called by the character to let the climbable surface know its not longer being used.
        /// </summary>
        /// <param name="aCharacter"></param>
        public void release(CharacterClimbing aCharacter)
        {
            m_CurrentTime = m_GrabCooldown;
            m_InUse = false;
        }

        public bool checkVerticalSide()
        {
            return false;
        }
        public bool checkHorizontalSide(Transform aCharacter, float aDistance, bool aLeft, out Vector3 aTarget)
        {
            aTarget = Vector3.zero;
            if(aCharacter == null)
            {
                return false;
            }

            Vector3 origin = aCharacter.position;
            Vector3 direction = Vector3.zero;

            if(aLeft == true)
            {
                direction = aCharacter.rotation * Vector3.left;
            }
            else
            {
                direction = aCharacter.rotation * Vector3.right;
            }
            direction.Normalize();
            float distance = Mathf.Clamp(aDistance, 1.1f, 300.0f);
            Debug.Log("Distance: " + distance);
            aTarget = origin + direction * distance;
            aTarget.y = origin.y;
            int layerMask = 1 << GameManager.CLIMB_LAYER;
            RaycastHit hit;
            if(Physics.Raycast(origin,direction,out hit, distance,layerMask))
            {
                aTarget = hit.point;
                

                return false;
            }

            return true;
        }
    }

}