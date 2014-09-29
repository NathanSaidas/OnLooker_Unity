using System;
using UnityEngine;

[Serializable]
public class NativeObject 
{
    [SerializeField]
    private string m_Name;
    public virtual string name
    {
        get { return m_Name; }
        set { m_Name = value; }
    }
}
