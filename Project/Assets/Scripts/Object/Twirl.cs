using UnityEngine;
using System.Collections;

namespace Gem
{

    public class Twirl : MonoBehaviour
    {

        [SerializeField]
        private Vector3 m_Axis = Vector3.up;
        [SerializeField]
        private Vector3 m_Speed = Vector3.zero;

        private Quaternion m_StartRotation = Quaternion.identity;

        // Use this for initialization
        void Start()
        {
            m_StartRotation = transform.rotation;
        }
        void OnDisable()
        {
            transform.rotation = m_StartRotation;
        }
        // Update is called once per frame
        void Update()
        {
            Vector3 rotation = m_Axis;
            rotation.Scale(m_Speed * Time.deltaTime);
            transform.Rotate(rotation);
        }

    }
}