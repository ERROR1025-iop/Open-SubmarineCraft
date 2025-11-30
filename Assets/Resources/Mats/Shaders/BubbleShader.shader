// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Scraft/BubbleShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
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
					float3 worldPos: TEXCOORD1;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed3 _Color;
				float _CameraSeaLevel;

				v2f vert(appdata v)
				{
					v2f o;
					o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);					 
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					if (_WorldSpaceCameraPos.y > _CameraSeaLevel) {
						discard;
					}
					fixed4 col = tex2D(_MainTex, i.uv);				
					return col;
				}
				ENDCG
			}
		}
}
