using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Gem
{
    public class GameLoseEvent : MonoGameEventHandler
    {
        [SerializeField]
        private Transform m_SpawnPoint = null;
        [SerializeField]
        private float m_RespawnDelay = 1.5f;
        private Unit m_PlayerToRespawn = null;

        private void Start()
        {
            RegisterEvent(GameEventID.UNIT_KILLED);
        }

        private void OnDestroy()
        {
            UnregisterEvent(GameEventID.UNIT_KILLED);
        }


        protected override void OnGameEvent(GameEventID aEventType)
        {
            Debug.Log(aEventType);
            switch(aEventType)
            {
                case GameEventID.UNIT_KILLED:
                    {
                        Unit dyingUnit = eventData.sender as Unit;
                        if(dyingUnit != null)
                        {
                            if(dyingUnit.unitType == UnitType.PLAYER)
                            {
                                Respawn(dyingUnit);
                            }
                        }
                    }
                    break;
            }
        }

        public void Respawn(Unit aPlayer)
        {
            aPlayer.transform.position = m_SpawnPoint.position;
            m_PlayerToRespawn = aPlayer;
            StartCoroutine(RespawnDelay());
        }

        private IEnumerator RespawnDelay()
        {
            yield return new WaitForSeconds(m_RespawnDelay);
            IEnumerator<Unit> iter = UnitManager.GetAllUnits().GetEnumerator();
            while(iter.MoveNext())
            {
                iter.Current.Revive();
            }
            Game.ResetGame();


            
        }
        
    }
}
