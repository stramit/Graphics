using System.Collections.Generic;
using UnityEngine.Rendering;

namespace UnityEditor.ShaderGraph.Internal
{
    class PreviewTarget : ITarget
    {
        public string displayName => "PREVIEW";
        public string passTemplatePath => GenerationUtils.GetDefaultTemplatePath("PassMesh.template");
        public string sharedTemplateDirectory => GenerationUtils.GetDefaultSharedTemplateDirectory();

        public bool Validate(IMasterNode masterNode)
        {
            return false;
        }

        public void SetupTarget(ref TargetSetupContext context)
        {
            context.SetupTarget(SubShaders.Preview);
        }

#region SubShaders
        public static class SubShaders
        {
            public static SubShaderDescriptor Preview = new SubShaderDescriptor()
            {
                renderQueueOverride = "Geometry",
                renderTypeOverride = "Opaque",
                passes = new ShaderPass[] { Passes.Preview },
            };
        }
#endregion

#region Passes
        public static class Passes
        {
            public static ShaderPass Preview = new ShaderPass()
            {
                // Definition
                referenceName = "SHADERPASS_PREVIEW",
                passInclude = "Packages/com.unity.shadergraph/ShaderGraphLibrary/PreviewPass.hlsl",
                varyingsInclude = "Packages/com.unity.shadergraph/ShaderGraphLibrary/PreviewVaryings.hlsl",
                useInPreview = true,

                // Pass setup
                pragmas = new ConditionalPragma[]
                {
                    new ConditionalPragma(Pragma.Vertex("vert")),
                    new ConditionalPragma(Pragma.Fragment("frag")),
                },
                defines = new ConditionalDefine[]
                {
                    new ConditionalDefine(Keywords.Preview, 1),
                },
                includes = new ConditionalInclude[]
                {
                    new ConditionalInclude(Include.File("Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl")),
                    new ConditionalInclude(Include.File("Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl")),
                    new ConditionalInclude(Include.File("Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl")),
                    new ConditionalInclude(Include.File("Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl")),
                    new ConditionalInclude(Include.File("Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl")),
                    new ConditionalInclude(Include.File("Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl")),
                    new ConditionalInclude(Include.File("Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariables.hlsl")),
                    new ConditionalInclude(Include.File("Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl")),
                    new ConditionalInclude(Include.File("Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl")),
                },
            };
        }
#endregion

#region Keywords
        public static class Keywords
        {
            public static KeywordDescriptor Preview = new KeywordDescriptor()
            {
                displayName = "Preview",
                referenceName = "SHADERGRAPH_PREVIEW",
                type = KeywordType.Boolean,
                definition = KeywordDefinition.MultiCompile,
                scope = KeywordScope.Global,
            };
        }
#endregion
    }
}
