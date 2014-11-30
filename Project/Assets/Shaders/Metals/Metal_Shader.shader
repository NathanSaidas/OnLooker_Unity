Shader "Custom/Metals/Metal_Shader" 
{
	Properties 
	{
		_MainTex ("Texture", 2D) = "white" {}
        _Gloss ("Gloss", Range(0.0,10.0)) = 0.5
		_Specular ("Specular", Float) = 0.0
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
		};
		
		sampler2D _MainTex;
		float _Gloss;
		float _Specular;
		
		float4 LightingSimpleSpecular(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		{
			half3 h = normalize (lightDir + viewDir);
		
	        half diff = max (0, dot (s.Normal, lightDir));
		
	        float nh = max (0, dot (s.Normal, h));
	        float spec = pow (nh, 48.0);
		
	        half4 c;
	        c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec * s.Gloss) * (atten * 2);
	        c.a = s.Alpha;
	        
	        return c;
		}
	
		void surf (Input IN, inout SurfaceOutput o) 
		{
			o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * 0.5;
			o.Gloss = _Gloss;
			o.Specular = _Specular;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
