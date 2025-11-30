Shader "Scraft/TerrainItems/UnderWaterEmissionShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MaxDistance ("Max Distance", Float) = 10.0
    }
    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        float3 _PlayerPos;
        float _MaxDistance;

        struct Input
        {
            float3 worldPos;
        };

        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Calculate distance to player
            float d = distance(_PlayerPos, IN.worldPos);
            
            // Calculate emission strength based on distance
            float strength = saturate(1.0 - d / _MaxDistance);
            
            // Apply emission
            o.Emission = _Color.rgb * strength;
            
            // Make object unlit (no albedo, metallic or smoothness)
            o.Albedo = float3(0,0,0);
            o.Metallic = 0;
            o.Smoothness = 0;
            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
