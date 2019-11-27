void BuildInputData(Varyings input, float3 normal, out InputData inputData)
{
    inputData.positionWS = input.positionWS;
#ifdef _NORMALMAP
    inputData.normalWS = TransformTangentToWorld(normal,
        half3x3(input.tangentWS.xyz, input.bitangentWS.xyz, input.normalWS.xyz));
#else
    inputData.normalWS = input.normalWS;
#endif
    inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
    inputData.viewDirectionWS = SafeNormalize(input.viewDirectionWS);
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

    half3 color = half3(0.5, 0.5, 0.5);
    half alpha = 1;
    half3 normal = half3(0.5, 0.5, 1);
    half smoothness = 0.5;
    half occlusion = 1;
    half3 emission = half3(0, 0, 0);

#ifdef OUTPUT_SURFACEDESCRIPTION_COLOR
    color = surfaceDescription.Color;
#endif
#ifdef OUTPUT_SURFACEDESCRIPTION_ALPHA
    alpha = surfaceDescription.Alpha;
#endif
#ifdef OUTPUT_SURFACEDESCRIPTION_NORMAL
    normal = surfaceDescription.Normal;
#endif
#ifdef OUTPUT_SURFACEDESCRIPTION_SMOOTHNESS
    smoothness = surfaceDescription.Smoothness;
#endif
#ifdef OUTPUT_SURFACEDESCRIPTION_OCCLUSION
    occlusion = surfaceDescription.Occlusion;
#endif
#ifdef OUTPUT_SURFACEDESCRIPTION_EMISSION
    emission = surfaceDescription.Emission;
#endif

#if _AlphaClip
    clip(alpha - surfaceDescription.AlphaClipThreshold);
#endif

    InputData inputData;
    BuildInputData(unpacked, normal, inputData);

#ifdef _SPECULAR_SETUP
    float3 specular = surfaceDescription.Specular;
    float metallic = 1;
#else   
    float3 specular = 0;
    float metallic = surfaceDescription.Metallic;
#endif

    half4 result = UniversalFragmentPBR(
			inputData,
			color,
			metallic,
			specular,
			smoothness,
			occlusion,
			emission,
			alpha); 

    result.rgb = MixFog(result.rgb, inputData.fogCoord); 
    return result;
}
