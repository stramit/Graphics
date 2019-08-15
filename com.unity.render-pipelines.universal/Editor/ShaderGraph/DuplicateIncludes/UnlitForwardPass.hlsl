#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/DuplicateIncludes/VaryingVertMesh.hlsl"

PackedVaryingsType vert(AttributesMesh inputMesh)
{
    VaryingsType varyingsType;
    varyingsType.vmesh = VertMesh(inputMesh);
    return PackVaryingsType(varyingsType);
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

    half3 albedo = surfaceData.albedo;
    half alpha = surfaceData.alpha;

#ifdef _ALPHAPREMULTIPLY_ON
    albedo *= alpha;
#endif

    return half4(albedo, alpha);
}
