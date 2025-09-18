Shader "Scraft/SubmarineEmissionShader"
{
    Properties
    {
		_Specular("Specular", Color) = (1, 1, 1, 1)
		_Gloss("Gloss", Range(8.0, 256)) = 20
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }      

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
          

			#include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;   
				float3 normal : NORMAL;
				fixed3 color : COLOR;
            };

            struct v2f
            {          
                float4 pos : SV_POSITION;
				fixed3 color : COLOR;
            };
   
			fixed4 _Specular;
			float _Gloss;
			fixed _Open;

            v2f vert (appdata v)
            {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				if (_Open > 0) {
					o.color = v.color;
					return o;
				}

				fixed3 worldNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);

				fixed3 diffuse = _LightColor0.rgb * v.color * saturate(dot(worldNormal, worldLightDir));

				fixed3 reflectDir = normalize(reflect(-worldLightDir, worldNormal));
				fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz);
				fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(reflectDir, viewDir)), _Gloss);

				o.color = diffuse + specular;

				return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {                              
				return fixed4(i.color, 1.0f);
            }
            ENDCG
        }
    }
}
