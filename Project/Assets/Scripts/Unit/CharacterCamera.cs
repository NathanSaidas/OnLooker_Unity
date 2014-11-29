using UnityEngine;
using System.Collections;
namespace Gem
{
    public enum CameraShakeMode
    {
        DEFAULT,
        INCREASE,
        DECREASE
    }
    public class CharacterCamera : MonoBehaviour
    {
        [SerializeField]
        private Transform m_Position = null;
        [SerializeField]
        private Transform m_XTransform = null;
        [SerializeField]
        private Transform m_YTransform = null;
        [SerializeField]
        private bool m_Invert = false;
        [SerializeField]
        private float m_HeighOffset = 0.5f;


        [SerializeField]
        private Vector3 m_ShakeMagnitude = Vector3.zero;
        private CameraShakeMode m_ShakeMode = CameraShakeMode.DEFAULT;
        private float m_ShakeTimeBegin = 0.0f;
        private float m_ShakeTimeLeft = 0.0f;

        [SerializeField]
        private float m_SwaySpeed = 1.0f;
        [SerializeField]
        private float m_SwayTarget = 0.5f;
        [SerializeField]
        private float m_CurrentSwayAmount = 0.0f;
        private float m_CurrentSwayTime = 0.0f;
        private bool m_IsSwaying = false;

        // Use this for initialization
        void Start()
        {

        }
        void Update()
        {
            //if(Input.GetKeyDown(KeyCode.Alpha2))
            //{
            //    ShakeCamera(5.0f);
            //}
            //if(Input.GetKeyDown(KeyCode.Alpha3))
            //{
            //    ShakeCamera(5.0f, CameraShakeMode.INCREASE);
            //}
            //if(Input.GetKeyDown(KeyCode.Alpha4))
            //{
            //    ShakeCamera(5.0f, CameraShakeMode.DECREASE);
            //}
            
            
        }
        // Update is called once per frame
        void LateUpdate()
        {
            if(m_Position != null)
            {
                Vector3 position = m_Position.position;
                position.y += m_HeighOffset;
                transform.position = position;
            }
            if(m_XTransform != null && m_YTransform != null)
            {
                float xRot = m_XTransform.transform.localRotation.eulerAngles.x;
                float yRot = m_YTransform.transform.rotation.eulerAngles.y;
                if(m_Invert)
                {
                    xRot = -xRot;
                }
                transform.rotation = Quaternion.Euler(xRot, yRot, 0.0f);
            }

            m_ShakeTimeLeft -= Time.deltaTime;
            if(m_ShakeTimeLeft > 0.0f)
            {
                CameraShake();
            }
            UpdateCameraSway();
        }

        public void ShakeCamera(float aTime)
        {
            m_ShakeTimeBegin = aTime;
            m_ShakeTimeLeft = aTime;
            m_ShakeMode = CameraShakeMode.DEFAULT;
        }
        public void ShakeCamera(float aTime, CameraShakeMode aShakeMode)
        {
            m_ShakeTimeBegin = aTime;
            m_ShakeTimeLeft = aTime;
            m_ShakeMode = aShakeMode;
        }

        public void ShakeCamera(float aTime, Vector3 aMagnitude)
        {
            if (aMagnitude.LessThan(m_ShakeMagnitude) && m_ShakeTimeLeft > 0.0f)
            {
                return;
            }
            m_ShakeTimeBegin = aTime;
            m_ShakeTimeLeft = aTime;
            m_ShakeMagnitude = aMagnitude;
            m_ShakeMode = CameraShakeMode.DEFAULT;
        }
        public void ShakeCamera(float aTime, CameraShakeMode aShakeMode, Vector3 aMagnitude)
        {
            if(aMagnitude.LessThan(m_ShakeMagnitude) && m_ShakeTimeLeft > 0.0f)
            {
                return;
            }
            m_ShakeTimeBegin = aTime;
            m_ShakeTimeLeft = aTime;
            m_ShakeMode = aShakeMode;
            m_ShakeMagnitude = aMagnitude;
        }

        private void CameraShake()
        {
            Vector3 offset = new Vector3(Random.Range(-m_ShakeMagnitude.x, m_ShakeMagnitude.x),
                Random.Range(-m_ShakeMagnitude.y, m_ShakeMagnitude.y),
                Random.Range(-m_ShakeMagnitude.z, m_ShakeMagnitude.z));


            switch(m_ShakeMode)
            {
                case CameraShakeMode.INCREASE:
                    {
                        float fallOff = 1- (m_ShakeTimeLeft / m_ShakeTimeBegin);
                        offset *= fallOff;
                    }
                    break;
                case CameraShakeMode.DECREASE:
                    {
                        float fallOff = (m_ShakeTimeLeft / m_ShakeTimeBegin);
                        offset *= fallOff;
                    }
                    break;
            }

            transform.position += transform.rotation * offset;
        }
        private void UpdateCameraSway()
        {
            if (m_IsSwaying == false)
            {
                m_CurrentSwayTime = 0.0f;
                m_CurrentSwayAmount = Mathf.Lerp(m_CurrentSwayAmount, 0.0f, Time.deltaTime);
                if (Mathf.Abs(m_CurrentSwayAmount) < 0.01f)
                {
                    m_CurrentSwayAmount = 0.0f;
                }
            }
            else
            {
                m_CurrentSwayTime += Time.deltaTime * m_SwaySpeed;
                if (m_CurrentSwayTime > 1.0f)
                {
                    m_SwayTarget = -m_SwayTarget;
                    m_CurrentSwayTime = 0.0f;
                }
                m_CurrentSwayAmount = Mathf.Lerp(-m_SwayTarget, m_SwayTarget,m_CurrentSwayTime);
            }
            transform.position += transform.rotation * new Vector3(m_CurrentSwayAmount, m_CurrentSwayAmount * 0.5f, 0.0f);
        }

        public Transform positionCam
        {
            get { return m_Position; }
            set { m_Position = value; }
        }
        public Transform xRotCam
        {
            get { return m_XTransform; }
            set { m_XTransform = value; }
        }
        public Transform yRotCam
        {
            get { return m_YTransform; }
            set { m_YTransform = value; }
        }
        public float swaySpeed
        {
            get { return m_SwaySpeed; }
            set { m_SwaySpeed = value; }
        }
        public float swayTarget
        {
            get { return m_SwayTarget; }
        }
        public bool isSwaying
        {
            get { return m_IsSwaying; }
            set { m_IsSwaying = value; }
        }
    }
}