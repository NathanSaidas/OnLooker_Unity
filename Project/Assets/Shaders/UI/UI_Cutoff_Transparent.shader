Shader "Custom/UI/UI_Cutoff_Transparent"
{
	//CHANGE LOG
	// november,29,2014 - Nathan Hanlan, Added and implemented UI_Unlit_Transparent Shader
	//END CHANGE LOG
	Properties 
	{
		_Texture ("Texture" , 2D) = "white" {}
		_Color ("Color", Color ) = (1.0,1.0,1.0,1.0)
		_CutoffX ("Cutoff X", Range(0.0,1.0)) = 1.0
		_CutoffY ("Cutoff Y", Range(0.0,1.0)) = 1.0
	}
	SubShader 
	{
		Tags{"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		Pass
		{
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		CGPROGRAM
		#pragma vertex vertShader
		#pragma fragment fragShader	
	
		sampler2D _Texture;
		float4 _Color;
		float _TileX;
		float _TileY;
		float _CutoffX;
		float _CutoffY;
		
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
			float2 uvCoords = aData.mTexCoord;
			float4 finalColor = _Color * tex2D(_Texture,uvCoords + float2(_TileX,_TileY));
			if(uvCoords.x > _CutoffX || uvCoords.y > _CutoffY)
			{
				finalColor.a = 0.0;
			}

			return finalColor;
		}

		
		ENDCG
		}
	} 
}