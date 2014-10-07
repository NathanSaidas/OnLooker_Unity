using UnityEngine;
using System.Collections;

public class AnimationTester : MonoBehaviour 
{
    public Animation m_Animation = null;
    public int m_ClipIndex = 0;
    public bool m_Update = false;

    public AnimationClip[] m_Clip;
	// Use this for initialization
	void Start () 
    {
	    if(m_Clip == null || m_Animation == null)
        {
            return;
        }

        for(int i = 0; i < m_Clip.Length; i++)
        {
            m_Animation.AddClip(m_Clip[i], m_Clip[i].name);
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(m_Update == true)
        {
            m_Animation.CrossFade(m_Clip[m_ClipIndex].name,0.3f);
            m_Update = false;
        }
        m_Animation.CrossFade(m_Clip[m_ClipIndex].name, 0.3f);
	}

    
}
