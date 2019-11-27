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

#ifdef OUTPUT_SURFACEDESCRIPTION_COLOR
    color = surfaceDescription.Color;
#endif
#ifdef OUTPUT_SURFACEDESCRIPTION_ALPHA
    alpha = surfaceDescription.Alpha;
#endif

#if _AlphaClip
    clip(alpha - surfaceDescription.AlphaClipThreshold);
#endif

    half4 result = half4(color, alpha);
    return result;
}
