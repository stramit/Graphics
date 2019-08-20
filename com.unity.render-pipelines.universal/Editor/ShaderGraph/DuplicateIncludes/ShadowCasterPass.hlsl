#ifndef SG_SHADOW_PASS_INCLUDED
#define SG_SHADOW_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/DuplicateIncludes/VaryingVertMesh.hlsl"

float3 _LightDirection;

PackedVaryingsType vert(AttributesMesh inputMesh)
{
    VaryingsType output;
    output.vmesh = VertMesh(inputMesh);

    //define shadow pass specific clip position for universal 
    float4 clipPos = TransformWorldToHClip(ApplyShadowBias(output.vmesh.positionWS, output.vmesh.normalWS, _LightDirection));
    #if UNITY_REVERSED_Z
        clipPos.z = min(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
    #else
        clipPos.z = max(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
    #endif

    output.vmesh.positionCS = clipPos;

    return PackVaryingsType(output);
}

half4 frag(PackedVaryingsToPS packedInput) : SV_TARGET 
{    
    FragInputs input = UnpackVaryingsMeshToFragInputs(packedInput.vmesh);
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    
     // input.positionSS is SV_Position
    //PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionWS);
#ifdef VARYINGS_NEED_POSITION_WS
    half3 V = input.positionWS;
#else
    // Unused
    half3 V = half3(1.0, 1.0, 1.0); // Avoid the division by 0
#endif

    SurfaceData surfaceData;
    GetSurfaceData(input, V, surfaceData);

    half alpha = surfaceData.alpha;

    return 0;
}

#endif
