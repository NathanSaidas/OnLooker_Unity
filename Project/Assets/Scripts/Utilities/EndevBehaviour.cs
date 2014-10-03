using UnityEngine;
using System.Collections;

public class EndevBehaviour : MonoBehaviour 
{
    /// <summary>
    /// This is how much time must pass to call slow update
    /// </summary>
    private static float s_SlowUpdateTime = 1.0f;

    /// <summary>
    /// This is the time variable that is kept track. (Prefixed EB for EndevBehaviour as well as to not use up the name.
    /// </summary>
    private float m_EBCurrentUpdateTime = 0.0f;
    


    /// <summary>
    /// Invoke this function to update the slow update timer.
    /// </summary>
    protected virtual void Update()
    {
        m_EBCurrentUpdateTime += Time.deltaTime;
        if (m_EBCurrentUpdateTime >= s_SlowUpdateTime)
        {
            SlowUpdate();
            m_EBCurrentUpdateTime = 0.0f;
        }
    }

    /// <summary>
    /// This method gets called every time the 'm_Time' is greater than 'Slow Update Time'
    /// </summary>
    protected virtual void SlowUpdate()
    {
        ///TOOD: Implement method
    }

    /// <summary>
    /// Accessor to the slow update time static member.
    /// </summary>
    public static float slowUpdateTime
    {
        get { return s_SlowUpdateTime; }
        set { s_SlowUpdateTime = value; }
    }


    /// <summary>
    /// This is a helper function not built into older versions of unity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T getComponentInParent<T>() where T : Component
    {
        Transform parent = transform.parent;
        while(parent != null)
        {
            T component = parent.GetComponent<T>();
            if(component != null)
            {
                return component;
            }
            parent = parent.parent;
        }
        return null;
    }
    /// <summary>
    /// This is an identical function as to the one above.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="aIterations">The maximum amount of iteratoins to search for</param>
    /// <returns></returns>
    public T getComponentInParent<T>(int aIterations) where T : Component
    {
        Transform parent = transform.parent;
        for (int i = 0; i < aIterations && parent != null; i++)
        {
            T component = parent.GetComponent<T>();
            if (component != null)
            {
                return component;
            }
            parent = parent.parent;
        }
        return null;
    }

	
}
