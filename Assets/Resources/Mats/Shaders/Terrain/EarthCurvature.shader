Shader "Scraft/Terrain/EarthCurvature"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Normal Scale", Range(0, 2)) = 1.0
        // 注意：这里改为 xy 控制 Tiling，zw 可以留作他用或置0
        _BumpMinTiling ("Normal Min Tiling (Close)", Vector) = (1000, 1000, 0, 0)
        _BumpMaxTiling ("Normal Max Tiling (Far)", Vector) = (100, 100, 0, 0)
        _BumpMaxDistance ("Normal Max Distance", Float) = 100.0
        _BumpTransitionRange ("Transition Range", Float) = 500.0
        _Color ("Diffuse Color", Color) = (1,1,1,1)
        
        // _PlayerPos 由脚本 SetGlobalVector 控制
        _Radius ("Effect Radius", Float) = 18.0 
        _MaxDrop ("Max Drop", Float) = 2.0
        _Power ("Drop Curve Power", Range(0.1, 5)) = 1.0
        _NoDropRange ("No Drop Range (Player Distance)", Float) = 300.0
        _Sonar("SonarColor", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        Cull Off
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert fullforwardshadows vertex:vert
        #pragma target 3.0

        #include "UnityCG.cginc"
        #include "Assets/Resources/Mats/Shaders/Terrain/Sonar.cginc"

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float3 worldPos;
            INTERNAL_DATA 
        };

        sampler2D _MainTex;
        sampler2D _BumpMap;
        half _BumpScale;
        float4 _BumpMinTiling;
        float4 _BumpMaxTiling;
        float _BumpTransitionRange;
        float _BumpMaxDistance;
        float4 _Color;
        
        float4 _PlayerPos;
        float _Radius;
        float _MaxDrop;
        float _Power;
        float _NoDropRange;
        float4 _OpenSonar;

        // --- 核心修改区域 ---
        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 texColor = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = texColor.rgb * _Color.rgb;
            o.Alpha = texColor.a;

            // 【修改点 1】：正确的法线混合逻辑
            if (_BumpScale > 0.0) 
            {
                // 计算距离和插值系数 t
                float2 playerXZ = _PlayerPos.xz;
                float2 worldXZ = IN.worldPos.xz;
                float distanceToPlayer = length(worldXZ - playerXZ);
                
                // 计算线性插值权重 (0 = 近, 1 = 远)
                float t = saturate((distanceToPlayer - _BumpMaxDistance) / _BumpTransitionRange);

                // 【核心修复】：采样两次，而不是插值 UV
                // 1. 近处采样（高频细节）
                float2 uvClose = IN.uv_BumpMap * _BumpMinTiling.xy;
                fixed3 normalClose = UnpackNormal(tex2D(_BumpMap, uvClose));

                // 2. 远处采样（低频细节）
                float2 uvFar = IN.uv_BumpMap * _BumpMaxTiling.xy;
                fixed3 normalFar = UnpackNormal(tex2D(_BumpMap, uvFar));

                // 3. 混合两个法线结果
                fixed3 finalNormal = lerp(normalClose, normalFar, t);

                // 4. 应用强度缩放
                finalNormal.xy *= _BumpScale;
                finalNormal.z = sqrt(1.0 - saturate(dot(finalNormal.xy, finalNormal.xy)));

                // 【修改点 2】：直接赋值，不要手动乘矩阵
                // Surface Shader 会自动处理切线空间到世界空间的转换
                o.Normal = finalNormal;
            }
            
            // Sonar: use reusable function from Sonar.cginc
            o.Emission = ComputeSonarEmission(IN.worldPos, _PlayerPos, _OpenSonar);
        }

        void vert (inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            float dx = worldPos.x - _PlayerPos.x;
            float dz = worldPos.z - _PlayerPos.z;
            float dist = length(float2(dx, dz));

            float t = 0.0;
            if (dist > _NoDropRange)
            {
                float effectiveDist = dist - _NoDropRange; 
                effectiveDist = max(effectiveDist, 0.0);   
                t = pow(effectiveDist / _Radius, _Power); 
            }

            worldPos.y -= t * _MaxDrop;
            v.vertex = mul(unity_WorldToObject, float4(worldPos, 1.0));
            o.worldPos = worldPos;
        }
        ENDCG
    }
    FallBack "Diffuse"
}