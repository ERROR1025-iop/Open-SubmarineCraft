// Sonar.cginc
// Reusable sonar visualization logic extracted from EarthCurvature.shader

fixed3 ComputeSonarEmission(float3 worldPos, float4 playerPos, float4 openSonar)
{
    if (openSonar.x <= 0 || worldPos.y >= -5)
        return fixed3(0, 0, 0);

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

    float2 worldXZ = worldPos.xz; 
    float2 gridUV = worldXZ * _GridDensity;
    float2 gridFrac = frac(gridUV);
    float xLine = step(_GridThickness, gridFrac.x) * step(_GridThickness, 1 - gridFrac.x);
    float zLine = step(_GridThickness, gridFrac.y) * step(_GridThickness, 1 - gridFrac.y);
    float gridIntensity = 1 - (xLine * zLine);

    float2 playerXZ = playerPos.xz; 
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

    return _GridColor * _GridBrightness * gridIntensity * attenuation * ringVisible; 
}
