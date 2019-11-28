using System;
using System.Linq;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

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
    }
}
