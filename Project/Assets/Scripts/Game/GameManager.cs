using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
    //Layer Constants
    public const int OBJECT_INTERACTION_LAYER = 8;
    public const int SURFACE_LAYER = 10;
    public const int PLANT_LIGHT_LAYER = SURFACE_LAYER;


    private static GameManager s_Instance = null;
    public static GameManager instance
    {
        get { return s_Instance; }
    }

    /// <summary>
    /// Debug Variables
    /// </summary>
    private Vector2 m_DebugScrollPosition = Vector2.zero;
    private Rect m_DebugRect = new Rect(0.0f, 0.0f, 600.0f, 800.0f);
    private List<IDebugDraw> m_DebugDraw = new List<IDebugDraw>();



	// Use this for initialization
	void Start () 
    {
        Debug.Log("Start Game Manager");
	    if(s_Instance == null)
        {
            s_Instance = this;
        }
        else
        {
            Debug.LogWarning("GameManager already exists.");
            Destroy(this);
            return;
        }
	}

    void OnDestroy()
    {
        if(s_Instance == this)
        {
            s_Instance = null;
        }
    }
	
	// Update is called once per frame
	void Update () 
    {
	
	}


    public void registerDebugDraw(IDebugDraw aDebugDraw)
    {
        if(aDebugDraw != null && m_DebugDraw.Contains(aDebugDraw) == false)
        {
            m_DebugDraw.Add(aDebugDraw);
        }
    }

    public void unregisterDebugDraw(IDebugDraw aDebugDraw)
    {
        if(aDebugDraw != null)
        {
            m_DebugDraw.Remove(aDebugDraw);
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(m_DebugRect);
        
        m_DebugScrollPosition = GUILayout.BeginScrollView(m_DebugScrollPosition);


        List<IDebugDraw>.Enumerator enumerator = m_DebugDraw.GetEnumerator();

        while(enumerator.MoveNext())
        {
            if(enumerator.Current == null)
            {
                m_DebugDraw.Remove(enumerator.Current);
                continue;
            }
            enumerator.Current.debugWatch();
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void OnDrawGizmos()
    {
        List<IDebugDraw>.Enumerator enumerator = m_DebugDraw.GetEnumerator();
        while (enumerator.MoveNext())
        {
            enumerator.Current.debugDraw();
        }
    }
}
