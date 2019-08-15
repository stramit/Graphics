struct VaryingsToPS
{
    VaryingsMeshToPS vmesh;
#ifdef VARYINGS_NEED_PASS
    VaryingsPassToPS vpass;
#endif
};

struct PackedVaryingsToPS
{
#ifdef VARYINGS_NEED_PASS
    PackedVaryingsPassToPS vpass;
#endif
    PackedVaryingsMeshToPS vmesh;

    UNITY_VERTEX_OUTPUT_STEREO
};

PackedVaryingsToPS PackVaryingsToPS(VaryingsToPS input)
{
    PackedVaryingsToPS output;
    output.vmesh = PackVaryingsMeshToPS(input.vmesh);
#ifdef VARYINGS_NEED_PASS
    output.vpass = PackVaryingsPassToPS(input.vpass);
#endif

    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
    return output;
}

#define VaryingsType VaryingsToPS
#define VaryingsMeshType VaryingsMeshToPS
#define PackedVaryingsType PackedVaryingsToPS
#define PackVaryingsType PackVaryingsToPS

VaryingsMeshType VertMesh(AttributesMesh input)
{
    VaryingsMeshType output;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);

#if defined(HAVE_MESH_MODIFICATION)
    input = ApplyMeshModification(input, _TimeParameters.xyz);
#endif

    // This return the camera relative position (if enable)
    float3 positionWS = TransformObjectToWorld(input.positionOS);
#ifdef ATTRIBUTES_NEED_NORMAL
    float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
#else
    float3 normalWS = float3(0.0, 0.0, 0.0); // We need this case to be able to compile ApplyVertexModification that doesn't use normal.
#endif

#ifdef ATTRIBUTES_NEED_TANGENT
    float4 tangentWS = float4(TransformObjectToWorldDir(input.tangentOS.xyz), input.tangentOS.w);
#endif

     // Do vertex modification in camera relative space (if enable)
#if defined(HAVE_VERTEX_MODIFICATION)
    ApplyVertexModification(input, normalWS, positionWS, _TimeParameters.xyz);
#endif

#ifdef VARYINGS_NEED_POSITION_WS
    output.positionWS = positionWS;
#endif
    output.positionCS = TransformWorldToHClip(positionWS);
#ifdef VARYINGS_NEED_TANGENT_TO_WORLD
    output.normalWS = normalWS;
    output.tangentWS = tangentWS;
#endif

#if defined(VARYINGS_NEED_TEXCOORD0) || defined(VARYINGS_DS_NEED_TEXCOORD0)
    output.texCoord0 = input.uv0;
#endif
#if defined(VARYINGS_NEED_TEXCOORD1) || defined(VARYINGS_DS_NEED_TEXCOORD1)
    output.texCoord1 = input.uv1;
#endif
#if defined(VARYINGS_NEED_TEXCOORD2) || defined(VARYINGS_DS_NEED_TEXCOORD2)
    output.texCoord2 = input.uv2;
#endif
#if defined(VARYINGS_NEED_TEXCOORD3) || defined(VARYINGS_DS_NEED_TEXCOORD3)
    output.texCoord3 = input.uv3;
#endif
#if defined(VARYINGS_NEED_COLOR) || defined(VARYINGS_DS_NEED_COLOR)
    output.color = input.color;
#endif

    return output;
}