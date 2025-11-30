Shader "Scraft/Terrain/ColorEarthCurvature"
{
    Properties
    {        
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Normal Scale", Range(0, 2)) = 1.0
        // 注意：这里改为 xy 控制 Tiling，zw 可以留作他用或置0
        _BumpTiling ("Normal Tiling", Vector) = (1000, 1000, 0, 0)
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
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert fullforwardshadows vertex:vert
        #pragma target 3.0

        #include "UnityCG.cginc"

        struct Input
        {            
            float2 uv_BumpMap;
            float3 worldPos;
            INTERNAL_DATA 
        };

        sampler2D _BumpMap;
        half _BumpScale;
        float4 _BumpTiling;
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
            o.Albedo = _Color.rgb;
            o.Alpha = _Color.a;

            // 【修改点 1】：正确的法线混合逻辑
            if (_BumpScale > 0.0) 
            {                     

                // 3. 混合两个法线结果                
                fixed3 normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap *_BumpTiling.xy));   

                // 4. 应用强度缩放
                fixed3 finalNormal = normal;
                finalNormal.xy *= _BumpScale;
                finalNormal.z = sqrt(1.0 - saturate(dot(finalNormal.xy, finalNormal.xy)));

                // 【修改点 2】：直接赋值，不要手动乘矩阵
                // Surface Shader 会自动处理切线空间到世界空间的转换
                o.Normal = finalNormal;
            }
            
            // --- 下方声纳逻辑保持不变 ---
            if (_OpenSonar.x > 0 && IN.worldPos.y < -5)
            {
                const float _GridDensity = 0.1;   
                const float _GridThickness = 0.02;
                const float _GridBrightness = 1.5; 
                const fixed3 _GridColor = fixed3(0, 0.7, 1);

                const float _AttenuationStart = 50.0;
                const float _AttenuationMax = 1000.0;  

                const float _RingShowWidth = 200.0;   
                const float _RingHideWidth = 300.0;   
                const float _RingSmoothWidth = 50.0;  
                const float _ScanSpeed = 120.0;       

                float2 worldXZ = IN.worldPos.xz; 
                float2 gridUV = worldXZ * _GridDensity;
                float2 gridFrac = frac(gridUV);
                float xLine = step(_GridThickness, gridFrac.x) * step(_GridThickness, 1 - gridFrac.x);
                float zLine = step(_GridThickness, gridFrac.y) * step(_GridThickness, 1 - gridFrac.y);
                float gridIntensity = 1 - (xLine * zLine);

                float2 playerXZ = _PlayerPos.xz; 
                float distanceToPlayer = length(worldXZ - playerXZ);

                float attenuation = smoothstep(_AttenuationMax, _AttenuationStart, distanceToPlayer);
                attenuation = max(attenuation, 0.0);

                float scanOffset = _Time.y * _ScanSpeed;
                float relativeDist = max(scanOffset - distanceToPlayer , 0.0);
                float ringCycle = _RingShowWidth + _RingHideWidth;
                float cyclePos = fmod(relativeDist, ringCycle);

                float smoothStart = max(0.0, _RingShowWidth - _RingSmoothWidth);
                float smoothEnd = _RingShowWidth;                              
                float ringVisible = smoothstep(smoothEnd, smoothStart, cyclePos);

                o.Emission = _GridColor * _GridBrightness * gridIntensity * attenuation * ringVisible; 
            }
            else
            {
                o.Emission = fixed3(0, 0, 0);
            }
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