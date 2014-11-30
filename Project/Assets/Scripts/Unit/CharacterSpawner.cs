using UnityEngine;
using System.Collections;

namespace Gem
{
    public class CharacterSpawner : MonoBehaviour
    {
        [SerializeField]
        GameObject m_PlayerPrefab = null;
        [SerializeField]
        GameObject m_PlayerUI = null;

        void Start()
        {
            Instantiate(m_PlayerPrefab, transform.position, transform.rotation);
            GameObject go = (GameObject)Instantiate(m_PlayerUI, transform.position, transform.rotation);
            UIBar healthBar = go.GetComponent<UIBar>();
        }


#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.position, Vector3.one);
            Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        }
#endif
    }
}