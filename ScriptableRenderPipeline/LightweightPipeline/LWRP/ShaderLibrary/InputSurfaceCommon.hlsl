#ifndef LIGHTWEIGHT_INPUT_SURFACE_COMMON_INCLUDED
#define LIGHTWEIGHT_INPUT_SURFACE_COMMON_INCLUDED

#include "Core.hlsl"
#include "CoreRP/ShaderLibrary/Packing.hlsl"
#include "CoreRP/ShaderLibrary/CommonMaterial.hlsl"

TEXTURE2D(_MainTex);            SAMPLER(sampler_MainTex);
TEXTURE2D(_BumpMap);            SAMPLER(sampler_BumpMap);
TEXTURE2D(_EmissionMap);        SAMPLER(sampler_EmissionMap);

TEXTURE3D(_OcclusionProbes);       SAMPLER(sampler_OcclusionProbes);
float4x4 _OcclusionProbesWorldToLocal;

// Must match Lightweigth ShaderGraph master node
struct SurfaceData
{
    half3 albedo;
    half3 specular;
    half  metallic;
    half  smoothness;
    half3 normalTS;
    half3 emission;
    half  occlusion;
    half  alpha;
};

///////////////////////////////////////////////////////////////////////////////
//                      Material Property Helpers                            //
///////////////////////////////////////////////////////////////////////////////
half Alpha(half albedoAlpha, half4 color, half cutoff)
{
#if !defined(_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A) && !defined(_GLOSSINESS_FROM_BASE_ALPHA)
    half alpha = albedoAlpha * color.a;
#else
    half alpha = color.a;
#endif

#if defined(_ALPHATEST_ON)
    clip(alpha - cutoff);
#endif

    return alpha;
}

half4 SampleAlbedoAlpha(float2 uv, TEXTURE2D_ARGS(albedoAlphaMap, sampler_albedoAlphaMap))
{
    return SAMPLE_TEXTURE2D(albedoAlphaMap, sampler_albedoAlphaMap, uv);
}

half3 SampleNormal(float2 uv, TEXTURE2D_ARGS(bumpMap, sampler_bumpMap), half scale = 1.0h)
{
#if _NORMALMAP
    half4 n = SAMPLE_TEXTURE2D(bumpMap, sampler_bumpMap, uv);
    #if BUMP_SCALE_NOT_SUPPORTED
        return UnpackNormal(n);
    #else
        return UnpackNormalScale(n, scale);
    #endif
#else
    return half3(0.0h, 0.0h, 1.0h);
#endif
}

half3 SampleEmission(float2 uv, half3 emissionColor, TEXTURE2D_ARGS(emissionMap, sampler_emissionMap))
{
#ifndef _EMISSION
    return 0;
#else
    return SAMPLE_TEXTURE2D(emissionMap, sampler_emissionMap, uv).rgb * emissionColor;
#endif
}

float SampleOcclusionProbes(float3 positionWS)
{
#if _OCCLUSION_PROBES
    // TODO: no full matrix mul needed, just scale and offset the pos (don't really need to support rotation)
    float occlusionProbes = 1;
    float3 pos = mul(_OcclusionProbesWorldToLocal, float4(positionWS, 1)).xyz;
    occlusionProbes = SAMPLE_TEXTURE3D(_OcclusionProbes, sampler_OcclusionProbes, pos).a;
    return occlusionProbes;
#else
    return 1;
#endif
}

#endif // LIGHTWEIGHT_INPUT_SURFACE_COMMON_INCLUDED
