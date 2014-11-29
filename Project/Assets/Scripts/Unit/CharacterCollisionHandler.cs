using UnityEngine;
using System.Collections;

namespace Gem
{
public class CharacterCollisionHandler : MonoBehaviour 
{
    [SerializeField]
    private CharacterMotor m_Character = null;
	// Use this for initialization
	void Start () 
    {
        if (m_Character != null)
        {
            m_Character.collisionHandler = this;
        }
	}
	
	void OnCollisionStay(Collision aCollision)
    {
        if(m_Character != null)
        {
            m_Character.OnCollisionStay(aCollision);
        }
    }
}
}