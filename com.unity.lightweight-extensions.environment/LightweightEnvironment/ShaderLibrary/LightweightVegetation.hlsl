#include "LWRP/ShaderLibrary/Lighting.hlsl"

// ------------------------------
// Defines

#define UNITY_USE_SHCOEFFS_ARRAYS 1

// ------------------------------
// Per Material

float _TrunkStiffness;
float _BranchStiffness;
float _LeafStiffness;

// ------------------------------
// Per Instance

UNITY_INSTANCING_BUFFER_START(Props)
UNITY_DEFINE_INSTANCED_PROP(float3, _Position)
UNITY_DEFINE_INSTANCED_PROP(half3, _VolumeWindDirection)
UNITY_DEFINE_INSTANCED_PROP(half, _VolumeWindStrength)
UNITY_DEFINE_INSTANCED_PROP(half, _VolumeWindTurbulence)
UNITY_INSTANCING_BUFFER_END(Props)

// ------------------------------
// Wave Functions

half4 SmoothCurve( half4 x ) 
{
    return x * x *( 3.0 - 2.0 * x );
}

half4 TriangleWave( half4 x ) 
{
    return abs( frac( x + 0.5 ) * 2.0 - 1.0 );
}

half4 SmoothTriangleWave( half4 x ) 
{
    return SmoothCurve( TriangleWave( x ) );
}

// ------------------------------
// Entry Point

float3 VegetationDeformation(float3 position, float3 origin, float3 normal, half3 vCol)
{
    // ------------------------------
    // Main Bending

    // Re-range material properties
    half turbulenceValue = UNITY_ACCESS_INSTANCED_PROP(Props, _VolumeWindTurbulence) * 10;
    half strengthValue = UNITY_ACCESS_INSTANCED_PROP(Props, _VolumeWindStrength) * 5;
    float2 directionValue = UNITY_ACCESS_INSTANCED_PROP(Props, _VolumeWindDirection).xz;

    // Calculate main bend
    float fBendScale = (1 - _TrunkStiffness) * 0.1; // main bend opacity
    float fLength = length(position); // distance to origin
    float2 turbulence = float2(sin(_Time.y * turbulenceValue + origin.x) * 0.1, sin(_Time.y * turbulenceValue + origin.z) * 0.1); // Turbulence 0 - 0.1
    float2 vWind = directionValue * lerp(0.1, turbulence, min(1, turbulence)); // wind direction

    // Bend factor - Wind variation is done on the CPU.
    float fBF = position.y * fBendScale * strengthValue;

    // Smooth bending factor and increase its nearby height limit.
    fBF += 1.0;
    fBF *= fBF;
    fBF = fBF * fBF - fBF;

    // Displace position
    float3 vNewPos = position;
    vNewPos.xz += vWind.xy * fBF;

    // Rescale
    position = normalize(vNewPos.xyz) * fLength;

    // ------------------------------
    // Detail blending

    float fSpeed = 0.25; // leaf occil
    float fDetailFreq = 0.3; // detail leaf occil
    float fEdgeAtten = vCol.x * (1 - _LeafStiffness) * 2; // leaf stiffness (red)
    float fDetailAmp = 0.5 * turbulenceValue; // leaf edge amplitude of movement
    float fBranchAtten = (1 - vCol.z) * (1 - _BranchStiffness) * 2; // branch stiffness (blue)
    float fBranchAmp = .11 * turbulenceValue; //5.5 // branch amplitude of movement
    float fBranchPhase = vCol.y * 3.3; // leaf phase (green)

    // Phases (object, vertex, branch)
    float fObjPhase = dot(origin, 1);
    fBranchPhase += fObjPhase;
    float fVtxPhase = dot(position, fBranchPhase + fBranchPhase);

    // x is used for edges; y is used for branches
    float2 vWavesIn = _Time.y + float2(fVtxPhase, fBranchPhase );

    // 1.975, 0.793, 0.375, 0.193 are good frequencies
    float4 vWaves = (frac( vWavesIn.xxyy * float4(1.975, 0.793, 0.375, 0.193) ) * 2.0 - 1.0 ) * fSpeed * fDetailFreq;
    vWaves = SmoothTriangleWave( vWaves );
    float2 vWavesSum = vWaves.xz + vWaves.yw;

    // Edge (xy) and branch bending (z)
    return position + vWavesSum.xyx * float3(fEdgeAtten * fDetailAmp * normal.x, fBranchAtten * fBranchAmp, fEdgeAtten * fDetailAmp * normal.z);
}