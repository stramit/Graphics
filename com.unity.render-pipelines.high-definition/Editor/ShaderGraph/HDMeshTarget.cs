using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Rendering.HighDefinition;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEditor.Graphing;

namespace UnityEditor.Rendering.HighDefinition.ShaderGraph
{
    class HDMeshTarget : ITargetImplementation
    {
        public Type targetType => typeof(MeshTarget);
        public string displayName => "HDRP";
        public string passTemplatePath => string.Empty;
        public string sharedTemplateDirectory => $"{HDUtils.GetHDRenderPipelinePath()}Editor/ShaderGraph/Templates";

        public Type[] requireBlocks => new Type[] 
        { 
            typeof(HDRPMeshOptionsBlock)
        };

        public void SetupTarget(ref TargetSetupContext context)
        {
            context.AddAssetDependencyPath(AssetDatabase.GUIDToAssetPath("7395c9320da217b42b9059744ceb1de6")); // MeshTarget
            context.AddAssetDependencyPath(AssetDatabase.GUIDToAssetPath("326a52113ee5a7d46bf9145976dcb7f6")); // HDRPMeshTarget

            if(context.blockDatas.Any(x => x is HDRPMeshOptionsBlock optionsBlock && 
                optionsBlock.lightingType == HDRPMeshOptionsBlock.LightingType.Lit))
            {
                context.SetupSubShader(HDSubShaders.HDLit);
            }
            else
            {
                context.SetupSubShader(HDSubShaders.HDUnlit);
            }
        }

        public IEnumerable<Type> GetSupportedBlocks(IEnumerable<BlockData> currentBlocks)
        {
            var supportedBlocks = ListPool<Type>.Get();

            // Add always supported features
            supportedBlocks.Add(typeof(HDRPMeshOptionsBlock));

            // TODO: Currently we avoid need to duplicate this list by only supporting blocks if OptionsBlock is active
            if(currentBlocks.FirstOrDefault(x => x is HDRPMeshOptionsBlock) is HDRPMeshOptionsBlock optionsBlock)
            {
                supportedBlocks.AddRange(optionsBlock.GetRequiredBlockTypes(false));
            }

            return supportedBlocks;
        }
    }
}
