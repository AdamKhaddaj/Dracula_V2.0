Shader "Custom/Water"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex + float2(cos(_Time.x), sin(_Time.x))) * _Color;
			c += 0.5 * tex2D(_MainTex, IN.uv_MainTex + float2(cos(_Time.x), 0.75 * -sin(_Time.x))) * _Color;

			o.Albedo = c.rgb;
			o.Metallic = 1;
			o.Smoothness = 1;
			o.Alpha = c.a;
		}
		ENDCG
	}
		FallBack "Diffuse"
}