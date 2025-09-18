Shader "Scraft/UnderwaterShader" {

		Properties{
			_MainTex("Base (RGB)", 2D) = "" {}				
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
				sampler2D _CameraDepthTexture;
				fixed4 _ColorMask;
				fixed2 _OpenSonar;


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

					if (_WorldSpaceCameraPos.y > 0) {
						return tex2D(_MainTex, i.uv);
					}
					
					float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
					float linearDepth = LinearEyeDepth(depth);
					float linear01Depth = Linear01Depth(depth);
					if (linearDepth < 1) {
						return tex2D(_MainTex, i.uv) + _ColorMask;
					}			

					float4 disTex = tex2D(_DistortionMap, i.uv + _Time.y * fixed2(0.02, 0.02));
					float2 offsetUV = float2(disTex.r, disTex.g);
					offsetUV = (offsetUV - 0.5) * 2 * 0.015;					
					fixed4 renderTex = tex2D(_MainTex, i.uv + offsetUV);
					renderTex = renderTex + _ColorMask;			
									
					if (_OpenSonar.x > 0 && linear01Depth < 1) {
						fixed4 _Sonar = fixed4(0, 0, linear01Depth * 0.5, 1);
						float offset = (1 - fmod(_Time.x, 1.0f)) * 2000;
						float startLine = fmod(linearDepth + offset, 100);//扫描频率
						float gradual = startLine * 0.008;						
						fixed4 radar = step(startLine, 100) * gradual * _Sonar;
						return renderTex + radar;
					}

					return renderTex;

			}

			ENDCG
		}
		}

			Fallback Off
}
