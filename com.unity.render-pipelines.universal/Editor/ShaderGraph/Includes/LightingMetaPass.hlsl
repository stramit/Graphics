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

    // Fields required by feature blocks are not currently generated
    // unless the corresponding data block is present
    // Therefore we need to predefine all potential data values.
    // Required fields should be tracked properly and generated.
    half3 color = half3(0.5, 0.5, 0.5);
    half3 emission = half3(0, 0, 0);
    half alpha = 1;
    half clipThreshold = 0.5;

    #if defined(FEATURES_GRAPH_PIXEL)
        SurfaceDescriptionInputs surfaceDescriptionInputs = BuildSurfaceDescriptionInputs(unpacked);
        SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);

        // Data is overriden if the corresponding data block is present.
        // Could use "$Tag.Field: value = surfaceDescription.Field" pattern
        // to avoid preprocessors if this was a template file.
        #ifdef OUTPUT_SURFACEDESCRIPTION_COLOR
            color = surfaceDescription.Color;
        #endif
        #ifdef OUTPUT_SURFACEDESCRIPTION_EMISSION
            emission = surfaceDescription.Emission;
        #endif
        #ifdef OUTPUT_SURFACEDESCRIPTION_ALPHA
            alpha = surfaceDescription.Alpha;
        #endif
        #ifdef OUTPUT_SURFACEDESCRIPTION_ALPHACLIPTHRESHOLD
            clipThreshold = surfaceDescription.AlphaClipThreshold;
        #endif
    #endif

    #if _AlphaClip
        clip(alpha - clipThreshold);
    #endif

    MetaInput metaInput = (MetaInput)0;
    metaInput.Albedo = color;
    metaInput.Emission = emission;

    return MetaFragment(metaInput);
}

#endif
