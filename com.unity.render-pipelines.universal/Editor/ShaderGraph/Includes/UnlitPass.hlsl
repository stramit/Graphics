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
    half clipThreshold = 0.5;

#ifdef OUTPUT_SURFACEDESCRIPTION_COLOR
    color = surfaceDescription.Color;
#endif
#ifdef OUTPUT_SURFACEDESCRIPTION_ALPHA
    alpha = surfaceDescription.Alpha;
#endif
#ifdef OUTPUT_SURFACEDESCRIPTION_ALPHACLIPTHRESHOLD
    clipThreshold = surfaceDescription.AlphaClipThreshold;
#endif

#if _AlphaClip
    clip(alpha - clipThreshold);
#endif

#ifdef _ALPHAPREMULTIPLY_ON
    color *= alpha;
#endif

    return half4(color, alpha);
}
