Shader "Scraft/LightBeamShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_NoiseTex("NoiseTexure", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
		Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        Pass
        {
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag         

            #include "UnityCG.cginc"

            struct a2v
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;               
                float4 vertex : SV_POSITION;
				float3 worldPos:TEXCOORD1;
            };

            sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _NoiseTex;
			fixed3 _Color;

            v2f vert (a2v v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				if (i.worldPos.y > 0) {
					discard;
				}

				fixed4 noise = tex2D(_NoiseTex, i.uv + _Time.y * float2(-0.1, -0.1));
				float2 offsetUV = float2(noise.g, noise.g);
				offsetUV = (offsetUV - 0.5) * 2 * 0.1 * cos(i.worldPos.y + i.worldPos.x);
                fixed4 col = tex2D(_MainTex, i.uv + offsetUV);
				fixed4 render = fixed4(_Color, col.g * 0.05);

                return render;
            }
            ENDCG
        }
    }
}
