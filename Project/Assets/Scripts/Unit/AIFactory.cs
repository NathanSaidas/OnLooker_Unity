using UnityEngine;
using System.Collections;

public class AIFactory : MonoBehaviour
{
    

    [SerializeField]
    private GameObject m_UnitSpawned = null;
    [SerializeField]
    private float m_SpawnRate = 5.0f;

    private bool m_IsSpawning = false;

	// Use this for initialization
	void Start () 
    {
	    
	}


	
	IEnumerator Spawner()
    {
        while(m_IsSpawning)
        {
            yield return new WaitForSeconds(m_SpawnRate);
            SpawnUnit();
        }
    }

    private void SpawnUnit()
    {

    }
}
