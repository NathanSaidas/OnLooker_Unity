using UnityEngine;
using System.Collections.Generic;

namespace Gem
{
    /// <summary>
    /// The AI controller controls what behaviour to execute
    /// </summary>
    public class AIController : MonoBehaviour
    {
        /// <summary>
        /// The angle the AI can see its targets
        /// </summary>
        [Range(1.0f,360.0f)]
        [SerializeField]
        private float m_VisionAngle = 90.0f;
        /// <summary>
        /// The maximum distance a target can be in order to be seen by AI
        /// </summary>
        [SerializeField]
        private float m_MaxDistance = 30.0f;
        /// <summary>
        /// The minimum distance a target can be without being seen by the AI
        /// </summary>
        [SerializeField]
        private float m_MinDistance = 10.0f;
        /// <summary>
        /// The distance for correcting raycasting errors.
        /// </summary>
        [SerializeField]
        private float m_DistanceCorrection = 1.0f;
        /// <summary>
        /// How much time to wait inbetween searches. 
        /// (Lower = Higher Quality but decreased Performance)
        /// (Hower = Lower Quality but increased performance)
        /// </summary>
        [SerializeField]
        private float m_SearchTime = 1.0f;
        [SerializeField]
        private AIBehaviour m_DefaultBehaviour = null;
        [SerializeField]
        private AIBehaviour m_TargetAquiredBehaviour = null;

        /// <summary>
        /// Search timer. Searches only happen if there is no target
        /// </summary>
        private float m_TimeTillNextSearch = 0.0f;
        /// <summary>
        /// The mask this AIController will search for in a raycast.
        /// </summary>
        private int m_SearchLayerMask = Game.LAYER_PLAYER | Game.LAYER_UNIT | Game.LAYER_SURFACE;
        /// <summary>
        /// A list of possible targets
        /// </summary>
        private List<Unit> m_TargetList = new List<Unit>();
        /// <summary>
        /// The current target aquired.
        /// </summary>
        private Unit m_Target = null;
        /// <summary>
        /// The AI motor driving the behaviours / movement
        /// </summary>
        private AIMotor m_AIMotor = null;


        private Unit m_Unit = null;
        protected virtual void Start()
        {
            m_AIMotor = GetComponent<AIMotor>();
            m_Unit = GetComponent<Unit>();
        }

        /// <summary>
        /// Updates the AI motors behaviours based on the state of the AIController
        /// </summary>
        protected virtual void Update()
        {
            if(m_Target != null && m_TargetAquiredBehaviour != null)
            {
                Game.CanSee(m_Unit);
                m_TargetAquiredBehaviour.target = m_Target.transform;
                m_AIMotor.aiBehaviour = m_TargetAquiredBehaviour;
            }
            else
            {
                Game.CantSee(m_Unit);
                m_AIMotor.aiBehaviour = m_DefaultBehaviour;
            }
        }

        private void FixedUpdate()
        {
            m_TimeTillNextSearch -= Time.fixedDeltaTime;
            if(m_TimeTillNextSearch < 0.0f)
            {
                m_TimeTillNextSearch = m_SearchTime;
                SearchForUnits();
            }
        }

        /// <summary>
        /// Searches for all units
        /// </summary>
        private void SearchForUnits()
        {
            if(m_TargetAquiredBehaviour == null)
            {
                return;
            }
            //DebugUtils.Log("Searching for units");
            IEnumerable<Unit> units = UnitManager.GetAllUnits();
            if(units == null)
            {
                return;
            }
            IEnumerator<Unit> iter = units.GetEnumerator();
            m_TargetList.Clear();
            while(iter.MoveNext())
            {
                if(iter.Current == null)
                {
                    continue;
                }
                if(SearchCondition(iter.Current))
                {
                    m_TargetList.Add(iter.Current);
                }
            }

            int count = m_TargetList.Count;
            if(count > 0)
            {
                if(count > 1)
                {
                    m_TargetList.Sort(CompareUnitDistance);
                }
                m_Target = m_TargetList[0];
            }
            else
            {
                m_Target = null;
            }

        }
        
        /// <summary>
        /// The conditions a unit must meet in order for the AI to chase it
        /// This condition does a raycast to check for vision and checks the unit type to be a player
        /// </summary>
        /// <param name="aUnit">The unit being tested.</param>
        /// <returns></returns>
        protected virtual bool SearchCondition(Unit aUnit)
        {
            Vector3 direction = (aUnit.transform.position - transform.position).normalized;
            Vector3 origin = transform.position;
            float distance = Vector3.Distance(aUnit.transform.position, transform.position);
            ///Check if the unit is beyond max distance
            if(distance > m_MaxDistance)
            {
                return false;
            }
            ///Verify in line of sight and not in minimum distance range
            float angle = Vector3.Dot(direction, transform.forward);
            float cosAngle = Mathf.Cos(m_VisionAngle * 0.5f);
            if(angle < cosAngle && distance > m_MinDistance)
            {
                return false;
            }
            ///Raycast
            RaycastHit hit;
            if (!Physics.Raycast(origin, direction, out hit, distance + m_DistanceCorrection, m_SearchLayerMask))
            {
                //DebugUtils.LogWarning("Raycast search for unit Missed");
                return false; //Missed??
            }
            //DebugUtils.Log("Hit" + hit.collider.name + "\nLayer = " + (1 << hit.collider.gameObject.layer));
            if((1 << hit.collider.gameObject.layer) == Game.LAYER_SURFACE)
            {
                return false;
            }
            ///Verify unit type
            return aUnit.unitType == UnitType.PLAYER;
        }

        /// <summary>
        /// Compare method to compare unit distance
        /// </summary>
        /// <param name="A">Unit A</param>
        /// <param name="B">Unit B</param>
        /// <returns></returns>
        protected int CompareUnitDistance(Unit A, Unit B)
        {
            float distanceA = Vector3.Distance(transform.position, A.transform.position);
            float distanceB = Vector3.Distance(transform.position, B.transform.position);

            if(distanceA < distanceB)
            {
                return -1;
            }
            else if(distanceA == distanceB)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        
    }
}