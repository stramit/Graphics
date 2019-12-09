#if ETC1_EXTERNAL_ALPHA
    TEXTURE2D(_AlphaTex); SAMPLER(sampler_AlphaTex);
    float _EnableAlphaTexture;
#endif
    float4 _RendererColor;

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
    
    // Fields required by feature blocks are not currently generated
    // unless the corresponding data block is present
    // Therefore we need to predefine all potential data values.
    // Required fields should be tracked properly and generated.
    half3 color = half3(0.5, 0.5, 0.5);
    half alpha = 1;

    #if defined(FEATURES_GRAPH_PIXEL)
        SurfaceDescriptionInputs surfaceDescriptionInputs = BuildSurfaceDescriptionInputs(unpacked);
        SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);
        
        // Data is overriden if the corresponding data block is present.
        // Could use "$Tag.Field: value = surfaceDescription.Field" pattern
        // to avoid preprocessors if this was a template file.
        #ifdef OUTPUT_SURFACEDESCRIPTION_COLOR
            color = surfaceDescription.Color;
        #endif
        #ifdef OUTPUT_SURFACEDESCRIPTION_ALPHA
            alpha = surfaceDescription.Alpha;
        #endif
    #endif

#if ETC1_EXTERNAL_ALPHA
    float4 alphaTex = SAMPLE_TEXTURE2D(_AlphaTex, sampler_AlphaTex, unpacked.texCoord0.xy);
    alpha = lerp (alpha, alphaTex.r, _EnableAlphaTexture);
#endif

    half4 result = half4(color, alpha);
    result *= unpacked.color;

    return result;
}
