#ifndef SG_LIT_META_INCLUDED
#define SG_LIT_META_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/DuplicateIncludes/VaryingVertMesh.hlsl"

PackedVaryingsType vert(AttributesMesh inputMesh)
{
    VaryingsType output;
    output.vmesh = VertMesh(inputMesh);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output.vmesh);

    //define meta clip position pass
    output.vmesh.positionCS = MetaVertexPosition(inputMesh.positionOS, inputMesh.uv1, inputMesh.uv1, unity_LightmapST, unity_DynamicLightmapST);

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

    MetaInput metaInput = (MetaInput)0;
    metaInput.Albedo = surfaceData.albedo;
    metaInput.Emission = surfaceData.emission;

    return MetaFragment(metaInput);
}

#endif
