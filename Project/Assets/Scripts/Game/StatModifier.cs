using UnityEngine;
using System;
using System.Collections;


[Serializable]
public class StatModifier : EndevGame.Object
{
    public StatModifier() : base()
    {

    }
    public StatModifier(float aAdditive, float aMultiplicative) : base()
    {
        m_Additive = aAdditive;
        m_Multiplicative = aMultiplicative;
    }

    [SerializeField]
    private float m_Additive = 0.0f;
    [SerializeField]
    private float m_Multiplicative = 0.0f;


    public float add(float aValue)
    {
        return (aValue + m_Additive) * m_Multiplicative;
    }
    public float multiply(float aValue)
    {
        return (aValue * m_Multiplicative) + m_Additive;
    }

    public float additive
    {
        get { return m_Additive; }
        set { m_Additive = value; }
    }
    public float multiplcative
    {
        get { return m_Multiplicative; }
        set { m_Multiplicative = value; }
    }
}