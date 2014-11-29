Shader "Custom/UI/UI_Diffuse_Transparent"
{
	Properties 
	{
		_Texture ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1.0,1.0,1.0,1.0)
	}
	SubShader 
	{
		Tags{"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		CGPROGRAM
		#pragma surface surf Lambert alpha

		sampler2D _Texture;
		float4 _Color;
		float _TileX;
		float _TileY;
		
		struct Input 
		{
			float2 uv_Texture;
		};

		void surf (Input aData, inout SurfaceOutput data) 
		{
			float4 col = tex2D (_Texture, aData.uv_Texture + float2(_TileX,_TileY)) * _Color;
			data.Albedo = col.rgb;
			data.Alpha = col.a;
		}
		ENDCG
	} 
}