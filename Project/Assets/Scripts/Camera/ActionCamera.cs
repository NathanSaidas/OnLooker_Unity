using UnityEngine;
using System.Collections;

namespace EndevGame
{

    public class ActionCamera : MonoBehaviour
    {
        private enum CameraState
        {
            LOW,
            NORMAL,
            HIGH
        }
        [SerializeField]
        private float m_LowFOV = 45.0f;
        [SerializeField]
        private float m_NormalFOV = 60.0f;
        [SerializeField]
        private float m_HighFOV = 70.0f;
        [SerializeField]
        private float m_TransitionSpeed = 5.0f;
        [SerializeField]
        private CameraState m_State;

        [SerializeField]
        private Camera m_Camera = null;

        // Use this for initialization
        void Start()
        {
            m_Camera = GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            if (m_Camera != null)
            {
                switch (m_State)
                {
                    case CameraState.LOW:
                        m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, m_LowFOV, Time.deltaTime * m_TransitionSpeed);
                        break;
                    case CameraState.NORMAL:
                        m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, m_NormalFOV, Time.deltaTime * m_TransitionSpeed);
                        break;
                    case CameraState.HIGH:
                        m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, m_HighFOV, Time.deltaTime * m_TransitionSpeed);
                        break;
                }
            }
        }

        public void setLow()
        {
            m_State = CameraState.LOW;
        }
        public void setNormal()
        {
            m_State = CameraState.NORMAL;
        }
        public void setHigh()
        {
            m_State = CameraState.HIGH;
        }
    }

}