#ifndef TOON_PASS_LIT_INCLUDED
#define TOON_PASS_LIT_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

// --------------------------------------------------
// Lighting Functions

half3 LightingToonLambert(half3 lightColor, half3 lightDir, half3 normal)
{
    half NdotL = saturate(dot(normal, lightDir));
    half3 directDiffuse = NdotL;
#if defined(_LOCAL_DIRECTLIGHTING)
    directDiffuse = LinearToSRGB(SAMPLE_TEXTURE2D(_LocalDirectLighting, sampler_LocalDirectLighting, float2(NdotL, 0)).rgb);
#elif defined(_GLOBAL_DIRECTLIGHTING)
    directDiffuse = LinearToSRGB(SAMPLE_TEXTURE2D(_GlobalDirectLighting, sampler_GlobalDirectLighting, float2(NdotL, 0)).rgb);
#endif // _GLOBAL_DIRECTLIGHTING
#if defined(UNITY_COLORSPACE_GAMMA)
    directDiffuse = SRGBToLinear(directDiffuse);
#endif // UNITY_COLORSPACE_GAMMA
    return lightColor * directDiffuse;
}

half3 LightingToonSpecular(half3 lightColor, half3 lightDir, half3 normal, half3 viewDir, half4 specularGloss, half shininess)
{
    half3 halfVec = SafeNormalize(lightDir + viewDir);
    half NdotH = saturate(dot(normal, halfVec));
    half modifier = pow(NdotH, shininess) * specularGloss.a;
    half3 directSpecular = modifier;
#if defined(_LOCAL_DIRECTLIGHTING)
    directSpecular = LinearToSRGB(SAMPLE_TEXTURE2D(_LocalDirectLighting, sampler_LocalDirectLighting, float2(modifier, 1)).rgb);
#elif defined(_GLOBAL_DIRECTLIGHTING)
    directSpecular = LinearToSRGB(SAMPLE_TEXTURE2D(_GlobalDirectLighting, sampler_GlobalDirectLighting, float2(modifier, 1)).rgb);
#endif // _GLOBAL_DIRECTLIGHTING
#if defined(UNITY_COLORSPACE_GAMMA)
    directSpecular = SRGBToLinear(directSpecular);
#endif // UNITY_COLORSPACE_GAMMA
    half3 specularReflection = specularGloss.rgb * directSpecular;
    return lightColor * specularReflection;
}

// --------------------------------------------------
// GI Functions

#ifdef LIGHTMAP_ON
#define SAMPLE_GI_TOON(lmName, shName, normalWSName) SampleGIToon(lmName, normalWSName)
half3 SampleGIToon(float2 sampleData, half3 normalWS)
{
    return SampleLightmap(sampleData, normalWS);
}
#else
#define SAMPLE_GI_TOON(lmName, shName, normalWSName) SampleGIToon(shName, normalWSName)
half3 SampleGIToon(half3 sampleData, half3 normalWS)
{
    // If lightmap is not enabled we sample GI from SH
    half3 sh = SampleSHPixel(sampleData, normalWS);
#if defined(_LOCAL_INDIRECTLIGHTING)
    sh = SRGBToLinear(_LocalIndirectLighting.rgb);
#elif defined(_GLOBAL_INDIRECTLIGHTING)
    sh = SRGBToLinear(_GlobalIndirectLighting.rgb);
#endif
    return sh;
}
#endif

// --------------------------------------------------
// Shadow Functions

half MainLightRealtimeShadowAttenuationToon(float4 shadowCoord)
{
    half shadows;
#if !defined(_SHADOWS_ENABLED)
    return 1.0h;
#endif
#if SHADOWS_SCREEN
    shadows = SampleScreenSpaceShadowMap(shadowCoord);
#else
    ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
    half shadowStrength = GetMainLightShadowStrength();
    shadows = SampleShadowmap(shadowCoord, TEXTURE2D_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture), shadowSamplingData, shadowStrength, false);
#endif

#if defined(_LOCAL_SHADOW)
    shadows = LinearToSRGB(SAMPLE_TEXTURE2D(_LocalShadow, sampler_LocalShadow, float2(shadows.x, 0)).rgb);
#elif defined(_GLOBAL_SHADOW)
    shadows = LinearToSRGB(SAMPLE_TEXTURE2D(_GlobalShadow, sampler_GlobalShadow, float2(shadows.x, 0)).rgb);
#endif
    return shadows;
}

half LocalLightRealtimeShadowAttenuationToon(int lightIndex, float3 positionWS)
{
    half shadows;
#if !defined(_LOCAL_SHADOWS_ENABLED)
    return 1.0h;
#else
    float4 shadowCoord = mul(_LocalWorldToShadowAtlas[lightIndex], float4(positionWS, 1.0));
    ShadowSamplingData shadowSamplingData = GetLocalLightShadowSamplingData();
    half shadowStrength = GetLocalLightShadowStrenth(lightIndex);
    shadows = SampleShadowmap(shadowCoord, TEXTURE2D_PARAM(_LocalShadowmapTexture, sampler_LocalShadowmapTexture), shadowSamplingData, shadowStrength, true);
#endif

#if defined(_LOCAL_SHADOW)
    shadows = LinearToSRGB(SAMPLE_TEXTURE2D(_LocalShadow, sampler_LocalShadow, float2(shadows.x, 0)).rgb);
#elif defined(_GLOBAL_SHADOW)
    shadows = LinearToSRGB(SAMPLE_TEXTURE2D(_GlobalShadow, sampler_GlobalShadow, float2(shadows.x, 0)).rgb);
#endif
    return shadows;
}

// --------------------------------------------------
// Fog Functions

void ApplyFogColorToon(inout half3 color, half3 fogColor, half fogFactor)
{
#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
#if defined(FOG_EXP)
    // factor = exp(-density*z)
    // fogFactor = density*z compute at vertex
    fogFactor = saturate(exp2(-fogFactor));
#elif defined(FOG_EXP2)
    // factor = exp(-(density*z)^2)
    // fogFactor = density*z compute at vertex
    fogFactor = saturate(exp2(-fogFactor*fogFactor));
#endif
    color = lerp(fogColor, color, fogFactor);
#endif

#if defined(_LOCAL_FOG)
    half alpha = SAMPLE_TEXTURE2D(_LocalFog, sampler_LocalFog, float2(fogFactor, 1)).r;
    half3 c = SRGBToLinear(SAMPLE_TEXTURE2D(_LocalFog, sampler_LocalFog, float2(fogFactor, 0)).rgb);
    color = lerp(color, c, alpha);
#elif defined(_GLOBAL_FOG)
    half alpha = SAMPLE_TEXTURE2D(_GlobalFog, sampler_GlobalFog, float2(fogFactor, 1)).r;
    half3 c = SRGBToLinear(SAMPLE_TEXTURE2D(_GlobalFog, sampler_GlobalFog, float2(fogFactor, 0)).rgb);
    color = lerp(color, c, alpha);
#endif
}

void ApplyFogToon(inout half3 color, half fogFactor)
{
    ApplyFogColorToon(color, unity_FogColor.rgb, fogFactor);
}

// --------------------------------------------------
// Misc Stylization Functions

half3 ToonRimlight(half3 viewDir, half3 normal)
{
    half3 rimlight = 0;
#if defined(_LOCAL_RIMLIGHT)
    half rim = 1.0 - saturate(dot(normalize(viewDir), normal));
    half power = pow(rim, _LocalRimlight.a);
	//half power = pow(rim, 20 - (_LocalRimlight.a * 20));
	rimlight = _LocalRimlight.rgb * power;
#elif defined(_GLOBAL_RIMLIGHT)
    half rim = 1.0 - saturate(dot(normalize(viewDir), normal));
    half power = pow(rim, 20 - (_GlobalRimlight.a * 20));
	rimlight = _GlobalRimlight.rgb * power;
#endif
	return rimlight;
}

// --------------------------------------------------
// Fragment Functions

half4 ToonFragment(InputData inputData, half3 diffuse, half4 specularGloss, half shininess, half3 emission, half alpha)
{
    Light mainLight = GetMainLight();
    half3 attenuation = MainLightRealtimeShadowAttenuationToon(inputData.shadowCoord);
    MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI, half4(0, 0, 0, 0));

    half3 attenuatedLightColor = mainLight.color * attenuation;
    half3 diffuseColor = inputData.bakedGI + LightingToonLambert(attenuatedLightColor, mainLight.direction, inputData.normalWS);
    half3 specularColor = LightingToonSpecular(attenuatedLightColor, mainLight.direction, inputData.normalWS, inputData.viewDirectionWS, specularGloss, shininess);

#ifdef _ADDITIONAL_LIGHTS
    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; ++i)
    {
        Light light = GetAdditionalLight(i, inputData.positionWS);
        half3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
        diffuseColor += LightingToonLambert(attenuatedLightColor, light.direction, inputData.normalWS);
        specularColor += LightingToonSpecular(attenuatedLightColor, light.direction, inputData.normalWS, inputData.viewDirectionWS, specularGloss, shininess);
    }
#endif

    half3 fullDiffuse = diffuseColor + inputData.vertexLighting;
    half3 rimlight = ToonRimlight(inputData.viewDirectionWS, inputData.normalWS);
    half3 finalColor = fullDiffuse * diffuse + emission + rimlight;

#if defined(_SPECGLOSSMAP) || defined(_SPECULAR_COLOR)
    finalColor += specularColor;
#endif

    ApplyFogToon(finalColor, inputData.fogCoord);
    return half4(finalColor, alpha);
}

half4 LitPassFragmentToon(Varyings IN) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(IN);

    float2 uv = IN.uv;
    half4 diffuseAlpha = SampleAlbedoAlpha(uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
    half3 diffuse = diffuseAlpha.rgb * _BaseColor.rgb;

    half alpha = diffuseAlpha.a * _BaseColor.a;
    AlphaDiscard(alpha, _Cutoff);
#ifdef _ALPHAPREMULTIPLY_ON
    diffuse *= alpha;
#endif

    half3 normalTS = SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap));
    half3 emission = SampleEmission(uv, _EmissionColor.rgb, TEXTURE2D_ARGS(_EmissionMap, sampler_EmissionMap));
    half4 specularGloss = SampleSpecularSmoothness(uv, diffuseAlpha.a, _SpecColor, TEXTURE2D_ARGS(_SpecGlossMap, sampler_SpecGlossMap));
    half shininess = specularGloss.a;

    InputData inputData;
    InitializeInputData(IN, normalTS, inputData);

    inputData.bakedGI = SAMPLE_GI_TOON(IN.lightmapUV, IN.vertexSH, inputData.normalWS);

    return ToonFragment(inputData, diffuse, specularGloss, shininess, emission, alpha);
};

#endif // TOON_PASS_LIT_INCLUDED
