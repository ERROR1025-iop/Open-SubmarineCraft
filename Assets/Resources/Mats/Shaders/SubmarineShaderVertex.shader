
Shader "Scraft/SubmarineShaderVertex" {
	Properties{
		_Luminou("Luminou", Color) = (1, 1, 1, 1)
		_Color("Color Tint", Color) = (1, 1, 1, 1)
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

		fixed4 _Color;
		fixed4 _Specular;
		float _Gloss;
		fixed3 _Luminou;

		struct a2v {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
		};

		struct v2f {
			float4 pos : SV_POSITION;
			float3 worldNormal : TEXCOORD0;
			float3 worldPos:TEXCOORD1;
		};

		v2f vert(a2v v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.worldNormal = UnityObjectToWorldNormal(v.normal);
			o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

			return o;
		}

		fixed4 frag(v2f i) : SV_Target {
			fixed3 worldNormal = normalize(i.worldNormal);
			fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);

			fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
			fixed3 luminou = _Luminou * clamp((-i.worldPos.y) * 0.1f, 0, 0.1f) * max(0, dot(worldNormal, fixed3(0,1,0)));
			fixed3 diffuse = _LightColor0.rgb * _Color.rgb * max(0, dot(worldNormal, worldLightDir));

			fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
			fixed3 halfDir = normalize(worldLightDir + viewDir);
			fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0, dot(worldNormal, halfDir)), _Gloss);
		
			return fixed4(ambient + luminou + (diffuse + specular), 1.0);
		}

		ENDCG
	}

	Pass {
			// Pass for other pixel lights
			Tags { "LightMode" = "ForwardAdd" }

			Blend One One

		CGPROGRAM

			// Apparently need to add this declaration
			#pragma multi_compile_fwdadd

			#pragma vertex vert
			#pragma fragment frag

			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			fixed4 _Color;
			fixed4 _Specular;
			float _Gloss;

			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				fixed3 color : COLOR;
			};

			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				fixed3 worldNormal = normalize(o.worldNormal);
				#ifdef USING_DIRECTIONAL_LIGHT
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
				#else
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz - o.worldPos.xyz);
				#endif

				fixed3 diffuse = _LightColor0.rgb * _Color.rgb * max(0, dot(worldNormal, worldLightDir));

				fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - o.worldPos.xyz);
				fixed3 halfDir = normalize(worldLightDir + viewDir);
				fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0, dot(worldNormal, halfDir)), _Gloss);
								
				o.color = fixed4((diffuse + specular), 1.0) - fixed4(0, 0, 0.18, 0);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target {

				return fixed4(i.color, 1.0);
			}
			
		ENDCG
	}		
}
FallBack "Specular"
}
