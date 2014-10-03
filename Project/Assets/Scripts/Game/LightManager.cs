using UnityEngine;
using System.Collections;



public class LightManager : MonoBehaviour 
{
    private static LightManager s_Instance = null;
    public static LightManager instance
    {
        get { return s_Instance; }
    }


    [SerializeField]
    private Transform m_DirectionalLight = null;
	
    
    /// <summary>
    /// Creates the singleton instance for everyone to use.
    /// </summary>
	void Start () 
    {
        if (s_Instance == null)
        {
            s_Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of \'Light Manager\' are being instantiated.");
            Destroy(this);
            return;
        }
	}
    /// <summary>
    /// Removes the singleton instance this instance owns.
    /// </summary>
    void OnDestroy()
    {
        if(s_Instance == this)
        {
            s_Instance = null;
        }
    }

    public static Transform directionalLight
    {
        get { return instance == null ? null : instance.m_DirectionalLight; }
    }

        
}
