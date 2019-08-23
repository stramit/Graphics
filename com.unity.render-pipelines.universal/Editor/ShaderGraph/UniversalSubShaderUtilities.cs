using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using Data.Util;
using UnityEngine;              // Vector3,4

namespace UnityEditor.Rendering.Universal
{
    delegate void OnGeneratePassDelegate(IMasterNode masterNode, ref Pass pass, ref ShaderGraphRequirements requirements);
    struct Pass
    {
        public string Name;
        public string LightMode;
        public string ShaderPassName;
        public List<string> Includes;
        public string TemplateName;
        public string MaterialName;
        public List<string> ExtraInstancingOptions;
        public List<string> ExtraDefines;
        public List<int> VertexShaderSlots;         // These control what slots are used by the pass vertex shader
        public List<int> PixelShaderSlots;          // These control what slots are used by the pass pixel shader
        public string CullOverride;
        public string BlendOverride;
        public string BlendOpOverride;
        public string ZTestOverride;
        public string ZWriteOverride;
        public string ColorMaskOverride;
        public string ZClipOverride;
        public List<string> StencilOverride;
        public ShaderGraphRequirements Requirements;
        public List<string> RequiredFields;         // feeds into the dependency analysis
        public bool UseInPreview;
        public SurfaceMaterialTags PassTags;
        public SurfaceMaterialOptions PassOptions;

        // All these lists could probably be hashed to aid lookups.
        public bool VertexShaderUsesSlot(int slotId)
        {
            return VertexShaderSlots.Contains(slotId);
        }
        public bool PixelShaderUsesSlot(int slotId)
        {
            return PixelShaderSlots.Contains(slotId);
        }
        public void OnGeneratePass(IMasterNode masterNode, ShaderGraphRequirements requirements)
        {
            if (OnGeneratePassImpl != null)
            {
                OnGeneratePassImpl(masterNode, ref this, ref requirements);
            }
        }      
        public OnGeneratePassDelegate OnGeneratePassImpl;
    }

    static class UniversalShaderStructs
    {
        internal struct Attributes
        {
            [Semantic("POSITION")]                  Vector3 positionOS;
            [Semantic("NORMAL")][Optional]          Vector3 normalOS;
            [Semantic("TANGENT")][Optional]         Vector4 tangentOS;       // Stores bi-tangent sign in w
            [Semantic("TEXCOORD0")][Optional]       Vector4 uv0;
            [Semantic("TEXCOORD1")][Optional]       Vector4 uv1;
            [Semantic("TEXCOORD2")][Optional]       Vector4 uv2;
            [Semantic("TEXCOORD3")][Optional]       Vector4 uv3;
            [Semantic("COLOR")][Optional]           Vector4 color;
            [Semantic("INSTANCEID_SEMANTIC")] [PreprocessorIf("UNITY_ANY_INSTANCING_ENABLED")] uint instanceID;
        };

        [InterpolatorPack]
        internal struct Varyings
        {
            [Semantic("SV_Position")]
            Vector4 positionCS;
            [Optional]
            Vector3 positionWS;
            [Optional]
            Vector3 normalWS;
            [Optional]
            Vector4 tangentWS;
            [Optional]
            Vector4 texCoord0;
            [Optional]
            Vector4 texCoord1;
            [Optional]
            Vector4 texCoord2;
            [Optional]
            Vector4 texCoord3;
            [Optional]
            Vector4 color;
            [Optional]
            Vector3 viewDirectionWS;
            [Optional]
            Vector3 bitangentWS;
            [Optional][PreprocessorIf("defined(USE_LIGHTMAP)")]
            Vector2 lightmapUV;
            [Optional][PreprocessorIf("defined(USE_SH)")]
            Vector3 sh;
            [Optional]
            Vector4 fogFactorAndVertexLight;
            [Optional]
            Vector4 shadowCoord;
            [Semantic("CUSTOM_INSTANCE_ID")] [PreprocessorIf("UNITY_ANY_INSTANCING_ENABLED")]
            uint instanceID;
            [Semantic("FRONT_FACE_SEMANTIC")][OverrideType("FRONT_FACE_TYPE")][PreprocessorIf("defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)")]
            bool cullFace;

            public static Dependency[] standardDependencies = new Dependency[]
            {
                new Dependency("Varyings.positionWS",       "Attributes.positionOS"),
                new Dependency("Varyings.normalWS",         "Attributes.normalOS"),
                new Dependency("Varyings.tangentWS",        "Attributes.tangentOS"),
                new Dependency("Varyings.bitangentWS",      "Attributes.tangentOS"),
                new Dependency("Varyings.bitangentWS",      "Attributes.normalOS"),
                new Dependency("Varyings.texCoord0",        "Attributes.uv0"),
                new Dependency("Varyings.texCoord1",        "Attributes.uv1"),
                new Dependency("Varyings.texCoord2",        "Attributes.uv2"),
                new Dependency("Varyings.texCoord3",        "Attributes.uv3"),
                new Dependency("Varyings.color",            "Attributes.color"),
                new Dependency("Varyings.instanceID",       "Attributes.instanceID"),
            };
        };

        // this describes the input to the pixel shader graph eval
        internal struct SurfaceDescriptionInputs
        {
            [Optional] Vector3 ObjectSpaceNormal;
            [Optional] Vector3 ViewSpaceNormal;
            [Optional] Vector3 WorldSpaceNormal;
            [Optional] Vector3 TangentSpaceNormal;

            [Optional] Vector3 ObjectSpaceTangent;
            [Optional] Vector3 ViewSpaceTangent;
            [Optional] Vector3 WorldSpaceTangent;
            [Optional] Vector3 TangentSpaceTangent;

            [Optional] Vector3 ObjectSpaceBiTangent;
            [Optional] Vector3 ViewSpaceBiTangent;
            [Optional] Vector3 WorldSpaceBiTangent;
            [Optional] Vector3 TangentSpaceBiTangent;

            [Optional] Vector3 ObjectSpaceViewDirection;
            [Optional] Vector3 ViewSpaceViewDirection;
            [Optional] Vector3 WorldSpaceViewDirection;
            [Optional] Vector3 TangentSpaceViewDirection;

            [Optional] Vector3 ObjectSpacePosition;
            [Optional] Vector3 ViewSpacePosition;
            [Optional] Vector3 WorldSpacePosition;
            [Optional] Vector3 TangentSpacePosition;
            [Optional] Vector3 AbsoluteWorldSpacePosition;

            [Optional] Vector4 ScreenPosition;
            [Optional] Vector4 uv0;
            [Optional] Vector4 uv1;
            [Optional] Vector4 uv2;
            [Optional] Vector4 uv3;
            [Optional] Vector4 VertexColor;
            [Optional] float FaceSign;
            [Optional] Vector3 TimeParameters;

            public static Dependency[] dependencies = new Dependency[]
            {
                new Dependency("SurfaceDescriptionInputs.WorldSpaceNormal",          "Varyings.normalWS"),
                new Dependency("SurfaceDescriptionInputs.ObjectSpaceNormal",         "SurfaceDescriptionInputs.WorldSpaceNormal"),
                new Dependency("SurfaceDescriptionInputs.ViewSpaceNormal",           "SurfaceDescriptionInputs.WorldSpaceNormal"),

                new Dependency("SurfaceDescriptionInputs.WorldSpaceTangent",         "Varyings.tangentWS"),
                new Dependency("SurfaceDescriptionInputs.ObjectSpaceTangent",        "SurfaceDescriptionInputs.WorldSpaceTangent"),
                new Dependency("SurfaceDescriptionInputs.ViewSpaceTangent",          "SurfaceDescriptionInputs.WorldSpaceTangent"),

                new Dependency("SurfaceDescriptionInputs.WorldSpaceBiTangent",       "Varyings.bitangentWS"),
                new Dependency("SurfaceDescriptionInputs.ObjectSpaceBiTangent",      "SurfaceDescriptionInputs.WorldSpaceBiTangent"),
                new Dependency("SurfaceDescriptionInputs.ViewSpaceBiTangent",        "SurfaceDescriptionInputs.WorldSpaceBiTangent"),

                new Dependency("SurfaceDescriptionInputs.WorldSpacePosition",        "Varyings.positionWS"),
                new Dependency("SurfaceDescriptionInputs.AbsoluteWorldSpacePosition","Varyings.positionWS"),
                new Dependency("SurfaceDescriptionInputs.ObjectSpacePosition",       "Varyings.positionWS"),
                new Dependency("SurfaceDescriptionInputs.ViewSpacePosition",         "Varyings.positionWS"),

                new Dependency("SurfaceDescriptionInputs.WorldSpaceViewDirection",   "Varyings.viewDirectionWS"),                   // we build WorldSpaceViewDirection using Varyings.positionWS in GetWorldSpaceNormalizeViewDir()
                new Dependency("SurfaceDescriptionInputs.ObjectSpaceViewDirection",  "SurfaceDescriptionInputs.WorldSpaceViewDirection"),
                new Dependency("SurfaceDescriptionInputs.ViewSpaceViewDirection",    "SurfaceDescriptionInputs.WorldSpaceViewDirection"),
                new Dependency("SurfaceDescriptionInputs.TangentSpaceViewDirection", "SurfaceDescriptionInputs.WorldSpaceViewDirection"),
                new Dependency("SurfaceDescriptionInputs.TangentSpaceViewDirection", "SurfaceDescriptionInputs.WorldSpaceTangent"),
                new Dependency("SurfaceDescriptionInputs.TangentSpaceViewDirection", "SurfaceDescriptionInputs.WorldSpaceBiTangent"),
                new Dependency("SurfaceDescriptionInputs.TangentSpaceViewDirection", "SurfaceDescriptionInputs.WorldSpaceNormal"),

                new Dependency("SurfaceDescriptionInputs.ScreenPosition",            "SurfaceDescriptionInputs.WorldSpacePosition"),
                new Dependency("SurfaceDescriptionInputs.uv0",                       "Varyings.texCoord0"),
                new Dependency("SurfaceDescriptionInputs.uv1",                       "Varyings.texCoord1"),
                new Dependency("SurfaceDescriptionInputs.uv2",                       "Varyings.texCoord2"),
                new Dependency("SurfaceDescriptionInputs.uv3",                       "Varyings.texCoord3"),
                new Dependency("SurfaceDescriptionInputs.VertexColor",               "Varyings.color"),
                new Dependency("SurfaceDescriptionInputs.FaceSign",                  "Varyings.isFrontFace"),

                new Dependency("DepthOffset", "Varyings.positionWS"),
            };
        };

        // this describes the input to the pixel shader graph eval
        internal struct VertexDescriptionInputs
        {
            [Optional] Vector3 ObjectSpaceNormal;
            [Optional] Vector3 ViewSpaceNormal;
            [Optional] Vector3 WorldSpaceNormal;
            [Optional] Vector3 TangentSpaceNormal;

            [Optional] Vector3 ObjectSpaceTangent;
            [Optional] Vector3 ViewSpaceTangent;
            [Optional] Vector3 WorldSpaceTangent;
            [Optional] Vector3 TangentSpaceTangent;

            [Optional] Vector3 ObjectSpaceBiTangent;
            [Optional] Vector3 ViewSpaceBiTangent;
            [Optional] Vector3 WorldSpaceBiTangent;
            [Optional] Vector3 TangentSpaceBiTangent;

            [Optional] Vector3 ObjectSpaceViewDirection;
            [Optional] Vector3 ViewSpaceViewDirection;
            [Optional] Vector3 WorldSpaceViewDirection;
            [Optional] Vector3 TangentSpaceViewDirection;

            [Optional] Vector3 ObjectSpacePosition;
            [Optional] Vector3 ViewSpacePosition;
            [Optional] Vector3 WorldSpacePosition;
            [Optional] Vector3 TangentSpacePosition;
            [Optional] Vector3 AbsoluteWorldSpacePosition;

            [Optional] Vector4 ScreenPosition;
            [Optional] Vector4 uv0;
            [Optional] Vector4 uv1;
            [Optional] Vector4 uv2;
            [Optional] Vector4 uv3;
            [Optional] Vector4 VertexColor;
            [Optional] Vector3 TimeParameters;

            public static Dependency[] dependencies = new Dependency[]
            {                                                                       // TODO: NOCHECKIN: these dependencies are not correct for vertex pass
                new Dependency("VertexDescriptionInputs.ObjectSpaceNormal",         "Attributes.normalOS"),
                new Dependency("VertexDescriptionInputs.WorldSpaceNormal",          "Attributes.normalOS"),
                new Dependency("VertexDescriptionInputs.ViewSpaceNormal",           "VertexDescriptionInputs.WorldSpaceNormal"),

                new Dependency("VertexDescriptionInputs.ObjectSpaceTangent",        "Attributes.tangentOS"),
                new Dependency("VertexDescriptionInputs.WorldSpaceTangent",         "Attributes.tangentOS"),
                new Dependency("VertexDescriptionInputs.ViewSpaceTangent",          "VertexDescriptionInputs.WorldSpaceTangent"),

                new Dependency("VertexDescriptionInputs.ObjectSpaceBiTangent",      "Attributes.normalOS"),
                new Dependency("VertexDescriptionInputs.ObjectSpaceBiTangent",      "Attributes.tangentOS"),
                new Dependency("VertexDescriptionInputs.WorldSpaceBiTangent",       "VertexDescriptionInputs.ObjectSpaceBiTangent"),
                new Dependency("VertexDescriptionInputs.ViewSpaceBiTangent",        "VertexDescriptionInputs.WorldSpaceBiTangent"),

                new Dependency("VertexDescriptionInputs.ObjectSpacePosition",       "Attributes.positionOS"),
                new Dependency("VertexDescriptionInputs.WorldSpacePosition",        "Attributes.positionOS"),
                new Dependency("VertexDescriptionInputs.AbsoluteWorldSpacePosition","Attributes.positionOS"),
                new Dependency("VertexDescriptionInputs.ViewSpacePosition",         "VertexDescriptionInputs.WorldSpacePosition"),

                new Dependency("VertexDescriptionInputs.WorldSpaceViewDirection",   "VertexDescriptionInputs.WorldSpacePosition"),
                new Dependency("VertexDescriptionInputs.ObjectSpaceViewDirection",  "VertexDescriptionInputs.WorldSpaceViewDirection"),
                new Dependency("VertexDescriptionInputs.ViewSpaceViewDirection",    "VertexDescriptionInputs.WorldSpaceViewDirection"),
                new Dependency("VertexDescriptionInputs.TangentSpaceViewDirection", "VertexDescriptionInputs.WorldSpaceViewDirection"),
                new Dependency("VertexDescriptionInputs.TangentSpaceViewDirection", "VertexDescriptionInputs.WorldSpaceTangent"),
                new Dependency("VertexDescriptionInputs.TangentSpaceViewDirection", "VertexDescriptionInputs.WorldSpaceBiTangent"),
                new Dependency("VertexDescriptionInputs.TangentSpaceViewDirection", "VertexDescriptionInputs.WorldSpaceNormal"),

                new Dependency("VertexDescriptionInputs.ScreenPosition",            "VertexDescriptionInputs.WorldSpacePosition"),
                new Dependency("VertexDescriptionInputs.uv0",                       "Attributes.uv0"),
                new Dependency("VertexDescriptionInputs.uv1",                       "Attributes.uv1"),
                new Dependency("VertexDescriptionInputs.uv2",                       "Attributes.uv2"),
                new Dependency("VertexDescriptionInputs.uv3",                       "Attributes.uv3"),
                new Dependency("VertexDescriptionInputs.VertexColor",               "Attributes.color"),
            };
        };

        // TODO: move this out of UniversalShaderStructs
        static public void AddActiveFieldsFromVertexGraphRequirements(IActiveFieldsSet activeFields, ShaderGraphRequirements requirements)
        {
            if (requirements.requiresScreenPosition)
            {
                activeFields.AddAll("VertexDescriptionInputs.ScreenPosition");
            }

            if (requirements.requiresVertexColor)
            {
                activeFields.AddAll("VertexDescriptionInputs.VertexColor");
            }

            if (requirements.requiresNormal != 0)
            {
                if ((requirements.requiresNormal & NeededCoordinateSpace.Object) > 0)
                    activeFields.AddAll("VertexDescriptionInputs.ObjectSpaceNormal");

                if ((requirements.requiresNormal & NeededCoordinateSpace.View) > 0)
                    activeFields.AddAll("VertexDescriptionInputs.ViewSpaceNormal");

                if ((requirements.requiresNormal & NeededCoordinateSpace.World) > 0)
                    activeFields.AddAll("VertexDescriptionInputs.WorldSpaceNormal");

                if ((requirements.requiresNormal & NeededCoordinateSpace.Tangent) > 0)
                    activeFields.AddAll("VertexDescriptionInputs.TangentSpaceNormal");
            }

            if (requirements.requiresTangent != 0)
            {
                if ((requirements.requiresTangent & NeededCoordinateSpace.Object) > 0)
                    activeFields.AddAll("VertexDescriptionInputs.ObjectSpaceTangent");

                if ((requirements.requiresTangent & NeededCoordinateSpace.View) > 0)
                    activeFields.AddAll("VertexDescriptionInputs.ViewSpaceTangent");

                if ((requirements.requiresTangent & NeededCoordinateSpace.World) > 0)
                    activeFields.AddAll("VertexDescriptionInputs.WorldSpaceTangent");

                if ((requirements.requiresTangent & NeededCoordinateSpace.Tangent) > 0)
                    activeFields.AddAll("VertexDescriptionInputs.TangentSpaceTangent");
            }

            if (requirements.requiresBitangent != 0)
            {
                if ((requirements.requiresBitangent & NeededCoordinateSpace.Object) > 0)
                    activeFields.AddAll("VertexDescriptionInputs.ObjectSpaceBiTangent");

                if ((requirements.requiresBitangent & NeededCoordinateSpace.View) > 0)
                    activeFields.AddAll("VertexDescriptionInputs.ViewSpaceBiTangent");

                if ((requirements.requiresBitangent & NeededCoordinateSpace.World) > 0)
                    activeFields.AddAll("VertexDescriptionInputs.WorldSpaceBiTangent");

                if ((requirements.requiresBitangent & NeededCoordinateSpace.Tangent) > 0)
                    activeFields.AddAll("VertexDescriptionInputs.TangentSpaceBiTangent");
            }

            if (requirements.requiresViewDir != 0)
            {
                if ((requirements.requiresViewDir & NeededCoordinateSpace.Object) > 0)
                    activeFields.AddAll("VertexDescriptionInputs.ObjectSpaceViewDirection");

                if ((requirements.requiresViewDir & NeededCoordinateSpace.View) > 0)
                    activeFields.AddAll("VertexDescriptionInputs.ViewSpaceViewDirection");

                if ((requirements.requiresViewDir & NeededCoordinateSpace.World) > 0)
                    activeFields.AddAll("VertexDescriptionInputs.WorldSpaceViewDirection");

                if ((requirements.requiresViewDir & NeededCoordinateSpace.Tangent) > 0)
                    activeFields.AddAll("VertexDescriptionInputs.TangentSpaceViewDirection");
            }

            if (requirements.requiresPosition != 0)
            {
                if ((requirements.requiresPosition & NeededCoordinateSpace.Object) > 0)
                    activeFields.AddAll("VertexDescriptionInputs.ObjectSpacePosition");

                if ((requirements.requiresPosition & NeededCoordinateSpace.View) > 0)
                    activeFields.AddAll("VertexDescriptionInputs.ViewSpacePosition");

                if ((requirements.requiresPosition & NeededCoordinateSpace.World) > 0)
                    activeFields.AddAll("VertexDescriptionInputs.WorldSpacePosition");

                if ((requirements.requiresPosition & NeededCoordinateSpace.Tangent) > 0)
                    activeFields.AddAll("VertexDescriptionInputs.TangentSpacePosition");

                if ((requirements.requiresPosition & NeededCoordinateSpace.AbsoluteWorld) > 0)
                    activeFields.AddAll("VertexDescriptionInputs.AbsoluteWorldSpacePosition");
            }

            foreach (var channel in requirements.requiresMeshUVs.Distinct())
            {
                activeFields.AddAll("VertexDescriptionInputs." + channel.GetUVName());
            }

            if (requirements.requiresTime)
            {
                activeFields.AddAll("VertexDescriptionInputs.TimeParameters");
            }
        }

        // TODO: move this out of UniversalShaderStructs
        static public void AddActiveFieldsFromPixelGraphRequirements(IActiveFields activeFields, ShaderGraphRequirements requirements)
        {
            if (requirements.requiresScreenPosition)
            {
                activeFields.Add("SurfaceDescriptionInputs.ScreenPosition");
            }

            if (requirements.requiresVertexColor)
            {
                activeFields.Add("SurfaceDescriptionInputs.VertexColor");
            }

            if (requirements.requiresFaceSign)
            {
                activeFields.Add("SurfaceDescriptionInputs.FaceSign");
            }

            if (requirements.requiresNormal != 0)
            {
                if ((requirements.requiresNormal & NeededCoordinateSpace.Object) > 0)
                    activeFields.Add("SurfaceDescriptionInputs.ObjectSpaceNormal");

                if ((requirements.requiresNormal & NeededCoordinateSpace.View) > 0)
                    activeFields.Add("SurfaceDescriptionInputs.ViewSpaceNormal");

                if ((requirements.requiresNormal & NeededCoordinateSpace.World) > 0)
                    activeFields.Add("SurfaceDescriptionInputs.WorldSpaceNormal");

                if ((requirements.requiresNormal & NeededCoordinateSpace.Tangent) > 0)
                    activeFields.Add("SurfaceDescriptionInputs.TangentSpaceNormal");
            }

            if (requirements.requiresTangent != 0)
            {
                if ((requirements.requiresTangent & NeededCoordinateSpace.Object) > 0)
                    activeFields.Add("SurfaceDescriptionInputs.ObjectSpaceTangent");

                if ((requirements.requiresTangent & NeededCoordinateSpace.View) > 0)
                    activeFields.Add("SurfaceDescriptionInputs.ViewSpaceTangent");

                if ((requirements.requiresTangent & NeededCoordinateSpace.World) > 0)
                    activeFields.Add("SurfaceDescriptionInputs.WorldSpaceTangent");

                if ((requirements.requiresTangent & NeededCoordinateSpace.Tangent) > 0)
                    activeFields.Add("SurfaceDescriptionInputs.TangentSpaceTangent");
            }

            if (requirements.requiresBitangent != 0)
            {
                if ((requirements.requiresBitangent & NeededCoordinateSpace.Object) > 0)
                    activeFields.Add("SurfaceDescriptionInputs.ObjectSpaceBiTangent");

                if ((requirements.requiresBitangent & NeededCoordinateSpace.View) > 0)
                    activeFields.Add("SurfaceDescriptionInputs.ViewSpaceBiTangent");

                if ((requirements.requiresBitangent & NeededCoordinateSpace.World) > 0)
                    activeFields.Add("SurfaceDescriptionInputs.WorldSpaceBiTangent");

                if ((requirements.requiresBitangent & NeededCoordinateSpace.Tangent) > 0)
                    activeFields.Add("SurfaceDescriptionInputs.TangentSpaceBiTangent");
            }

            if (requirements.requiresViewDir != 0)
            {
                if ((requirements.requiresViewDir & NeededCoordinateSpace.Object) > 0)
                    activeFields.Add("SurfaceDescriptionInputs.ObjectSpaceViewDirection");

                if ((requirements.requiresViewDir & NeededCoordinateSpace.View) > 0)
                    activeFields.Add("SurfaceDescriptionInputs.ViewSpaceViewDirection");

                if ((requirements.requiresViewDir & NeededCoordinateSpace.World) > 0)
                    activeFields.Add("SurfaceDescriptionInputs.WorldSpaceViewDirection");

                if ((requirements.requiresViewDir & NeededCoordinateSpace.Tangent) > 0)
                    activeFields.Add("SurfaceDescriptionInputs.TangentSpaceViewDirection");
            }

            if (requirements.requiresPosition != 0)
            {
                if ((requirements.requiresPosition & NeededCoordinateSpace.Object) > 0)
                    activeFields.Add("SurfaceDescriptionInputs.ObjectSpacePosition");

                if ((requirements.requiresPosition & NeededCoordinateSpace.View) > 0)
                    activeFields.Add("SurfaceDescriptionInputs.ViewSpacePosition");

                if ((requirements.requiresPosition & NeededCoordinateSpace.World) > 0)
                    activeFields.Add("SurfaceDescriptionInputs.WorldSpacePosition");

                if ((requirements.requiresPosition & NeededCoordinateSpace.Tangent) > 0)
                    activeFields.Add("SurfaceDescriptionInputs.TangentSpacePosition");

                if ((requirements.requiresPosition & NeededCoordinateSpace.AbsoluteWorld) > 0)
                    activeFields.Add("SurfaceDescriptionInputs.AbsoluteWorldSpacePosition");
            }

            foreach (var channel in requirements.requiresMeshUVs.Distinct())
            {
                activeFields.Add("SurfaceDescriptionInputs." + channel.GetUVName());
            }

            if (requirements.requiresTime)
            {
                activeFields.Add("SurfaceDescriptionInputs.TimeParameters");
            }
        }

        public static void AddRequiredFields(
            List<string> passRequiredFields,            // fields the pass requires
            IActiveFieldsSet activeFields)
        {
            if (passRequiredFields != null)
            {
                foreach (var requiredField in passRequiredFields)
                {
                    activeFields.AddAll(requiredField);
                }
            }
        }
    };

    static class UniversalSubShaderUtilities
    {
        public static readonly NeededCoordinateSpace k_PixelCoordinateSpace = NeededCoordinateSpace.World;

        static string GetTemplatePath(string templateName)
        {
            var basePath = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/";
            string templatePath = Path.Combine(basePath, templateName);

            if (File.Exists(templatePath))
                return templatePath;

            throw new FileNotFoundException(string.Format(@"Cannot find a template with name ""{0}"".", templateName));
        }

        public static string GetSubShader<T>(T masterNode, SurfaceMaterialTags tags, SurfaceMaterialOptions options, Pass[] passes, 
            GenerationMode mode, string customEditorPath = null, List<string> sourceAssetDependencyPaths = null) where T : IMasterNode
        {
            if (sourceAssetDependencyPaths != null)
            {
                var relativePath = "Packages/com.unity.render-pipelines.universal/";
                var fullPath = Path.GetFullPath(relativePath);
                var shaderFiles = Directory.GetFiles(Path.Combine(fullPath, "ShaderLibrary")).Select(x => Path.Combine(relativePath, x.Substring(fullPath.Length)));
                sourceAssetDependencyPaths.AddRange(shaderFiles);
            }

            var subShader = new ShaderStringBuilder();
            subShader.AppendLine("SubShader");
            using (subShader.BlockScope())
            {
                var tagsBuilder = new ShaderStringBuilder(0);
                tags.GetTags(tagsBuilder, UnityEngine.Rendering.Universal.UniversalRenderPipeline.k_ShaderTagName);
                subShader.AppendLines(tagsBuilder.ToString());

                foreach(Pass pass in passes)
                {
                    var templatePath = GetTemplatePath(pass.TemplateName);
                    if (!File.Exists(templatePath))
                        continue;

                    if (sourceAssetDependencyPaths != null)
                        sourceAssetDependencyPaths.Add(templatePath);

                    string template = File.ReadAllText(templatePath);

                    subShader.AppendLines(GetShaderPassFromTemplate(
                        template,
                        masterNode,
                        pass,
                        mode,
                        options));
                }
            }

            if(!string.IsNullOrEmpty(customEditorPath))
                subShader.Append($"CustomEditor \"{customEditorPath}\"");

            return subShader.ToString();
        }

        static string GetShaderPassFromTemplate(string template, IMasterNode iMasterNode, Pass pass, GenerationMode mode, SurfaceMaterialOptions materialOptions)
        {
            // ----------------------------------------------------- //
            //                         SETUP                         //
            // ----------------------------------------------------- //

            AbstractMaterialNode masterNode = iMasterNode as AbstractMaterialNode;

            // -------------------------------------
            // String builders

            var shaderProperties = new PropertyCollector();
            var shaderKeywords = new KeywordCollector();
            var shaderPropertyUniforms = new ShaderStringBuilder(1);
            var shaderKeywordDeclarations = new ShaderStringBuilder(1);

            var functionBuilder = new ShaderStringBuilder(1);
            var functionRegistry = new FunctionRegistry(functionBuilder);

            var defines = new ShaderStringBuilder(1);
            var graph = new ShaderStringBuilder(0);

            var vertexDescriptionInputStruct = new ShaderStringBuilder(1);
            var vertexDescriptionStruct = new ShaderStringBuilder(1);
            var vertexDescriptionFunction = new ShaderStringBuilder(1);

            var surfaceDescriptionInputStruct = new ShaderStringBuilder(1);
            var surfaceDescriptionStruct = new ShaderStringBuilder(1);
            var surfaceDescriptionFunction = new ShaderStringBuilder(1);

            var vertexInputStruct = new ShaderStringBuilder(1);
            var vertexOutputStruct = new ShaderStringBuilder(2);

            var vertexShader = new ShaderStringBuilder(2);
            var vertexShaderDescriptionInputs = new ShaderStringBuilder(2);
            var vertexShaderOutputs = new ShaderStringBuilder(2);

            var pixelShader = new ShaderStringBuilder(2);
            var pixelShaderSurfaceInputs = new ShaderStringBuilder(2);
            var pixelShaderSurfaceRemap = new ShaderStringBuilder(2);

            // -------------------------------------
            // Get Slot and Node lists per stage

            var vertexSlots = pass.VertexShaderSlots.Select(masterNode.FindSlot<MaterialSlot>).ToList();
            var vertexNodes = ListPool<AbstractMaterialNode>.Get();
            NodeUtils.DepthFirstCollectNodesFromNode(vertexNodes, masterNode, NodeUtils.IncludeSelf.Include, pass.VertexShaderSlots);

            var pixelSlots = pass.PixelShaderSlots.Select(masterNode.FindSlot<MaterialSlot>).ToList();
            var pixelNodes = ListPool<AbstractMaterialNode>.Get();
            NodeUtils.DepthFirstCollectNodesFromNode(pixelNodes, masterNode, NodeUtils.IncludeSelf.Include, pass.PixelShaderSlots);

            // -------------------------------------
            // Get Requirements

            var vertexRequirements = ShaderGraphRequirements.FromNodes(vertexNodes, ShaderStageCapability.Vertex, false);
            var pixelRequirements = ShaderGraphRequirements.FromNodes(pixelNodes, ShaderStageCapability.Fragment);
            var graphRequirements = pixelRequirements.Union(vertexRequirements);
            var surfaceRequirements = ShaderGraphRequirements.FromNodes(pixelNodes, ShaderStageCapability.Fragment, false);
            var modelRequirements = pass.Requirements;

            // ----------------------------------------------------- //
            //                START SHADER GENERATION                //
            // ----------------------------------------------------- //

            // -------------------------------------
            // Calculate material options

            var blendingBuilder = new ShaderStringBuilder(1);
            var cullingBuilder = new ShaderStringBuilder(1);
            var zTestBuilder = new ShaderStringBuilder(1);
            var zWriteBuilder = new ShaderStringBuilder(1);

            materialOptions.GetBlend(blendingBuilder);
            materialOptions.GetCull(cullingBuilder);
            materialOptions.GetDepthTest(zTestBuilder);
            materialOptions.GetDepthWrite(zWriteBuilder);

            // -------------------------------------
            // Generate defines

            pass.OnGeneratePass(iMasterNode, graphRequirements);
            
            foreach(string define in pass.ExtraDefines)
                defines.AppendLine(define);

            // ----------------------------------------------------- //
            //                         KEYWORDS                      //
            // ----------------------------------------------------- //

            // -------------------------------------
            // Get keyword permutations

            masterNode.owner.CollectShaderKeywords(shaderKeywords, mode);

            // Track permutation indices for all nodes
            List<int>[] keywordPermutationsPerVertexNode = new List<int>[vertexNodes.Count];
            List<int>[] keywordPermutationsPerPixelNode = new List<int>[pixelNodes.Count];

            // -------------------------------------
            // Evaluate all permutations

            for(int i = 0; i < shaderKeywords.permutations.Count; i++)
            {
                // Get active nodes for this permutation
                var localVertexNodes = ListPool<AbstractMaterialNode>.Get();
                var localPixelNodes = ListPool<AbstractMaterialNode>.Get();
                NodeUtils.DepthFirstCollectNodesFromNode(localVertexNodes, masterNode, NodeUtils.IncludeSelf.Include, pass.VertexShaderSlots, shaderKeywords.permutations[i]);
                NodeUtils.DepthFirstCollectNodesFromNode(localPixelNodes, masterNode, NodeUtils.IncludeSelf.Include, pass.PixelShaderSlots, shaderKeywords.permutations[i]);

                // Track each vertex node in this permutation
                foreach(AbstractMaterialNode vertexNode in localVertexNodes)
                {
                    int nodeIndex = vertexNodes.IndexOf(vertexNode);

                    if(keywordPermutationsPerVertexNode[nodeIndex] == null)
                        keywordPermutationsPerVertexNode[nodeIndex] = new List<int>();
                    keywordPermutationsPerVertexNode[nodeIndex].Add(i);
                }

                // Track each pixel node in this permutation
                foreach(AbstractMaterialNode pixelNode in localPixelNodes)
                {
                    int nodeIndex = pixelNodes.IndexOf(pixelNode);

                    if(keywordPermutationsPerPixelNode[nodeIndex] == null)
                        keywordPermutationsPerPixelNode[nodeIndex] = new List<int>();
                    keywordPermutationsPerPixelNode[nodeIndex].Add(i);
                }
            }

            // ----------------------------------------------------- //
            //                START VERTEX DESCRIPTION               //
            // ----------------------------------------------------- //

            // -------------------------------------
            // Generate Input structure for Vertex Description function
            // TODO - Vertex Description Input requirements are needed to exclude intermediate translation spaces

            vertexDescriptionInputStruct.AppendLine("struct VertexDescriptionInputs");
            using (vertexDescriptionInputStruct.BlockSemicolonScope())
            {
                ShaderGenerator.GenerateSpaceTranslationSurfaceInputs(vertexRequirements.requiresNormal, InterpolatorType.Normal, vertexDescriptionInputStruct);
                ShaderGenerator.GenerateSpaceTranslationSurfaceInputs(vertexRequirements.requiresTangent, InterpolatorType.Tangent, vertexDescriptionInputStruct);
                ShaderGenerator.GenerateSpaceTranslationSurfaceInputs(vertexRequirements.requiresBitangent, InterpolatorType.BiTangent, vertexDescriptionInputStruct);
                ShaderGenerator.GenerateSpaceTranslationSurfaceInputs(vertexRequirements.requiresViewDir, InterpolatorType.ViewDirection, vertexDescriptionInputStruct);
                ShaderGenerator.GenerateSpaceTranslationSurfaceInputs(vertexRequirements.requiresPosition, InterpolatorType.Position, vertexDescriptionInputStruct);

                if (vertexRequirements.requiresVertexColor)
                    vertexDescriptionInputStruct.AppendLine("float4 {0};", ShaderGeneratorNames.VertexColor);

                if (vertexRequirements.requiresScreenPosition)
                    vertexDescriptionInputStruct.AppendLine("float4 {0};", ShaderGeneratorNames.ScreenPosition);

                foreach (var channel in vertexRequirements.requiresMeshUVs.Distinct())
                    vertexDescriptionInputStruct.AppendLine("half4 {0};", channel.GetUVName());

                if (vertexRequirements.requiresTime)
                {
                    vertexDescriptionInputStruct.AppendLine("float3 {0};", ShaderGeneratorNames.TimeParameters);
                }
            }

            // -------------------------------------
            // Generate Output structure for Vertex Description function

            SubShaderGenerator.GenerateVertexDescriptionStruct(vertexDescriptionStruct, vertexSlots);

            // -------------------------------------
            // Generate Vertex Description function

            SubShaderGenerator.GenerateVertexDescriptionFunction(
                masterNode.owner as GraphData,
                vertexDescriptionFunction,
                functionRegistry,
                shaderProperties,
                shaderKeywords,
                mode,
                masterNode,
                vertexNodes,
                keywordPermutationsPerVertexNode,
                vertexSlots);

            // ----------------------------------------------------- //
            //               START SURFACE DESCRIPTION               //
            // ----------------------------------------------------- //

            // -------------------------------------
            // Generate Input structure for Surface Description function
            // Surface Description Input requirements are needed to exclude intermediate translation spaces

            surfaceDescriptionInputStruct.AppendLine("struct SurfaceDescriptionInputs");
            using (surfaceDescriptionInputStruct.BlockSemicolonScope())
            {
                ShaderGenerator.GenerateSpaceTranslationSurfaceInputs(surfaceRequirements.requiresNormal, InterpolatorType.Normal, surfaceDescriptionInputStruct);
                ShaderGenerator.GenerateSpaceTranslationSurfaceInputs(surfaceRequirements.requiresTangent, InterpolatorType.Tangent, surfaceDescriptionInputStruct);
                ShaderGenerator.GenerateSpaceTranslationSurfaceInputs(surfaceRequirements.requiresBitangent, InterpolatorType.BiTangent, surfaceDescriptionInputStruct);
                ShaderGenerator.GenerateSpaceTranslationSurfaceInputs(surfaceRequirements.requiresViewDir, InterpolatorType.ViewDirection, surfaceDescriptionInputStruct);
                ShaderGenerator.GenerateSpaceTranslationSurfaceInputs(surfaceRequirements.requiresPosition, InterpolatorType.Position, surfaceDescriptionInputStruct);

                if (surfaceRequirements.requiresVertexColor)
                    surfaceDescriptionInputStruct.AppendLine("float4 {0};", ShaderGeneratorNames.VertexColor);

                if (surfaceRequirements.requiresScreenPosition)
                    surfaceDescriptionInputStruct.AppendLine("float4 {0};", ShaderGeneratorNames.ScreenPosition);

                if (surfaceRequirements.requiresFaceSign)
                    surfaceDescriptionInputStruct.AppendLine("float {0};", ShaderGeneratorNames.FaceSign);

                foreach (var channel in surfaceRequirements.requiresMeshUVs.Distinct())
                    surfaceDescriptionInputStruct.AppendLine("half4 {0};", channel.GetUVName());

                if (surfaceRequirements.requiresTime)
                {
                    surfaceDescriptionInputStruct.AppendLine("float3 {0};", ShaderGeneratorNames.TimeParameters);
                }
            }

            // -------------------------------------
            // Generate Output structure for Surface Description function

            SubShaderGenerator.GenerateSurfaceDescriptionStruct(surfaceDescriptionStruct, pixelSlots);

            // -------------------------------------
            // Generate Surface Description function

            SubShaderGenerator.GenerateSurfaceDescriptionFunction(
                pixelNodes,
                keywordPermutationsPerPixelNode,
                masterNode,
                masterNode.owner as GraphData,
                surfaceDescriptionFunction,
                functionRegistry,
                shaderProperties,
                shaderKeywords,
                mode,
                "PopulateSurfaceData",
                "SurfaceDescription",
                null,
                pixelSlots);

            // ----------------------------------------------------- //
            //           GENERATE VERTEX > PIXEL PIPELINE            //
            // ----------------------------------------------------- //

            // -------------------------------------
            // Keyword declarations

            shaderKeywords.GetKeywordsDeclaration(shaderKeywordDeclarations, mode);

            // -------------------------------------
            // Property uniforms

            shaderProperties.GetPropertiesDeclaration(shaderPropertyUniforms, mode, masterNode.owner.concretePrecision);

            // -------------------------------------
            // Generate Input structure for Vertex shader

            SubShaderGenerator.GenerateApplicationVertexInputs(vertexRequirements.Union(pixelRequirements.Union(modelRequirements)), vertexInputStruct);

            // -------------------------------------
            // Generate standard transformations
            // This method ensures all required transform data is available in vertex and pixel stages

            ShaderGenerator.GenerateStandardTransforms(
                3,
                10,
                vertexOutputStruct,
                vertexShader,
                vertexShaderDescriptionInputs,
                vertexShaderOutputs,
                pixelShader,
                pixelShaderSurfaceInputs,
                pixelRequirements,
                surfaceRequirements,
                modelRequirements,
                vertexRequirements,
                CoordinateSpace.World);

            // -------------------------------------
            // Generate pixel shader surface remap

            foreach (var slot in pixelSlots)
            {
                pixelShaderSurfaceRemap.AppendLine("{0} = surf.{0};", slot.shaderOutputName);
            }

            // -------------------------------------
            // Extra pixel shader work

            var faceSign = new ShaderStringBuilder();

            if (pixelRequirements.requiresFaceSign)
                faceSign.AppendLine(", half FaceSign : VFACE");

            // ----------------------------------------------------- //
            //                      FINALIZE                         //
            // ----------------------------------------------------- //

            // -------------------------------------
            // Combine Graph sections

            graph.AppendLines(shaderKeywordDeclarations.ToString());
            graph.AppendLines(shaderPropertyUniforms.ToString());

            graph.AppendLine(vertexDescriptionInputStruct.ToString());
            graph.AppendLine(surfaceDescriptionInputStruct.ToString());

            graph.AppendLine(functionBuilder.ToString());

            graph.AppendLine(vertexDescriptionStruct.ToString());
            graph.AppendLine(vertexDescriptionFunction.ToString());

            graph.AppendLine(surfaceDescriptionStruct.ToString());
            graph.AppendLine(surfaceDescriptionFunction.ToString());

            graph.AppendLine(vertexInputStruct.ToString());

            // -------------------------------------
            // Generate final subshader

            var resultPass = template.Replace("${Tags}", string.Empty);
            resultPass = resultPass.Replace("${Blending}", blendingBuilder.ToString());
            resultPass = resultPass.Replace("${Culling}", cullingBuilder.ToString());
            resultPass = resultPass.Replace("${ZTest}", zTestBuilder.ToString());
            resultPass = resultPass.Replace("${ZWrite}", zWriteBuilder.ToString());
            resultPass = resultPass.Replace("${Defines}", defines.ToString());

            resultPass = resultPass.Replace("${Graph}", graph.ToString());
            resultPass = resultPass.Replace("${VertexOutputStruct}", vertexOutputStruct.ToString());

            resultPass = resultPass.Replace("${VertexShader}", vertexShader.ToString());
            resultPass = resultPass.Replace("${VertexShaderDescriptionInputs}", vertexShaderDescriptionInputs.ToString());
            resultPass = resultPass.Replace("${VertexShaderOutputs}", vertexShaderOutputs.ToString());

            resultPass = resultPass.Replace("${FaceSign}", faceSign.ToString());
            resultPass = resultPass.Replace("${PixelShader}", pixelShader.ToString());
            resultPass = resultPass.Replace("${PixelShaderSurfaceInputs}", pixelShaderSurfaceInputs.ToString());
            resultPass = resultPass.Replace("${PixelShaderSurfaceRemap}", pixelShaderSurfaceRemap.ToString());

            return resultPass;
        }

        static List<Dependency[]> k_Dependencies = new List<Dependency[]>()
        {
            UniversalShaderStructs.Varyings.standardDependencies,
            UniversalShaderStructs.SurfaceDescriptionInputs.dependencies,
            UniversalShaderStructs.VertexDescriptionInputs.dependencies
        };

        public static bool GenerateShaderPass(AbstractMaterialNode masterNode, Pass pass, GenerationMode mode, ActiveFields activeFields, ShaderGenerator result, List<string> sourceAssetDependencyPaths, bool vertexActive, SurfaceMaterialTags tags)
        {
            string templateLocation = GetTemplatePath(pass.TemplateName);
            if (!File.Exists(templateLocation))
            {
                // TODO: produce error here
                Debug.LogError("Template not found: " + templateLocation);
                return false;
            }

            bool debugOutput = true;

            // grab all of the active nodes (for pixel and vertex graphs)
            var vertexNodes = Graphing.ListPool<AbstractMaterialNode>.Get();
            NodeUtils.DepthFirstCollectNodesFromNode(vertexNodes, masterNode, NodeUtils.IncludeSelf.Include, pass.VertexShaderSlots);

            var pixelNodes = Graphing.ListPool<AbstractMaterialNode>.Get();
            NodeUtils.DepthFirstCollectNodesFromNode(pixelNodes, masterNode, NodeUtils.IncludeSelf.Include, pass.PixelShaderSlots);

            // graph requirements describe what the graph itself requires
            ShaderGraphRequirementsPerKeyword pixelRequirements = new ShaderGraphRequirementsPerKeyword();
            ShaderGraphRequirementsPerKeyword vertexRequirements = new ShaderGraphRequirementsPerKeyword();
            ShaderGraphRequirementsPerKeyword graphRequirements = new ShaderGraphRequirementsPerKeyword();

            // Function Registry tracks functions to remove duplicates, it wraps a string builder that stores the combined function string
            ShaderStringBuilder graphNodeFunctions = new ShaderStringBuilder();
            graphNodeFunctions.IncreaseIndent();
            var functionRegistry = new FunctionRegistry(graphNodeFunctions);

            // TODO: this can be a shared function for all HDRP master nodes -- From here through GraphUtil.GenerateSurfaceDescription(..)

            // Build the list of active slots based on what the pass requires
            var pixelSlots = UniversalSubShaderUtilities.FindMaterialSlotsOnNode(pass.PixelShaderSlots, masterNode);
            var vertexSlots = UniversalSubShaderUtilities.FindMaterialSlotsOnNode(pass.VertexShaderSlots, masterNode);

            // properties used by either pixel and vertex shader
            PropertyCollector sharedProperties = new PropertyCollector();
            KeywordCollector sharedKeywords = new KeywordCollector();
            ShaderStringBuilder shaderPropertyUniforms = new ShaderStringBuilder(1);
            ShaderStringBuilder shaderKeywordDeclarations = new ShaderStringBuilder(1);
            ShaderStringBuilder shaderKeywordPermutations = new ShaderStringBuilder(1);

            // build the graph outputs structure to hold the results of each active slots (and fill out activeFields to indicate they are active)
            string pixelGraphInputStructName = "SurfaceDescriptionInputs";
            string pixelGraphOutputStructName = "SurfaceDescription";
            string pixelGraphEvalFunctionName = "SurfaceDescriptionFunction";
            ShaderStringBuilder pixelGraphEvalFunction = new ShaderStringBuilder();
            ShaderStringBuilder pixelGraphOutputs = new ShaderStringBuilder();

            // ----------------------------------------------------- //
            //                         KEYWORDS                      //
            // ----------------------------------------------------- //

            // -------------------------------------
            // Get keyword permutations

            masterNode.owner.CollectShaderKeywords(sharedKeywords, mode);

            // Track permutation indices for all nodes
            List<int>[] keywordPermutationsPerVertexNode = new List<int>[vertexNodes.Count];
            List<int>[] keywordPermutationsPerPixelNode = new List<int>[pixelNodes.Count];

            // -------------------------------------
            // Evaluate all permutations

            if (sharedKeywords.permutations.Count > 0)
            {
                for(int i = 0; i < sharedKeywords.permutations.Count; i++)
                {
                    // Get active nodes for this permutation
                    var localVertexNodes = UnityEngine.Rendering.ListPool<AbstractMaterialNode>.Get();
                    var localPixelNodes = UnityEngine.Rendering.ListPool<AbstractMaterialNode>.Get();
                    NodeUtils.DepthFirstCollectNodesFromNode(localVertexNodes, masterNode, NodeUtils.IncludeSelf.Include, pass.VertexShaderSlots, sharedKeywords.permutations[i]);
                    NodeUtils.DepthFirstCollectNodesFromNode(localPixelNodes, masterNode, NodeUtils.IncludeSelf.Include, pass.PixelShaderSlots, sharedKeywords.permutations[i]);

                    // Track each vertex node in this permutation
                    foreach(AbstractMaterialNode vertexNode in localVertexNodes)
                    {
                        int nodeIndex = vertexNodes.IndexOf(vertexNode);

                        if(keywordPermutationsPerVertexNode[nodeIndex] == null)
                            keywordPermutationsPerVertexNode[nodeIndex] = new List<int>();
                        keywordPermutationsPerVertexNode[nodeIndex].Add(i);
                    }

                    // Track each pixel node in this permutation
                    foreach(AbstractMaterialNode pixelNode in localPixelNodes)
                    {
                        int nodeIndex = pixelNodes.IndexOf(pixelNode);

                        if(keywordPermutationsPerPixelNode[nodeIndex] == null)
                            keywordPermutationsPerPixelNode[nodeIndex] = new List<int>();
                        keywordPermutationsPerPixelNode[nodeIndex].Add(i);
                    }

                    // Get active requirements for this permutation
                    var localVertexRequirements = ShaderGraphRequirements.FromNodes(localVertexNodes, ShaderStageCapability.Vertex, false);
                    var localPixelRequirements = ShaderGraphRequirements.FromNodes(localPixelNodes, ShaderStageCapability.Fragment, false);

                    vertexRequirements[i].SetRequirements(localVertexRequirements);
                    pixelRequirements[i].SetRequirements(localPixelRequirements);

                    // build initial requirements
                    UniversalShaderStructs.AddActiveFieldsFromPixelGraphRequirements(activeFields[i], localPixelRequirements);
                    UniversalShaderStructs.AddActiveFieldsFromVertexGraphRequirements(activeFields[i], localVertexRequirements);
                }
            }
            else
            {
                pixelRequirements.baseInstance.SetRequirements(ShaderGraphRequirements.FromNodes(pixelNodes, ShaderStageCapability.Fragment, false));   // TODO: is ShaderStageCapability.Fragment correct?
                vertexRequirements.baseInstance.SetRequirements(ShaderGraphRequirements.FromNodes(vertexNodes, ShaderStageCapability.Vertex, false));
                UniversalShaderStructs.AddActiveFieldsFromPixelGraphRequirements(activeFields.baseInstance, pixelRequirements.baseInstance.requirements);
                UniversalShaderStructs.AddActiveFieldsFromVertexGraphRequirements(activeFields.baseInstance, vertexRequirements.baseInstance.requirements);
            }

            graphRequirements.UnionWith(pixelRequirements);
            graphRequirements.UnionWith(vertexRequirements);

            // build the graph outputs structure, and populate activeFields with the fields of that structure
            SubShaderGenerator.GenerateSurfaceDescriptionStruct(pixelGraphOutputs, pixelSlots, pixelGraphOutputStructName, activeFields.baseInstance);

            // Build the graph evaluation code, to evaluate the specified slots
            SubShaderGenerator.GenerateSurfaceDescriptionFunction(
                pixelNodes,
                keywordPermutationsPerPixelNode,
                masterNode,
                masterNode.owner as GraphData,
                pixelGraphEvalFunction,
                functionRegistry,
                sharedProperties,
                sharedKeywords,
                mode,
                pixelGraphEvalFunctionName,
                pixelGraphOutputStructName,
                null,
                pixelSlots,
                pixelGraphInputStructName);

            string vertexGraphInputStructName = "VertexDescriptionInputs";
            string vertexGraphOutputStructName = "VertexDescription";
            string vertexGraphEvalFunctionName = "VertexDescriptionFunction";
            ShaderStringBuilder vertexGraphEvalFunction = new ShaderStringBuilder();
            ShaderStringBuilder vertexGraphOutputs = new ShaderStringBuilder();

            // check for vertex animation -- enables HAVE_VERTEX_MODIFICATION
            if (vertexActive)
            {
                vertexActive = true;
                activeFields.baseInstance.Add("features.modifyMesh");

                // -------------------------------------
                // Generate Output structure for Vertex Description function
                SubShaderGenerator.GenerateVertexDescriptionStruct(vertexGraphOutputs, vertexSlots, vertexGraphOutputStructName, activeFields.baseInstance);

                // -------------------------------------
                // Generate Vertex Description function
                SubShaderGenerator.GenerateVertexDescriptionFunction(
                    masterNode.owner as GraphData,
                    vertexGraphEvalFunction,
                    functionRegistry,
                    sharedProperties,
                    sharedKeywords,
                    mode,
                    masterNode,
                    vertexNodes,
                    keywordPermutationsPerVertexNode,
                    vertexSlots,
                    vertexGraphInputStructName,
                    vertexGraphEvalFunctionName,
                    vertexGraphOutputStructName);
            }

            var blendCode = new ShaderStringBuilder();
            var cullCode = new ShaderStringBuilder();
            var zTestCode = new ShaderStringBuilder();
            var zWriteCode = new ShaderStringBuilder();
            var zClipCode = new ShaderStringBuilder();
            var stencilCode = new ShaderStringBuilder();
            var colorMaskCode = new ShaderStringBuilder();
            UniversalSubShaderUtilities.BuildRenderStatesFromPass(pass, blendCode, cullCode, zTestCode, zWriteCode, zClipCode, stencilCode, colorMaskCode);

            UniversalShaderStructs.AddRequiredFields(pass.RequiredFields, activeFields.baseInstance);

            // Get keyword declarations
            sharedKeywords.GetKeywordsDeclaration(shaderKeywordDeclarations, mode);

            // Get property declarations
            sharedProperties.GetPropertiesDeclaration(shaderPropertyUniforms, mode, masterNode.owner.concretePrecision);

            // propagate active field requirements using dependencies
            foreach (var instance in activeFields.all.instances)
                ShaderSpliceUtil.ApplyDependencies(instance, k_Dependencies);

            // debug output all active fields
            var interpolatorDefines = new ShaderGenerator();
            if (debugOutput)
            {
                interpolatorDefines.AddShaderChunk("// ACTIVE FIELDS:");
                foreach (string f in activeFields.baseInstance.fields)
                {
                    interpolatorDefines.AddShaderChunk("//   " + f);
                }
            }

            // build graph inputs structures
            ShaderGenerator pixelGraphInputs = new ShaderGenerator();
            ShaderSpliceUtil.BuildType(typeof(UniversalShaderStructs.SurfaceDescriptionInputs), activeFields, pixelGraphInputs);
            ShaderGenerator vertexGraphInputs = new ShaderGenerator();
            ShaderSpliceUtil.BuildType(typeof(UniversalShaderStructs.VertexDescriptionInputs), activeFields, vertexGraphInputs);

            ShaderGenerator instancingOptions = new ShaderGenerator();
            {
                instancingOptions.AddShaderChunk("#pragma multi_compile_instancing", true);
                if (pass.ExtraInstancingOptions != null)
                {
                    foreach (var instancingOption in pass.ExtraInstancingOptions)
                        instancingOptions.AddShaderChunk(instancingOption);
                }
            }

            ShaderGenerator defines = new ShaderGenerator();
            {
                defines.AddShaderChunk(string.Format("#define SHADERPASS {0}", pass.ShaderPassName), true);
                if (pass.ExtraDefines != null)
                {
                    foreach (var define in pass.ExtraDefines)
                        defines.AddShaderChunk(define);
                }

                if (graphRequirements.permutationCount > 0)
                {
                    {
                        var activePermutationIndices = graphRequirements.allPermutations.instances
                            .Where(p => p.requirements.requiresDepthTexture)
                            .Select(p => p.permutationIndex)
                            .ToList();
                        if (activePermutationIndices.Count > 0)
                        {
                            defines.AddShaderChunk(KeywordUtil.GetKeywordPermutationSetConditional(activePermutationIndices));
                            defines.AddShaderChunk("#define REQUIRE_DEPTH_TEXTURE");
                            defines.AddShaderChunk("#endif");
                        }
                    }

                    {
                        var activePermutationIndices = graphRequirements.allPermutations.instances
                            .Where(p => p.requirements.requiresCameraOpaqueTexture)
                            .Select(p => p.permutationIndex)
                            .ToList();
                        if (activePermutationIndices.Count > 0)
                        {
                            defines.AddShaderChunk(KeywordUtil.GetKeywordPermutationSetConditional(activePermutationIndices));
                            defines.AddShaderChunk("#define REQUIRE_OPAQUE_TEXTURE");
                            defines.AddShaderChunk("#endif");
                        }
                    }
                }
                else
                {
                    if (graphRequirements.baseInstance.requirements.requiresDepthTexture)
                        defines.AddShaderChunk("#define REQUIRE_DEPTH_TEXTURE");
                    if (graphRequirements.baseInstance.requirements.requiresCameraOpaqueTexture)
                        defines.AddShaderChunk("#define REQUIRE_OPAQUE_TEXTURE");
                }

                defines.AddGenerator(interpolatorDefines);
            }

            var shaderPassIncludes = new ShaderGenerator();
            if (pass.Includes != null)
            {
                foreach (var include in pass.Includes)
                    shaderPassIncludes.AddShaderChunk(include);
            }

            defines.AddShaderChunk("// Shared Graph Keywords");
            defines.AddShaderChunk(shaderKeywordDeclarations.ToString());
            defines.AddShaderChunk(shaderKeywordPermutations.ToString());

            // build graph code
            var graph = new ShaderGenerator();
            {
                graph.AddShaderChunk("// Shared Graph Properties (uniform inputs)");
                graph.AddShaderChunk(shaderPropertyUniforms.ToString());

                if (vertexActive)
                {
                    graph.AddShaderChunk("// Vertex Graph Inputs");
                    graph.Indent();
                    graph.AddGenerator(vertexGraphInputs);
                    graph.Deindent();
                    graph.AddShaderChunk("// Vertex Graph Outputs");
                    graph.Indent();
                    graph.AddShaderChunk(vertexGraphOutputs.ToString());
                    graph.Deindent();
                }

                graph.AddShaderChunk("// Pixel Graph Inputs");
                graph.Indent();
                graph.AddGenerator(pixelGraphInputs);
                graph.Deindent();
                graph.AddShaderChunk("// Pixel Graph Outputs");
                graph.Indent();
                graph.AddShaderChunk(pixelGraphOutputs.ToString());
                graph.Deindent();

                graph.AddShaderChunk("// Shared Graph Node Functions");
                graph.AddShaderChunk(graphNodeFunctions.ToString());

                if (vertexActive)
                {
                    graph.AddShaderChunk("// Vertex Graph Evaluation");
                    graph.Indent();
                    graph.AddShaderChunk(vertexGraphEvalFunction.ToString());
                    graph.Deindent();
                }

                graph.AddShaderChunk("// Pixel Graph Evaluation");
                graph.Indent();
                graph.AddShaderChunk(pixelGraphEvalFunction.ToString());
                graph.Deindent();
            }

            // build the hash table of all named fragments      TODO: could make this Dictionary<string, ShaderGenerator / string>  ?
            Dictionary<string, string> namedFragments = new Dictionary<string, string>();
            namedFragments.Add("InstancingOptions", instancingOptions.GetShaderString(0, false));
            namedFragments.Add("Defines", defines.GetShaderString(2, false));
            namedFragments.Add("Graph", graph.GetShaderString(2, false));
            namedFragments.Add("LightMode", pass.LightMode);
            namedFragments.Add("PassName", pass.Name);
            namedFragments.Add("Includes", shaderPassIncludes.GetShaderString(2, false));
            namedFragments.Add("Blending", blendCode.ToString());
            namedFragments.Add("Culling", cullCode.ToString());
            namedFragments.Add("ZTest", zTestCode.ToString());
            namedFragments.Add("ZWrite", zWriteCode.ToString());
            namedFragments.Add("ZClip", zClipCode.ToString());
            namedFragments.Add("Stencil", stencilCode.ToString());
            namedFragments.Add("ColorMask", colorMaskCode.ToString());

            // this is the format string for building the 'C# qualified assembly type names' for $buildType() commands
            string buildTypeAssemblyNameFormat = "UnityEditor.Rendering.Universal.UniversalShaderStructs+{0}, " + typeof(UniversalSubShaderUtilities).Assembly.FullName.ToString();

            string sharedTemplatePath = Path.Combine(Path.Combine("Packages/com.unity.render-pipelines.universal", "Editor"), "ShaderGraph/DuplicateGenCode");
            // process the template to generate the shader code for this pass
            ShaderSpliceUtil.TemplatePreprocessor templatePreprocessor =
                new ShaderSpliceUtil.TemplatePreprocessor(activeFields, namedFragments, debugOutput, sharedTemplatePath, sourceAssetDependencyPaths, buildTypeAssemblyNameFormat);

            templatePreprocessor.ProcessTemplateFile(templateLocation);

            result.AddShaderChunk(templatePreprocessor.GetShaderCode().ToString(), false);

            return true;
        }

        public static List<MaterialSlot> FindMaterialSlotsOnNode(IEnumerable<int> slots, AbstractMaterialNode node)
        {
            var activeSlots = new List<MaterialSlot>();
            if (slots != null)
            {
                foreach (var id in slots)
                {
                    MaterialSlot slot = node.FindSlot<MaterialSlot>(id);
                    if (slot != null)
                    {
                        activeSlots.Add(slot);
                    }
                }
            }
            return activeSlots;
        }

        public static void BuildRenderStatesFromPass(
            Pass pass,
            ShaderStringBuilder blendCode,
            ShaderStringBuilder cullCode,
            ShaderStringBuilder zTestCode,
            ShaderStringBuilder zWriteCode,
            ShaderStringBuilder zClipCode,
            ShaderStringBuilder stencilCode,
            ShaderStringBuilder colorMaskCode)
        {
            if (pass.BlendOverride != null)
                blendCode.AppendLine(pass.BlendOverride);

            if (pass.BlendOpOverride != null)
                blendCode.AppendLine(pass.BlendOpOverride);

            if (pass.CullOverride != null)
                cullCode.AppendLine(pass.CullOverride);

            if (pass.ZTestOverride != null)
                zTestCode.AppendLine(pass.ZTestOverride);

            if (pass.ZWriteOverride != null)
                zWriteCode.AppendLine(pass.ZWriteOverride);

            if (pass.ColorMaskOverride != null)
                colorMaskCode.AppendLine(pass.ColorMaskOverride);

            if (pass.ZClipOverride != null)
                zClipCode.AppendLine(pass.ZClipOverride);

            if (pass.StencilOverride != null)
            {
                foreach (var str in pass.StencilOverride)
                    stencilCode.AppendLine(str);
            }
        }
    }
}
