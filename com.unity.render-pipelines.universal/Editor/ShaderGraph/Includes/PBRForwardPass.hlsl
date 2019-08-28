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

PackedVaryings vert(Attributes input)
{
    Varyings output = (Varyings)0;
    output = BuildVaryings(input);
    PackedVaryings packedOutput = (PackedVaryings)0;
    packedOutput = PackVaryings(output);
    return packedOutput;
}

half4 frag(PackedVaryings packedInput) : SV_TARGET 
{    
    Varyings unpacked = UnpackVaryings(packedInput);
    UNITY_SETUP_INSTANCE_ID(unpacked);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(unpacked);

    SurfaceDescriptionInputs surfaceDescriptionInputs = BuildSurfaceDescriptionInputs(unpacked);
    SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);

    #if _AlphaClip
        clip(surfaceDescription.Alpha - surfaceDescription.AlphaClipThreshold);
    #endif

    InputData inputData;
    BuildInputData(unpacked, surfaceDescription.Normal, inputData);

    #ifdef _SPECULAR_SETUP
        float3 specular = surfaceDescription.Specular;
        float metallic = 0;
    #else   
        float3 specular = 0;
        float metallic = surfaceDescription.Metallic;
    #endif

    half4 color = UniversalFragmentPBR(
			inputData,
			surfaceDescription.Albedo,
			metallic,
			specular,
			surfaceDescription.Smoothness,
			surfaceDescription.Occlusion,
			surfaceDescription.Emission,
			surfaceDescription.Alpha); 

    //TODO: fog factor per vertex

    return color;
}
