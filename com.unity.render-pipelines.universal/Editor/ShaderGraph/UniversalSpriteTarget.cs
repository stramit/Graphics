using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEditor.Graphing;

namespace UnityEditor.Rendering.Universal.ShaderGraph
{
    class UniversalSpriteTarget : ITargetImplementation
    {
        public Type targetType => typeof(SpriteTarget);
        public string displayName => "Universal";
        public string passTemplatePath => GenerationUtils.GetDefaultTemplatePath("PassMesh.template");
        public string sharedTemplateDirectory => GenerationUtils.GetDefaultSharedTemplateDirectory();

        public Type[] requireBlocks => new Type[] { typeof(UniversalSpriteOptionsBlock )};

        public void SetupTarget(ref TargetSetupContext context)
        {
            context.AddAssetDependencyPath(AssetDatabase.GUIDToAssetPath("c6d8628bedefc4667b8789b924d7a909")); // SpriteTarget
            context.AddAssetDependencyPath(AssetDatabase.GUIDToAssetPath("3ffa2b308e219411ea6f62b1a8fa0ad0")); // UniversalSpriteTarget

            if(context.blockDatas.Any(x => x is UniversalSpriteOptionsBlock optionsBlock && optionsBlock.materialType == UniversalSpriteOptionsBlock.MaterialType.Lit))
            {
                context.SetupSubShader(UniversalSubShaders.SpriteLit);
            }
            else
            {
                context.SetupSubShader(UniversalSubShaders.SpriteUnlit);
            }
        }

        public List<Type> GetSupportedBlocks(List<BlockData> currentBlocks)
        {
            var supportedBlocks = ListPool<Type>.Get();

            // Add always supported features
            supportedBlocks.Add(typeof(UniversalSpriteOptionsBlock));
            
            // Add always supported data
            supportedBlocks.Add(typeof(VertexPositionBlock));
            supportedBlocks.Add(typeof(VertexNormalBlock));
            supportedBlocks.Add(typeof(VertexTangentBlock));
            supportedBlocks.Add(typeof(BaseColorBlock));

            // Evaluate remaining supported blocks
            if(currentBlocks.Any(x => x is UniversalSpriteOptionsBlock optionsBlock && optionsBlock.materialType == UniversalSpriteOptionsBlock.MaterialType.Lit))
            {
                supportedBlocks.Add(typeof(SpriteMaskBlock));
                supportedBlocks.Add(typeof(NormalTSBlock));
            }

            return supportedBlocks;
        }
    }
}
