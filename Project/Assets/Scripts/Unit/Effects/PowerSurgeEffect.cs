using UnityEngine;
using System.Collections;

namespace Gem
{
    [RequireComponent(typeof(SphereCollider),typeof(Rigidbody))]
    public class PowerSurgeEffect : MonoBehaviour
    {
        private static readonly WaitForSeconds LIFE_TIME = new WaitForSeconds(30.0f);

        [SerializeField]
        private float m_Speed = 5.0f;
        [SerializeField]
        private float m_LifeTime = 15.0f;
        [SerializeField]
        private float m_ExplosionTimer = 1.0f;
        [SerializeField]
        private float m_ExplosionRadius = 5.0f;


        private float m_Damage = 0.0f;
        Vector3 m_Direction = Vector3.zero;
        private Unit m_Owner = null;
        private float m_CurrentTime = 0.0f;
        private bool m_IsExploding = false;
        // Use this for initialization
        void Start()
        {
            m_Direction = transform.forward;
            m_CurrentTime = m_LifeTime;

            SphereCollider sCollider = GetComponent<SphereCollider>();
            if (sCollider != null)
            {
                sCollider.isTrigger = true;
            }
            if (rigidbody != null)
            {
                rigidbody.useGravity = false;
                rigidbody.isKinematic = true;
            }

            //ParticleSystem[] systems = GetComponentsInChildren<ParticleSystem>();
            StartCoroutine(LifeTimer());
        }

        // Update is called once per frame
        void Update()
        {
            transform.position += m_Direction * m_Speed * Time.deltaTime;
        }
        void OnTriggerEnter(Collider aCollider)
        {
            if(m_IsExploding)
            {
                return;
            }
            bool killEffect = false;
            Unit unit = aCollider.GetComponent<Unit>();
            if(unit != null)
            {
                if(unit != m_Owner)
                {
                    killEffect = true;
                }
            }
            else if(aCollider.gameObject.layer == Game.LAYER_SURFACE)
            {
                killEffect = true;
            }
            
            if(killEffect == true)
            {
                StartCoroutine(DeathExplosion());
            }
        }

        void OnTriggerStay(Collider aCollider)
        {
            Unit unit = aCollider.GetComponent<Unit>();
            if(unit != null && unit != m_Owner)
            {
                unit.ReceiveDamage(m_Damage);
            }        
        }

        IEnumerator DeathExplosion()
        {
            //TODO: Explosion Particle Effect
            yield return new WaitForSeconds(m_ExplosionTimer);
            //TODO: Sphere cast and deal damage to units in area.
            Destroy(gameObject);
        }
        IEnumerator LifeTimer()
        {
            yield return LIFE_TIME;
            Destroy(gameObject);
        }

        public float speed
        {
            get { return m_Speed; }
            set { m_Speed = value; }
        }
        public float lifeTime
        {
            get { return m_LifeTime; }
            set { m_LifeTime = value; }
        }
        public float explosionTimer
        {
            get { return m_ExplosionTimer; }
            set { m_ExplosionTimer = value; }
        }
        public float explosionRadius
        {
            get { return m_ExplosionRadius; }
            set { m_ExplosionRadius = value; }
        }

        public float damage
        {
            get { return m_Damage; }
            set { m_Damage = value; }
        }
        public Vector3 direction
        {
            get { return m_Direction; }
        }
        public Unit owner
        {
            get { return m_Owner; }
            set { m_Owner = value; }
        }
        public float currentTime
        {
            get { return m_CurrentTime; }
        }
        public bool isExploding
        {
            get { return m_IsExploding; }
        }
    }
}