using UnityEngine;
using System.Collections;

public class BatchingUtil : MonoBehaviour 
{
    [SerializeField]
    GameObject m_Root = null;

    [SerializeField]
    GameObject[] m_Targets = null;
	// Use this for initialization
	void Start () 
    {
        StaticBatchingUtility.Combine(m_Targets, m_Root);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
