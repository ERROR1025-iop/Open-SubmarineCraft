// Upgrade NOTE: replaced '_LightMatrix0' with 'unity_WorldToLight'
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Scraft/WaterUpShader" {
	Properties{
		_Color("Color", Color) = (1, 1, 1, 1)
		_ViewOffset("ViewOffset", Color) = (1, 1, 1, 1)
		_MainTex("Base (RGB)", 2D) = "" {}
		_Diffuse("Diffuse", Color) = (1, 1, 1, 1)
		_Specular("Specular", Color) = (1, 1, 1, 1)
		_Gloss("Gloss", Range(8.0, 256)) = 20
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }

			Pass {
		// Pass for ambient light & first pixel light (directional light)
		Tags { "LightMode" = "ForwardBase" }

		CGPROGRAM

		// Apparently need to add this declaration 
		#pragma multi_compile_fwdbase	 

		#pragma vertex vert
		#pragma fragment frag

		#include "Lighting.cginc"

		sampler2D _MainTex;
		fixed4 _Diffuse;
		fixed4 _Specular;
		float _Gloss;
		fixed4 _Color;
		fixed3 _ViewOffset;

		struct a2v {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
		};

		struct v2f {
			float4 pos : SV_POSITION;		
			float3 worldPos : TEXCOORD1;
		};

		v2f vert(a2v v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

			return o;
		}

		fixed4 frag(v2f i) : SV_Target {
			fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
			fixed3 worldNormal = UnpackNormal(tex2D(_MainTex, i.worldPos.xz *  0.05f  + _Time.y * float2(0.02, 0)));
			float2 reflUV = worldNormal.xz * (viewDir.xyz + _ViewOffset);
			worldNormal = UnpackNormal(tex2D(_MainTex, reflUV + _Time.y * float2(0.01, -0.01)));

			fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
			fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
			fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * max(0, dot(worldNormal, worldLightDir));

			fixed3 halfDir = normalize(worldLightDir + viewDir);
			fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0, dot(worldNormal, halfDir)), _Gloss);

			fixed atten = 1.0;

			fixed4 render = fixed4(ambient + (diffuse + specular) * atten, 1.0);
			return render + _Color;
		}

			ENDCG
	}
		
	}
		FallBack "Specular"
}
