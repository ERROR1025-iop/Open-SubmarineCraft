Shader "Scraft/SubmarineTransparentShader"
{
	Properties
	{		
		_Luminou("Luminou", Color) = (1, 1, 1, 1)
	}
		SubShader
		{
			Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}

			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag 

				#include "Lighting.cginc"

				fixed4 _Specular;
				float _Gloss;
				fixed3 _Luminou;

				struct appdata
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					fixed3 color : COLOR;
				};

				struct v2f
				{					
					float3 worldNormal : TEXCOORD0;
					float4 vertex : SV_POSITION;	
					float3 worldPos:TEXCOORD1;
					fixed3 color : COLOR;
				};				

				v2f vert(appdata v)
				{
					v2f o;					
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.worldNormal = UnityObjectToWorldNormal(v.normal);
					o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					o.color = v.color;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed3 worldNormal = normalize(i.worldNormal);
					fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
					
					fixed3 luminou = _Luminou * clamp((-i.worldPos.y) * 0.1f, 0, 0.1f) * max(0, dot(worldNormal, fixed3(0,1,0)));
					fixed3 diffuse = _LightColor0.rgb * i.color * max(0, dot(worldNormal, worldLightDir));

					fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
					fixed3 halfDir = normalize(worldLightDir + viewDir);
					fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0, dot(worldNormal, halfDir)), _Gloss);
										
					return fixed4(luminou + (diffuse + specular), 0.2f);					
				}
				ENDCG
			}
		}
}
