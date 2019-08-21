using System;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.Rendering.Universal.Experimental
{
    class UniversalMeshTarget : IGraphTarget
    {
        public string displayName => "Mesh (Universal)";
        public string targetTemplatePath => "Packages/com.unity.shadergraph/Editor/Templates/DefaultTarget.template";
        public IEnumerable<string> assetDependencyPaths => null;

        public const string kPassTemplatePath = "Packages/com.unity.shadergraph/Editor/Templates/MeshPass.template";

        public void Generate(IGeneratorContext context)
        {
            context.SetDefaultEditorPath("UnityEditor.ShaderGraph.DefaultMeshGUI");
            context.SetPipelineTag("UniversalPipeline");

            switch(context.graphInfo.masterNodeInfo.type)
            {
                case "PBRMasterNode":
                    context.AddPass(ForwardPass);
                    context.AddPass(DepthOnlyPass);
                    context.AddPass(ShadowCasterPass);
                    context.AddPass(MetaPass);
                    break;
                case "UnlitMasterNode":
                    context.AddPass(UnlitPass);
                    context.AddPass(DepthOnlyPass);
                    context.AddPass(ShadowCasterPass);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

#region Resources
        static readonly string[] DefaultIncludes =
        {
            "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl",
            "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl",
            "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl",
            "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl",
        };

        static readonly string[] InstancedIncludes =
        {
            "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl",
            "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl",
            "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl",
            "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl",
            "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl",
        };
#endregion

#region Passes
        ShaderPass ForwardPass = new ShaderPass()
        {
            name = "UniversalForward",
            templatePath = UniversalMeshTarget.kPassTemplatePath,
            pragmas = new string[]
            {
                "prefer_hlslcc gles",
                "exclude_renderers d3d11_9x",
                "target 2.0",
                "multi_compile_instancing",
                "multi_compile_fog",
            },
            includes = InstancedIncludes,
            keywords = new KeywordDescriptor[]
            {
                LightmapKeyword,
                DirectionalLightmapCombinedKeyword,
                MainLightShadowsKeyword,
                MainLightShadowsCascadeKeyword,
                AdditionalLightsKeyword,
                AdditionalLightShadowsKeyword,
                ShadowsSoftKeyword,
                MixedLightingSubtractiveKeyword,
            },
        };

        ShaderPass UnlitPass = new ShaderPass()
        {
            name = "Unlit",
            templatePath = UniversalMeshTarget.kPassTemplatePath,
            pragmas = new string[]
            {
                "prefer_hlslcc gles",
                "exclude_renderers d3d11_9x",
                "target 2.0",
                "multi_compile_instancing",
                "multi_compile_fog",
            },
            includes = InstancedIncludes,
            keywords = new KeywordDescriptor[]
            {
                LightmapKeyword,
                DirectionalLightmapCombinedKeyword,
                SampleGIKeyword,
            },
        };

        ShaderPass ShadowCasterPass = new ShaderPass()
        {
            name = "ShadowCaster",
            templatePath = UniversalMeshTarget.kPassTemplatePath,
            pragmas = new string[]
            {
                "prefer_hlslcc gles",
                "exclude_renderers d3d11_9x",
                "target 2.0",
                "multi_compile_instancing",
            },
            includes = DefaultIncludes,
        };

        ShaderPass DepthOnlyPass = new ShaderPass()
        {
            name = "DepthOnly",
            templatePath = UniversalMeshTarget.kPassTemplatePath,
            pragmas = new string[]
            {
                "prefer_hlslcc gles",
                "exclude_renderers d3d11_9x",
                "target 2.0",
                "multi_compile_instancing",
            },
            includes = DefaultIncludes,
        };

        ShaderPass MetaPass = new ShaderPass()
        {
            name = "Meta",
            templatePath = UniversalMeshTarget.kPassTemplatePath,
            pragmas = new string[]
            {
                "prefer_hlslcc gles",
                "exclude_renderers d3d11_9x",
                "target 2.0",
            },
            includes = DefaultIncludes,
            keywords = new KeywordDescriptor[]
            {
                SmoothnessChannelKeyword,
            },
        };
#endregion

#region Keywords
        static KeywordDescriptor LightmapKeyword = new KeywordDescriptor()
        {
            displayName = "Lightmap",
            referenceName = "LIGHTMAP_ON",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        static KeywordDescriptor DirectionalLightmapCombinedKeyword = new KeywordDescriptor()
        {
            displayName = "Directional Lightmap Combined",
            referenceName = "DIRLIGHTMAP_COMBINED",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        static KeywordDescriptor SampleGIKeyword = new KeywordDescriptor()
        {
            displayName = "Sample GI",
            referenceName = "_SAMPLE_GI",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.ShaderFeature,
            scope = KeywordScope.Global,
        };

        static KeywordDescriptor MainLightShadowsKeyword = new KeywordDescriptor()
        {
            displayName = "Main Light Shadows",
            referenceName = "_MAIN_LIGHT_SHADOWS",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        static KeywordDescriptor MainLightShadowsCascadeKeyword = new KeywordDescriptor()
        {
            displayName = "Main Light Shadows Cascade",
            referenceName = "_MAIN_LIGHT_SHADOWS_CASCADE",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        static KeywordDescriptor AdditionalLightsKeyword = new KeywordDescriptor()
        {
            displayName = "Additional Lights",
            referenceName = "_ADDITIONAL",
            type = KeywordType.Enum,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
            entries = new KeywordEntry[]
            {
                new KeywordEntry() { displayName = "Vertex", referenceName = "LIGHTS_VERTEX" },
                new KeywordEntry() { displayName = "Fragment", referenceName = "LIGHTS" },
                new KeywordEntry() { displayName = "Off", referenceName = "OFF" },
            }
        };

        static KeywordDescriptor AdditionalLightShadowsKeyword = new KeywordDescriptor()
        {
            displayName = "Additional Light Shadows",
            referenceName = "_ADDITIONAL_LIGHT_SHADOWS",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        static KeywordDescriptor ShadowsSoftKeyword = new KeywordDescriptor()
        {
            displayName = "Shadows Soft",
            referenceName = "_SHADOWS_SOFT",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        static KeywordDescriptor MixedLightingSubtractiveKeyword = new KeywordDescriptor()
        {
            displayName = "Mixed Lighting Subtractive",
            referenceName = "_MIXED_LIGHTING_SUBTRACTIVE",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        static KeywordDescriptor SmoothnessChannelKeyword = new KeywordDescriptor()
        {
            displayName = "Smoothness Channel",
            referenceName = "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.ShaderFeature,
            scope = KeywordScope.Global,
        };
#endregion
    }
}
