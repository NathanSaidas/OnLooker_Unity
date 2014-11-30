using UnityEngine;
using System.Collections;
namespace Gem
{

    /// <summary>
    /// Teaches the unit the given abilities upon event. 
    /// Events handled TRIGGER_AREA & UNIT_KILLED
    /// </summary>
    public class LearnAbility : MonoGameEventHandler
    {
        [SerializeField]
        private string m_EventName = string.Empty;
        [SerializeField]
        private GameEventID m_EventType = GameEventID.TRIGGER_AREA;
        [SerializeField]
        private Ability[] m_Abilities = null;
        [SerializeField]
        private string m_TargetName = string.Empty;
        private Unit m_Unit = null;
        // Use this for initialization
        void Start()
        {
            RegisterEvent(m_EventType);
            StartCoroutine(LateStart());
        }
        IEnumerator LateStart()
        {
            yield return new WaitForEndOfFrame();
            m_Unit = UnitManager.GetUnit(m_TargetName);
        }
        void OnDestroy()
        {
            UnregisterEvent(m_EventType);
        }

        protected override void OnGameEvent(GameEventID aEventType)
        {
            if(m_Unit == null || m_Abilities == null)
            {
                return;
            }
            if(aEventType == m_EventType)
            {
                switch(m_EventType)
                {
                    case GameEventID.TRIGGER_AREA:
                        {
                            AreaTrigger trigger = eventData.sender as AreaTrigger;
                            Unit unit = eventData.triggeringObject as Unit;
                            if (trigger != null && unit != null && unit == m_Unit && trigger.triggerName == m_EventName)
                            {
                                TeachAbilities(unit);
                            }
                        }
                        break;
                    case GameEventID.UNIT_KILLED:
                        {
                            Unit unit = eventData.sender as Unit;
                            if (unit != null && unit.unitName == m_EventName)
                            {
                                TeachAbilities(unit);
                            }
                        }
                        break;
                }
            }
        }

        private void TeachAbilities(Unit unit)
        {
            IEnumerator ability = m_Abilities.GetEnumerator();
            while (ability.MoveNext())
            {
                Ability current = ability.Current as Ability;
                if (current == null)
                {
                    continue;
                }
                DebugUtils.Log("Teaching " + unit.unitName + " ability " + current.abilityName);
                unit.AddAbility(current);
            }
            enabled = false;
        }
    }
}