Shader "Universal Render Pipeline/Nature/SpeedTree8" {
	Properties {
		_MainTex ("Base (RGB) Transparency (A)", 2D) = "white" {}
		_Color ("Color", Vector) = (1,1,1,1)
		[Toggle(EFFECT_HUE_VARIATION)] _HueVariationKwToggle ("Hue Variation", Float) = 0
		_HueVariationColor ("Hue Variation Color", Vector) = (1,0.5,0,0.1)
		[Toggle(EFFECT_BUMP)] _NormalMapKwToggle ("Normal Mapping", Float) = 0
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_ExtraTex ("Smoothness (R), Metallic (G), AO (B)", 2D) = "(0.5, 0.0, 1.0)" {}
		_Glossiness ("Smoothness", Range(0, 1)) = 0.5
		_Metallic ("Metallic", Range(0, 1)) = 0
		[Toggle(EFFECT_SUBSURFACE)] _SubsurfaceKwToggle ("Subsurface", Float) = 0
		_SubsurfaceTex ("Subsurface (RGB)", 2D) = "white" {}
		_SubsurfaceColor ("Subsurface Color", Vector) = (1,1,1,1)
		_SubsurfaceIndirect ("Subsurface Indirect", Range(0, 1)) = 0.25
		[Toggle(EFFECT_BILLBOARD)] _BillboardKwToggle ("Billboard", Float) = 0
		_BillboardShadowFade ("Billboard Shadow Fade", Range(0, 1)) = 0.5
		[Enum(No,2,Yes,0)] _TwoSided ("Two Sided", Float) = 2
		[KeywordEnum(None,Fastest,Fast,Better,Best,Palm)] _WindQuality ("Wind Quality", Range(0, 5)) = 0
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "Hidden/Universal Render Pipeline/FallbackError"
	//CustomEditor "SpeedTree8ShaderGUI"
}