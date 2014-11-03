Shader "Custom/UI/UI_Diffuse" 
{
	Properties 
	{
		_Texture ("Texture", 2D) = "white" {}
		//_NormalMap ("Normal Map", 2D) = "white" {}
		_Color ("Color", Color) = (1.0,1.0,1.0,1.0)
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		CGPROGRAM
		#pragma surface surf Lambert 

		sampler2D _Texture;
		//sampler2D _NormalMap;
		float4 _Color;
		float _TileX;
		float _TileY;
		
		struct Input 
		{
			float2 uv_Texture;
			//float2 uv_NormalMap;
		};

		void surf (Input aData, inout SurfaceOutput data) 
		{
			float2 tileOffset = float2(_TileX,_TileY);
			float4 col = tex2D (_Texture, aData.uv_Texture + tileOffset) * _Color;
			data.Albedo = col.rgb;
			data.Alpha = 1.0;
			//Get Normals this way
			//data.Normal = UnpackNormal(tex2D(_NormalMap,aData.uv_NormalMap));
		}
		ENDCG
	} 
}
