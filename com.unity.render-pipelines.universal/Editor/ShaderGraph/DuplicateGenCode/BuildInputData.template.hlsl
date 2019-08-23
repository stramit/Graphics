void BuildInputData(Varyings input, float3 normal, out InputData inputData)
{
    inputData.positionWS = input.positionWS;

    #ifdef _NORMALMAP
        inputData.normalWS = normalize(TransformTangentToWorld(normal, half3x3(input.tangentWS.xyz, input.bitangentWS, input.normalWS)));
    #else
        #if !SHADER_HINT_NICE_QUALITY
            inputData.normalWS = input.normalWS;
        #else
            inputData.normalWS = normalize(input.normalWS);
        #endif
    #endif

    #if !SHADER_HINT_NICE_QUALITY
        // viewDirection should be normalized here, but we avoid doing it as it's close enough and we save some ALU.
        inputData.viewDirectionWS = input.viewDirectionWS;
    #else
        inputData.viewDirectionWS = normalize(input.viewDirectionWS);
    #endif

    inputData.shadowCoord = input.shadowCoord;

    inputData.fogCoord = input.fogFactorAndVertexLight.x;
    inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
    inputData.bakedGI = SAMPLE_GI(input.lightmapUV, input.sh, inputData.normalWS);
}