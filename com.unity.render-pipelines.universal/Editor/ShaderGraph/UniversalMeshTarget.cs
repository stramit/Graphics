using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEditor.Graphing;

namespace UnityEditor.Rendering.Universal.ShaderGraph
{
    class UniversalMeshTarget : ITargetImplementation
    {
        public Type targetType => typeof(MeshTarget);
        public string displayName => "Universal";
        public string passTemplatePath => GenerationUtils.GetDefaultTemplatePath("PassMesh.template");
        public string sharedTemplateDirectory => GenerationUtils.GetDefaultSharedTemplateDirectory();

        public Type[] requireBlocks => new Type[] 
        { 
            typeof(UniversalMeshOptionsBlock)
        };

        public void SetupTarget(ref TargetSetupContext context)
        {
            context.AddAssetDependencyPath(AssetDatabase.GUIDToAssetPath("7395c9320da217b42b9059744ceb1de6")); // MeshTarget
            context.AddAssetDependencyPath(AssetDatabase.GUIDToAssetPath("ac9e1a400a9ce404c8f26b9c1238417e")); // UniversalMeshTarget

            if(context.blockDatas.Any(x => x is UniversalMeshOptionsBlock optionsBlock && 
                optionsBlock.materialType == UniversalMeshOptionsBlock.MaterialType.Lit))
            {
                context.SetupSubShader(UniversalSubShaders.PBR);
            }
            else
            {
                context.SetupSubShader(UniversalSubShaders.Unlit);
            }
        }

        public IEnumerable<Type> GetSupportedBlocks(IEnumerable<BlockData> currentBlocks)
        {
            var supportedBlocks = ListPool<Type>.Get();

            // Add always supported features
            supportedBlocks.Add(typeof(UniversalMeshOptionsBlock));
            supportedBlocks.Add(typeof(TransparencyBlock));
            supportedBlocks.Add(typeof(AlphaClipBlock));
            supportedBlocks.Add(typeof(RenderBackfacesBlock));
            
            // Add always supported data
            supportedBlocks.Add(typeof(VertexPositionBlock));
            supportedBlocks.Add(typeof(VertexNormalBlock));
            supportedBlocks.Add(typeof(VertexTangentBlock));
            supportedBlocks.Add(typeof(BaseColorBlock));

            // Evaluate remaining supported blocks
            if(currentBlocks.FirstOrDefault(x => x is UniversalMeshOptionsBlock) is UniversalMeshOptionsBlock optionsBlock && 
                optionsBlock.materialType == UniversalMeshOptionsBlock.MaterialType.Lit)
            {
                if(optionsBlock.workflowMode == UniversalMeshOptionsBlock.WorkflowMode.Specular)
                {
                    supportedBlocks.Add(typeof(SpecularColorBlock));
                }
                else
                {
                    supportedBlocks.Add(typeof(MetallicBlock));
                }
                
                supportedBlocks.Add(typeof(SmoothnessBlock));
                supportedBlocks.Add(typeof(NormalTSBlock));
                supportedBlocks.Add(typeof(EmissionBlock));
                supportedBlocks.Add(typeof(AmbientOcclusionBlock));
            }
            if(currentBlocks.Any(x => x is TransparencyBlock transparencyBlock) || currentBlocks.Any(x => x is AlphaClipBlock alphaClipBlock))
            {
                supportedBlocks.Add(typeof(AlphaBlock));
            }
            if(currentBlocks.Any(x => x is AlphaClipBlock alphaClipBlock))
            {
                supportedBlocks.Add(typeof(ClipThresholdBlock));
            }

            return supportedBlocks;
        }
    }
}
