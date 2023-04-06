Shader "Custom/Crystal"
{
	Properties
	{
		_Cube("Reflection Map", Cube) = "" {}
		_Color1("Color1", Color) = (0,0,0,1)
		_Color2("Color2", Color) = (0,0,0,1)
		_Emission("Emission", Float) = 0
	}
		SubShader
		{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
			#pragma surface surf Standard fullforwardshadows
			#pragma target 3.0

			samplerCUBE _Cube;
			float3 _Color1;
			float3 _Color2;
			float _Emission;

			struct Input {
				float2 uv_MainTex;
				float3 viewDir;
				float3 worldNormal;
			};

			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_INSTANCING_BUFFER_END(Props)

			void surf(Input IN, inout SurfaceOutputStandard o) {
				float3 r1 = texCUBE(_Cube, reflect(IN.viewDir, IN.worldNormal));
				float3 r2 = texCUBE(_Cube, reflect(IN.viewDir, IN.worldNormal));

				r1 *= _Color1;
				r2 *= _Color2;

				o.Albedo = lerp(r1, r2, abs(dot(IN.viewDir, IN.worldNormal)));
				o.Emission = o.Albedo * _Emission;

				o.Metallic = 0;
				o.Smoothness = 0;
				o.Alpha = 1;
			}
			ENDCG
		}
			FallBack "Diffuse"
}