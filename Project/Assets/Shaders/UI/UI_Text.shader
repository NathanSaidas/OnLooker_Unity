Shader "Custom/UI/UI_Text" 
{

	//CHANGE LOG
	// November,13,2014 - Nathan Hanlan, Added and implemented UI Text Shader
	//END CHANGE LOG
	Properties 
	{
		_Texture ("Texture" , 2D) = "white" {}
		_Color ("Color", Color ) = (1.0,1.0,1.0,1.0)
	}
	SubShader 
	{
		Tags { 
		"Queue" = "Transparent" 
		"IgnoreProjector" = "True" 
		"RenderType"="Transparent" }
		
		Lighting Off Cull Off ZWrite Off Fog {Mode Off}
		Blend SrcAlpha OneMinusSrcAlpha
		
		Tags { 
		"Queue" = "Transparent" 
		"IgnoreProjector" = "True" 
		"RenderType"="Transparent" }
		
		Pass
		{
		CGPROGRAM
		#pragma vertex vertShader
		#pragma fragment fragShader	
	
			sampler2D _Texture;
			float4 _Color;
			float _TileX;
			float _TileY;
			
			struct VertData
			{
				float4 mPos : POSITION0;
				float2 mTexCoord : TEXCOORD0;
			};
			struct FragData
			{
				float4 mPos : POSITION0;
				float2 mTexCoord : TEXCOORD0;
			};
			
			FragData vertShader (VertData aData)
			{
				FragData data;
				data.mPos = mul(UNITY_MATRIX_MVP,aData.mPos);
				data.mTexCoord = aData.mTexCoord;
				return data;
			}
			
			float4 fragShader(FragData aData) : COLOR0
			{
				return _Color + tex2D(_Texture,aData.mTexCoord);
			}
		ENDCG
		}
	} 
	FallBack "Diffuse"
}
