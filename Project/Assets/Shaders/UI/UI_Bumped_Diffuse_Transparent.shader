Shader "Custom/UI/UI_Bumped_Diffuse_Transparent"
{
	//CHANGE LOG
	// October,27,2014 - Nathan Hanlan, Added and implemented UI_Bumped_Diffuse_Transparent Shader
	//END CHANGE LOG
	Properties 
	{
		_Texture ("Texture", 2D) = "white" {}
		_NormalMap ("Normal Map", 2D) = "white" {}
		_Color ("Color", Color) = (1.0,1.0,1.0,1.0)
	}
	SubShader 
	{
		Tags{"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		CGPROGRAM
		#pragma surface surf Lambert alpha

		sampler2D _Texture;
		sampler2D _NormalMap;
		float4 _Color;
		float _TileX;
		float _TileY;
		
		struct Input 
		{
			float2 uv_Texture;
			float2 uv_NormalMap;
		};

		void surf (Input aData, inout SurfaceOutput data) 
		{
			float4 col = tex2D (_Texture, aData.uv_Texture + float2(_TileX,_TileY)) * _Color;
			data.Albedo = col.rgb;
			data.Alpha = col.a;
			data.Normal = UnpackNormal(tex2D(_NormalMap,aData.uv_NormalMap));
		}
		ENDCG
	} 
}