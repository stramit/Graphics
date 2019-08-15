#ifndef UNIVERSAL_DEPTH_ONLY_PASS_INCLUDED
#define UNIVERSAL_DEPTH_ONLY_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/DuplicateIncludes/VaryingVertMesh.hlsl"

PackedVaryingsType vert(AttributesMesh inputMesh)
{
    VaryingsType varyingsType;
    varyingsType.vmesh = VertMesh(inputMesh);
    return PackVaryingsType(varyingsType);
}

half4 frag(PackedVaryingsToPS packedInput) : SV_TARGET 
{    
    FragInputs input = UnpackVaryingsMeshToFragInputs(packedInput.vmesh);
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(packedInput);

    return 0;
}

#endif
