Shader "Custom/UI/UI_Unlit" 
{
	//CHANGE LOG
	// October,27,2014 - Nathan Hanlan, Added and implemented UI_Unlit Shader
	//END CHANGE LOG
	Properties 
	{
		_Texture ("Texture" , 2D) = "white" {}
		_Color ("Color", Color ) = (1.0,1.0,1.0,1.0)
	}
	SubShader 
	{
		Tags{"RenderType"="Opaque"}
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
			return _Color * tex2D(_Texture,aData.mTexCoord + float2(_TileX,_TileY));
		}

		
		ENDCG
		}
	} 
}
