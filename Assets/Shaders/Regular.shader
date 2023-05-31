Shader "Toony/Regular"
{
	Properties
	{
		//TOONY COLORS
		_Color ("Color", Color) = (0.5,0.5,0.5,1.0)
		_SColor ("Shadow Color", Color) = (0.3,0.3,0.3,1.0)
		
		//DIFFUSE
		_MainTex ("Main Texture (RGB)", 2D) = "white" {}
		
		//TOONY COLORS RAMP
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}

		[Header(Specular)]
		_SpecularIntensity ("Specular Intensity", Range(0, 64)) = 16
		_SpecularAlpha ("Specular Alpha", Range(0, 1)) = 0.66666
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		
		#pragma surface surf ToonyColorsCustom fullforwardshadows
		//#pragma target 2.0
		//#pragma glsl
		
		//================================================================
		// VARIABLES
		
		fixed4 _Color;
		sampler2D _MainTex;
		
		
		struct Input
		{
			half2 uv_MainTex;
		};
		
		//================================================================
		// CUSTOM LIGHTING
		
		//Lighting-related variables
		fixed4 _SColor;
		sampler2D _Ramp;
		
		//Custom SurfaceOutput
		struct SurfaceOutputCustom
		{
			fixed3 Albedo;
			fixed3 Normal;
			fixed3 Emission;
			half Specular;
			fixed Alpha;
		};

		float _SpecularIntensity;
		float _SpecularAlpha;
		
		inline half4 LightingToonyColorsCustom (SurfaceOutputCustom s, half3 lightDir, half3 viewDir, half atten)
		{
			s.Normal = normalize(s.Normal);
			fixed ndl = max(0, dot(s.Normal, lightDir)*0.5 + 0.5);
			
			fixed3 ramp = tex2D(_Ramp, fixed2(ndl,ndl));

			float3 halfVector = normalize(_WorldSpaceLightPos0 + normalize(viewDir));
			float NdotH = saturate(dot(s.Normal, halfVector));
			float specularIntensity = pow(NdotH * ramp, _SpecularIntensity);
			float3 specular = specularIntensity * float3(1, 1, 1) * _SpecularAlpha;
			//Reflection

			// #if !(POINT) && !(SPOT)
			ramp *= atten;
			// #endif

			_SColor = lerp(fixed4(1, 1, 1, 1), _SColor, _SColor.a);	//Shadows intensity through alpha
			ramp = lerp(_SColor.rgb, fixed3(1, 1, 1), ramp);
			fixed4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * ramp + specular;
			c.a = s.Alpha;

			// #if (POINT || SPOT)
			c.rgb *= atten;
			// #endif

			return c;
		}
		
		
		//================================================================
		// SURFACE FUNCTION
		
		void surf (Input IN, inout SurfaceOutputCustom o)
		{
			fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
			
			o.Albedo = mainTex.rgb * _Color.rgb;
			o.Alpha = mainTex.a * _Color.a;	
		}
		
		ENDCG
	}
	
	Fallback "Diffuse"
}
