using UnityEngine;
using System.Collections;

/// <summary>
/// This enum describes the types of objects within the world.
/// </summary>
public enum WorldObjectType
{
    LEDGE,
}
    

public class WorldObject : MonoBehaviour 
{
    [SerializeField]
    private WorldObjectType m_ObjectType = WorldObjectType.LEDGE;

    public WorldObjectType objectType
    {
        get { return m_ObjectType; }
    }
}
