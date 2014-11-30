using UnityEngine;
using UnityEditor;
using System.Collections;

public class RenderCubemap : MonoBehaviour 
{
	[MenuItem("CONTEXT/RenderCubemap/Render Cubemap")]
	static private void OnRenderCubeMap(MenuCommand aCommand)
	{
		((RenderCubemap)aCommand.context).RenderCubeMap();
	}

    [SerializeField]
    private Cubemap m_Target = null;
    [SerializeField]
    private Camera m_Camera = null;
	
	private void RenderCubeMap()
	{
		m_Camera.RenderToCubemap(m_Target);
	}
	
	
}
