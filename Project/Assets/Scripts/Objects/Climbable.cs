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


        /// <summary>
        /// Checks the left and right sides of the climable object
        /// </summary>
        /// <param name="aCharacter">The position / rotation to use for calculation</param>
        /// <param name="aDistance">How far does the character need to move? (Min/Max 1.1f/300.0f)</param>
        /// <param name="aDirection">The direction to check </param>
        /// <param name="aTarget">A returned variable with the target position to go to</param>
        /// <returns>True if can move, false if there is a collider blocking the way</returns>
        public bool checkSide(Transform aCharacter, float aDistance, Direction aDirection, out Vector3 aTarget)
        {
            aTarget = Vector3.zero;
            if(aCharacter == null)
            {
                return false;
            }
            //Set the origin / direction
            Vector3 origin = aCharacter.position;
            Vector3 direction = Vector3.zero;

            //Recalculate direction
            switch(aDirection)
            {
                case Direction.LEFT:
                    direction = aCharacter.rotation * Vector3.left;
                    break;
                case Direction.RIGHT:
                    direction = aCharacter.rotation * Vector3.right;
                    break;
                case Direction.UP:
                    direction = aCharacter.rotation * Vector3.up;
                    break;
                case Direction.DOWN:
                    direction = aCharacter.rotation * Vector3.down;
                    break;
            }
            direction.Normalize();
            float distance = Mathf.Clamp(aDistance, 1.1f, 300.0f);
            aTarget = origin + direction * distance;
            
            int layerMask = 1 << GameManager.CLIMB_LAYER;
            RaycastHit hit;

            if(aDirection == Direction.UP)
            {
                aTarget.y = aCharacter.position.y + distance;
                origin.y = aCharacter.position.y + m_TriggeringCharacter.characterHeight; 
            }

            if(Physics.Raycast(origin,direction,out hit, distance,layerMask))
            {
                aTarget = hit.point;
                if(aDirection == Direction.UP)
                {
                    aTarget.y -= m_TriggeringCharacter.characterHeight;
                }
                return false;
            }
            return true;
        }
    }

}