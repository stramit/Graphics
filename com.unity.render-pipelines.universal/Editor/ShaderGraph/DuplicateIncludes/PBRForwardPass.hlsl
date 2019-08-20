#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/DuplicateIncludes/VaryingVertMesh.hlsl"

PackedVaryingsType vert(AttributesMesh inputMesh)
{
    VaryingsType output;
    output.vmesh = VertMesh(inputMesh);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output.vmesh);

    //TODO : sample GI 

    //vertex lighting for universal forward
    half3 vertexLight = VertexLighting(output.vmesh.positionWS, output.vmesh.normalWS);
    half fogFactor = ComputeFogFactor(output.vmesh.positionCS.z);
    output.texCoord1 = half4(vertexLight, fogFactor);

    //vertex shadow coords
    #ifdef _MAIN_LIGHT_SHADOWS
		output.texCoord2 = GetShadowCoord(output.vmesh);
	#endif

    return PackVaryingsType(output);
}

half4 frag(PackedVaryingsToPS packedInput) : SV_TARGET 
{    
    FragInputs input = UnpackVaryingsMeshToFragInputs(packedInput.vmesh);
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    InputData inputData;
    // input.positionSS is SV_Position
    //PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionWS);
    //TODO: input data. normal +inputdata.viewdirection
#ifdef VARYINGS_NEED_POSITION_WS
    half3 V = input.positionWS;
#else
    // Unused
    half3 V = half3(1.0, 1.0, 1.0); // Avoid the division by 0
#endif
    inputData.positionWS = V; 
    inputData.normalWS = input.normalWS;

    inputData.shadowCoord = input.texCoord2;
    inputData.fogCoord = input.texCoord1.x;
    inputData.vertexLighting = input.texCoord1.yzw;
    //TODO: inputdata.bakedGI

    SurfaceData surfaceData;
    GetSurfaceData(input, V, surfaceData);

    half4 color = UniversalFragmentPBR(
			inputData,
			surfaceData.albedo,
			surfaceData.metallic,
			surfaceData.specular,
			surfaceData.smoothness,
			surfaceData.occlusion,
			surfaceData.emission,
			surfaceData.alpha); 

    //TODO: fog factor per vertex

    return color;
}
