#ifndef SG_LIT_META_INCLUDED
#define SG_LIT_META_INCLUDED

PackedVaryings vert(Attributes input)
{
    Varyings output = (Varyings)0;
    output = BuildVaryings(input);
    PackedVaryings packedOutput = (PackedVaryings)0;
    packedOutput = PackVaryings(output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(packedOutput);
    return packedOutput;
}

half4 frag(PackedVaryings packedInput) : SV_TARGET 
{    
    Varyings unpacked = UnpackVaryings(packedInput);
    UNITY_SETUP_INSTANCE_ID(unpacked);

    SurfaceDescriptionInputs surfaceDescriptionInputs = BuildSurfaceDescriptionInputs(unpacked);
    SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);

    half3 color = half3(0.5, 0.5, 0.5);
    half alpha = 1;
    half3 emission = half3(0, 0, 0);

#ifdef OUTPUT_SURFACEDESCRIPTION_COLOR
    color = surfaceDescription.Color;
#endif
#ifdef OUTPUT_SURFACEDESCRIPTION_ALPHA
    alpha = surfaceDescription.Alpha;
#endif
#ifdef OUTPUT_SURFACEDESCRIPTION_EMISSION
    emission = surfaceDescription.Emission;
#endif

#if _AlphaClip
    clip(alpha - surfaceDescription.AlphaClipThreshold);
#endif

    MetaInput metaInput = (MetaInput)0;
    metaInput.Albedo = color;
    metaInput.Emission = emission;

    return MetaFragment(metaInput);
}

#endif
