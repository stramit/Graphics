PackedVaryings vert(Attributes input)
{
    Varyings output = (Varyings)0;
    output = BuildVaryings(input);
    PackedVaryings packedOutput = PackVaryings(output);
    return packedOutput;
}

half4 frag(PackedVaryings packedInput) : SV_TARGET 
{    
    Varyings unpacked = UnpackVaryings(packedInput);
    UNITY_SETUP_INSTANCE_ID(unpacked);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(unpacked);

#if defined(FEATURES_GRAPH_PIXEL)
    SurfaceDescriptionInputs surfaceDescriptionInputs = BuildSurfaceDescriptionInputs(unpacked);
    SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);
#endif

    half3 color = half3(0.5, 0.5, 0.5);
    half alpha = 1;
    half3 normal = half3(0.5, 0.5, 1);

#ifdef OUTPUT_SURFACEDESCRIPTION_COLOR
    color = surfaceDescription.Color;
#endif
#ifdef OUTPUT_SURFACEDESCRIPTION_ALPHA
    alpha = surfaceDescription.Alpha;
#endif
#ifdef OUTPUT_SURFACEDESCRIPTION_NORMAL
    normal = surfaceDescription.Normal;
#endif

    return NormalsRenderingShared(half4(color, alpha), normal, unpacked.tangentWS.xyz, unpacked.bitangentWS, unpacked.normalWS);
}
