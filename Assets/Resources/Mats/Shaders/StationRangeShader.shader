// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Scraft/StationRangeShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
	}
		SubShader
		{
			Tags {"Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
			

			Pass
			{
				
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag 

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION; 
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed4 _Color;

				v2f vert(appdata v)
				{
					v2f o;
					float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					worldPos = float3(worldPos.x, 0.01f, worldPos.z);
					o.vertex = UnityWorldToClipPos(worldPos);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv);	
					clip(col.a - 0.5f);					
					return  col * _Color;
				}
				ENDCG
			}
		}
}
