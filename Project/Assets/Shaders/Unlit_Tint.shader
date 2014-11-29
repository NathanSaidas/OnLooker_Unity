Shader "Custom/Unlit_Tint" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color", Color ) = (0.0, 0.0, 0.0, 0.0)
	}
	SubShader 
	{
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vertShader
			#pragma fragment fragShader

			sampler2D _MainTex;
			float4 _Color;

			struct attrib
			{
				float4 a_Position : POSITION0;
				float4 a_Color : COLOR0;
				float2 a_UV : TEXCOORD0;
			};


			attrib vertShader(attrib attribs)
			{
				attrib result;
				result.a_Position = mul(UNITY_MATRIX_MVP,attribs.a_Position);
				result.a_Color = attribs.a_Color;
				result.a_UV = attribs.a_UV;

				return result;
			}

			float4 fragShader(attrib attribs) : COLOR0
			{
				float4 result = tex2D(_MainTex, attribs.a_UV);
				return result * _Color;
			}
			
			ENDCG
		}
	}
}
