using UnityEngine;
using System.Collections;

namespace Gem
{

    public class AreaTrigger : MonoBehaviour
    {
        [SerializeField]
        private string m_TriggerName = string.Empty;

        private void OnTriggerEnter(Collider aCollider)
        {
            Unit unit = aCollider.GetComponent<Unit>();
            if(unit == null)
            {
                unit = aCollider.GetComponentInChildren<Unit>();
            }
            if(unit == null)
            {
                unit = aCollider.GetComponentInParent<Unit>();
            }
            if (unit != null)
            {
                Debug.Log("Invoking Event");
                GameEventManager.InvokeEvent(new GameEventData(Time.time,
                    GameEventID.TRIGGER_AREA,
                    GameEventType.TRIGGER,
                    this,
                    unit));
            }
        }
        
        public string triggerName
        {
            get { return m_TriggerName; }
            set { m_TriggerName = value; }
        }

    }
}