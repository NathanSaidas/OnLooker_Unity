using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

public class RenderCubemap : MonoBehaviour 
{
#if UNITY_EDITOR
	[MenuItem("CONTEXT/RenderCubemap/Render Cubemap")]
	static private void OnRenderCubeMap(MenuCommand aCommand)
	{
		((RenderCubemap)aCommand.context).RenderCubeMap();
	}
#endif

    [SerializeField]
    private Cubemap m_Target = null;
    [SerializeField]
    private Camera m_Camera = null;
	
	private void RenderCubeMap()
	{
		m_Camera.RenderToCubemap(m_Target);
	}
	
	
}
