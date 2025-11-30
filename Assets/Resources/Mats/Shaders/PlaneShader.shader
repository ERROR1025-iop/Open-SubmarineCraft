Shader "Scraft/PlaneShader"
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

			fixed4 _Luminou;

			struct appdata
			{
				float4 vertex : POSITION;				
			};

			struct v2f
			{				
				float4 vertex : SV_POSITION;					
			};

			v2f vert(appdata v)
			{
				v2f o;				
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target 
			{				
				return _Luminou;
			}
			ENDCG
		}
	}
}
