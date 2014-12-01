Shader "Custom/Metals/Robot_Metal_Shader" 
{
	Properties 
	{
		_MainTex ("Texture", 2D) = "white" {}
		_NormalMap ("Normal Map", 2D) = "white"{}
		_EmissionMap ("Emission Map", 2D) = "white"{}
		_SpecularMap ("Specular Map", 2D) = "white"{}
		_GlossMap ("Gloss Map", 2D) = "white" {}
        _Gloss ("Gloss", Range(0.0,10.0)) = 0.5
		_Specular ("Specular", Float) = 0.0
		_Emission ("Emission", Range(0.0,1.0)) = 0.0
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		CGPROGRAM
		#pragma surface surf SimpleSpecular
		#pragma target 3.0
		

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_NormalMap;
			float2 uv_EmissionMap;
			float2 uv_SpecularMap;
			float2 uv_GlossMap;
		};
		
		sampler2D _MainTex;
		sampler2D _NormalMap;
		sampler2D _EmissionMap;
		sampler2D _SpecularMap;
		sampler2D _GlossMap;
		float _Gloss;
		float _Specular;
		float _Emission;
		
		float4 LightingSimpleSpecular(SurfaceOutput surfaceOutput, half3 lightDir, half3 viewDir, half atten)
		{
			float3 h = normalize (lightDir + viewDir);
		
	        float difference = max (0, dot (surfaceOutput.Normal, lightDir));
		
	        float nh = max (0, dot (surfaceOutput.Normal, h));
	        float specularPower = pow (nh, _Specular);
		
	        float4 result;
	        result.rgb = (surfaceOutput.Albedo * _LightColor0.rgb * difference + _LightColor0.rgb * surfaceOutput.Specular * surfaceOutput.Gloss) * (atten * 2) + surfaceOutput.Emission;
	        result.a = surfaceOutput.Alpha;
	        
	        return result;
		}
	
		void surf (Input IN, inout SurfaceOutput o) 
		{
			o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * 0.5f;
			o.Normal = UnpackNormal(tex2D(_NormalMap,IN.uv_NormalMap));
			o.Gloss = tex2D(_GlossMap,IN.uv_GlossMap).r * _Gloss;
			o.Specular = tex2D(_SpecularMap,IN.uv_SpecularMap).r;
			o.Emission = tex2D(_EmissionMap,IN.uv_EmissionMap).rgb * _Emission;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
