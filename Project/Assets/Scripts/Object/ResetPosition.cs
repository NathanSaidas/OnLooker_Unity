using UnityEngine;
using System.Collections;

namespace Gem
{

    public class ResetPosition : MonoBehaviour, IGameListener
    {

        private Vector3 m_OriginalPosition = Vector3.zero;

        // Use this for initialization
        void Start()
        {
            m_OriginalPosition = transform.position;
            Game.Register(this);
        }
        void OnDestroy()
        {
            Game.Unregister(this);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnGamePaused()
        {
            
        }

        public void OnGameUnpaused()
        {
            
        }

        public void OnGameReset()
        {
            transform.position = m_OriginalPosition;
        }
    }
}