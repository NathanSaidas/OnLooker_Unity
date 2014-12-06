using UnityEngine;
using System.Collections;

namespace Gem
{

    public class PlayerDeath : MonoGameEventHandler
    {


        public string playerName = "Player_Nathan";


        private Unit unit = null;

        private void Start()
        {
            RegisterEvent(GameEventID.UNIT_KILLED);
        }
        private void Destroy()
        {
            UnregisterEvent(GameEventID.UNIT_KILLED);
        }

        protected override void OnGameEvent(GameEventID aEventType)
        {
            if (aEventType == GameEventID.UNIT_KILLED)
            {
                Unit sender = eventData.sender as Unit;
                if (sender != null && sender.unitName == playerName)
                {
                    unit = sender;
                    StartCoroutine(Revive());
                }
                    
            }
        }

        IEnumerator Revive()
        {
            yield return new WaitForSeconds(0.5f);
            unit.Revive();
            unit.transform.position = transform.position;
        }
    }
}
