Shader "Scraft/MenuBackgroundImageShader" {

		Properties{
		_MainTex("Base (RGB)", 2D) = "" {}	
		_DistortionMap("DistortionMap (RGB)", 2D) = "" {}		
		}

		SubShader{
			Pass {
				ZTest Always Cull Off ZWrite Off

				CGPROGRAM
				#pragma vertex vert  
				#pragma fragment frag  

				#include "UnityCG.cginc"  

				sampler2D _MainTex;
				sampler2D _DistortionMap;				
				fixed4 _ColorMask; 

				struct v2f {
					float4 pos : SV_POSITION;
					half2 uv: TEXCOORD0;				
				};

				v2f vert(appdata_img v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = v.texcoord;				
					return o;
				}

				fixed4 frag(v2f i) : SV_Target {
				
					float4 disTex = tex2D(_DistortionMap, i.uv + _Time.y * fixed2(0.02, 0.02));
					float2 offsetUV = float2(disTex.r, disTex.g);
					offsetUV = (offsetUV - 0.5) * 2 * 0.03;					
					fixed4 renderTex = tex2D(_MainTex, i.uv + offsetUV);
					renderTex = renderTex + _ColorMask;

					return renderTex;
			}

			ENDCG
		}
		}

			Fallback Off
}
