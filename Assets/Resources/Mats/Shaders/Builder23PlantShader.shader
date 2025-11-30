Shader "Custom/Builder23PlantShader"
{
    Properties
    {
        _Color ("Main Color", Color) = (0, 0, 0, 1)  // 默认黑色
        _Alpha ("Transparency", Range(0.0, 1.0)) = 0.5  // 透明度控制
    }
    SubShader
    {
        Tags 
        { 
            "Queue" = "Transparent" 
            "RenderType" = "Transparent" 
            "IgnoreProjector" = "True"
        }

        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off 
        Cull Back

        // 注意：Surface Shader 不需要手动写 Pass！
        // Unity 会自动生成正确的 Pass

        CGPROGRAM
        #pragma surface surf Lambert alpha:fade

        fixed4 _Color;
        float _Alpha;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            o.Albedo = _Color.rgb;    // 黑色
            o.Alpha = _Alpha;         // 透明度
        }
        ENDCG
    }
    FallBack "Transparent/VertexLit"
}