Shader "Scraft/SubmarineNormalShader"
{
    Properties
    {
        _Emission("Emission", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf BlinnPhong fullforwardshadows 

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct Input
        {
            float3 vColor: COLOR;
			float3 worldPos;
			float3 worldNormal;
        };

        fixed4 _Emission;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input i, inout SurfaceOutput o)
        {           
			fixed3 emission = _Emission * clamp((-i.worldPos.y) * 0.1f, 0, 0.1f) * max(0, dot(i.worldNormal, fixed3(0, 1, 0)));
			o.Emission = emission;
			o.Albedo = i.vColor.xyz;
            o.Alpha = 1;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
