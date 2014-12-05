using UnityEngine;
using System.Collections;


namespace Gem
{
    public class Interactive : MonoBehaviour
    {
        private float m_UseTime = 0.0f;
        private float m_CurrentUseTime = 0.0f;
        private bool m_OneShot = true;

        private bool m_InUse = false;
        private CharacterAction m_User = null;

        public void Use(CharacterAction aActor)
        {
            if(m_OneShot)
            {
                m_User = aActor;
                OnUse();
            }
            else if(m_UseTime <= 0.0f)
            {
                m_User = aActor;
                m_InUse = true;
                OnUse();
            }
        }
        public void UseContinue()
        {
            if (m_OneShot == true)
            {
                return;
            }
            m_CurrentUseTime += Time.deltaTime;
            if(m_CurrentUseTime > m_UseTime && m_InUse == false)
            {
                OnUse();
            }
            OnUseContinue();
        }
        public void UseContinueEnd()
        {
            OnUseContinueEnd();
            m_CurrentUseTime = 0.0f;
        }
        public void StopUsing()
        {
            m_CurrentUseTime = 0.0f;
            m_InUse = false;
            OnStopUsing();
        }

        public virtual void OnUse()
        {

        }
        public virtual void OnUseContinue()
        {
            
        }
        public virtual void OnUseContinueEnd()
        {
            
        }
        public virtual void OnStopUsing()
        {
            
        }

        public bool CanUse(Transform aPlayer)
        {
            if(aPlayer != null)
            {
                Vector3 direction = transform.position - aPlayer.position;
                direction.Normalize();
                float angle = Vector3.Dot(direction, aPlayer.forward);
                return angle > 0.0f;
            }
            return false;
        }

        public float useTime
        {
            get { return m_UseTime; }
            set { m_UseTime = value; }
        }
        public float currentUseTime
        {
            get { return m_CurrentUseTime; }
        }
        public bool inUse
        {
            get { return m_InUse; }
            set { m_InUse = value; }
        }
        public CharacterAction user
        {
            get { return m_User; }
        }
    }

}