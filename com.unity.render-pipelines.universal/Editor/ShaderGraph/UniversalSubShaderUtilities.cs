using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using Data.Util;
using UnityEngine;              // Vector3,4

namespace UnityEditor.Rendering.Universal
{
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
            [Optional][PreprocessorIf("defined(LIGHTMAP_ON)")]
            Vector2 lightmapUV;
            [Optional][PreprocessorIf("!defined(LIGHTMAP_ON)")]
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
                new Dependency("Varyings.bitangentWS",      "Attributes.normalOS"),
                new Dependency("Varyings.bitangentWS",      "Attributes.tangentOS"),
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
        public static void SetRenderState(SurfaceType surfaceType, AlphaMode alphaMode, bool twoSided, ref ShaderPass pass)
        {
            // Get default render state from Master Node
            var options = ShaderGenerator.GetMaterialOptions(surfaceType, alphaMode, twoSided);
            
            // Update render state on ShaderPass if there is no active override
            if(string.IsNullOrEmpty(pass.ZWriteOverride))
            {
                pass.ZWriteOverride = "ZWrite " + options.zWrite.ToString();
            }

            if(string.IsNullOrEmpty(pass.ZTestOverride))
            {
                pass.ZTestOverride = "ZTest " + options.zTest.ToString();
            }

            if(string.IsNullOrEmpty(pass.CullOverride))
            {
                pass.CullOverride = "Cull " + options.cullMode.ToString();
            }
            
            if(string.IsNullOrEmpty(pass.BlendOverride))
            {
                pass.BlendOverride = string.Format("Blend {0} {1}, {2} {3}", options.srcBlend, options.dstBlend, options.alphaSrcBlend, options.alphaDstBlend);
            }
        }

        static string GetTemplatePath(string templateName)
        {
            var basePath = "Packages/com.unity.shadergraph/Editor/Templates/";
            string templatePath = Path.Combine(basePath, templateName);

            if (File.Exists(templatePath))
                return templatePath;

            throw new FileNotFoundException(string.Format(@"Cannot find a template with name ""{0}"".", templateName));
        }

        static List<Dependency[]> k_Dependencies = new List<Dependency[]>()
        {
            UniversalShaderStructs.Varyings.standardDependencies,
            UniversalShaderStructs.SurfaceDescriptionInputs.dependencies,
            UniversalShaderStructs.VertexDescriptionInputs.dependencies
        };

        static void GetUpstreamNodesForShaderPass(AbstractMaterialNode masterNode, ShaderPass pass, out List<AbstractMaterialNode> vertexNodes, out List<AbstractMaterialNode> pixelNodes)
        {
            // Traverse Graph Data
            vertexNodes = Graphing.ListPool<AbstractMaterialNode>.Get();
            NodeUtils.DepthFirstCollectNodesFromNode(vertexNodes, masterNode, NodeUtils.IncludeSelf.Include, pass.vertexPorts);

            pixelNodes = Graphing.ListPool<AbstractMaterialNode>.Get();
            NodeUtils.DepthFirstCollectNodesFromNode(pixelNodes, masterNode, NodeUtils.IncludeSelf.Include, pass.pixelPorts);
        }

        static void GetActiveFieldsAndPermutationsForNodes(AbstractMaterialNode masterNode, ShaderPass pass, 
            KeywordCollector keywordCollector,  List<AbstractMaterialNode> vertexNodes, List<AbstractMaterialNode> pixelNodes,
            List<int>[] vertexNodePermutations, List<int>[] pixelNodePermutations,
            ActiveFields activeFields, out ShaderGraphRequirementsPerKeyword graphRequirements)
        {
            // Initialize requirements
            ShaderGraphRequirementsPerKeyword pixelRequirements = new ShaderGraphRequirementsPerKeyword();
            ShaderGraphRequirementsPerKeyword vertexRequirements = new ShaderGraphRequirementsPerKeyword();
            graphRequirements = new ShaderGraphRequirementsPerKeyword();

            // Evaluate all Keyword permutations
            if (keywordCollector.permutations.Count > 0)
            {
                for(int i = 0; i < keywordCollector.permutations.Count; i++)
                {
                    // Get active nodes for this permutation
                    var localVertexNodes = UnityEngine.Rendering.ListPool<AbstractMaterialNode>.Get();
                    var localPixelNodes = UnityEngine.Rendering.ListPool<AbstractMaterialNode>.Get();
                    NodeUtils.DepthFirstCollectNodesFromNode(localVertexNodes, masterNode, NodeUtils.IncludeSelf.Include, pass.vertexPorts, keywordCollector.permutations[i]);
                    NodeUtils.DepthFirstCollectNodesFromNode(localPixelNodes, masterNode, NodeUtils.IncludeSelf.Include, pass.pixelPorts, keywordCollector.permutations[i]);

                    // Track each vertex node in this permutation
                    foreach(AbstractMaterialNode vertexNode in localVertexNodes)
                    {
                        int nodeIndex = vertexNodes.IndexOf(vertexNode);

                        if(vertexNodePermutations[nodeIndex] == null)
                            vertexNodePermutations[nodeIndex] = new List<int>();
                        vertexNodePermutations[nodeIndex].Add(i);
                    }

                    // Track each pixel node in this permutation
                    foreach(AbstractMaterialNode pixelNode in localPixelNodes)
                    {
                        int nodeIndex = pixelNodes.IndexOf(pixelNode);

                        if(pixelNodePermutations[nodeIndex] == null)
                            pixelNodePermutations[nodeIndex] = new List<int>();
                        pixelNodePermutations[nodeIndex].Add(i);
                    }

                    // Get requirements for this permutation
                    vertexRequirements[i].SetRequirements(ShaderGraphRequirements.FromNodes(localVertexNodes, ShaderStageCapability.Vertex, false));
                    pixelRequirements[i].SetRequirements(ShaderGraphRequirements.FromNodes(localPixelNodes, ShaderStageCapability.Fragment, false));

                    // Add active fields
                    UniversalShaderStructs.AddActiveFieldsFromVertexGraphRequirements(activeFields[i], vertexRequirements[i].requirements);
                    UniversalShaderStructs.AddActiveFieldsFromPixelGraphRequirements(activeFields[i], pixelRequirements[i].requirements);
                }
            }
            // No Keywords
            else
            {
                // Get requirements
                vertexRequirements.baseInstance.SetRequirements(ShaderGraphRequirements.FromNodes(vertexNodes, ShaderStageCapability.Vertex, false));
                pixelRequirements.baseInstance.SetRequirements(ShaderGraphRequirements.FromNodes(pixelNodes, ShaderStageCapability.Fragment, false));

                // Add active fields
                UniversalShaderStructs.AddActiveFieldsFromVertexGraphRequirements(activeFields.baseInstance, vertexRequirements.baseInstance.requirements);
                UniversalShaderStructs.AddActiveFieldsFromPixelGraphRequirements(activeFields.baseInstance, pixelRequirements.baseInstance.requirements);
            }
            
            // Build graph requirements
            graphRequirements.UnionWith(pixelRequirements);
            graphRequirements.UnionWith(vertexRequirements);
        }

        public static bool GenerateShaderPass(AbstractMaterialNode masterNode, ShaderPass pass, GenerationMode mode, ActiveFields activeFields, ShaderGenerator result, List<string> sourceAssetDependencyPaths)
        {
            // --------------------------------------------------
            // Setup

            // Initiailize Collectors
            var propertyCollector = new PropertyCollector();
            var keywordCollector = new KeywordCollector();
            masterNode.owner.CollectShaderKeywords(keywordCollector, mode);

            // Get upstream nodes from ShaderPass port mask
            List<AbstractMaterialNode> vertexNodes;
            List<AbstractMaterialNode> pixelNodes;
            GetUpstreamNodesForShaderPass(masterNode, pass, out vertexNodes, out pixelNodes);

            // Track permutation indices for all nodes
            List<int>[] vertexNodePermutations = new List<int>[vertexNodes.Count];
            List<int>[] pixelNodePermutations = new List<int>[pixelNodes.Count];

            // Get active fields from upstream Node requirements
            ShaderGraphRequirementsPerKeyword graphRequirements;
            GetActiveFieldsAndPermutationsForNodes(masterNode, pass, keywordCollector, vertexNodes, pixelNodes,
                vertexNodePermutations, pixelNodePermutations, activeFields, out graphRequirements);

            // GET CUSTOM ACTIVE FIELDS HERE!

            // Get active fields from ShaderPass
            UniversalShaderStructs.AddRequiredFields(pass.requiredAttributes, activeFields.baseInstance);
            UniversalShaderStructs.AddRequiredFields(pass.requiredVaryings, activeFields.baseInstance);

            // Get Port references from ShaderPass
            var pixelSlots = UniversalSubShaderUtilities.FindMaterialSlotsOnNode(pass.pixelPorts, masterNode);
            var vertexSlots = UniversalSubShaderUtilities.FindMaterialSlotsOnNode(pass.vertexPorts, masterNode);                     

            // Function Registry
            var functionBuilder = new ShaderStringBuilder(1);
            var functionRegistry = new FunctionRegistry(functionBuilder);

            // Hash table of named $splice(name) commands
            // Key: splice token
            // Value: string to splice
            Dictionary<string, string> spliceCommands = new Dictionary<string, string>();

            // --------------------------------------------------
            // Pass Setup

            spliceCommands.Add("PassName", pass.displayName);
            spliceCommands.Add("LightMode", pass.lightMode);
            UniversalSubShaderUtilities.BuildRenderStatesFromPass(pass, ref spliceCommands);

            // --------------------------------------------------
            // Pass Code

            // Pragmas
            using (var passPragmaBuilder = new ShaderStringBuilder())
            {
                if(pass.pragmas != null)
                {
                    foreach(string pragma in pass.pragmas)
                    {
                        passPragmaBuilder.AppendLine($"#pragma {pragma}");
                    }
                }
                spliceCommands.Add("PassPragmas", passPragmaBuilder.ToString());
            }

            // Includes
            using (var passIncludeBuilder = new ShaderStringBuilder())
            {
                if(pass.includes != null)
                {
                    foreach(string include in pass.includes)
                    {
                        passIncludeBuilder.AppendLine($"#include \"{include}\"");
                    }
                }
                spliceCommands.Add("PassIncludes", passIncludeBuilder.ToString());
            }

            // Keywords
            using (var passKeywordBuilder = new ShaderStringBuilder())
            {
                if(pass.keywords != null)
                {
                    foreach(KeywordDescriptor keyword in pass.keywords)
                    {
                        passKeywordBuilder.AppendLine(keyword.ToDeclarationString());
                    }
                }
                spliceCommands.Add("PassKeywords", passKeywordBuilder.ToString());
            }

            // --------------------------------------------------
            // Graph Vertex

            var vertexBuilder = new ShaderStringBuilder();

            // If vertex modification enabled
            if (activeFields.baseInstance.Contains("features.modifyMesh"))
            {
                // Setup
                string vertexGraphInputName = "VertexDescriptionInputs";
                string vertexGraphOutputName = "VertexDescription";
                string vertexGraphFunctionName = "VertexDescriptionFunction";
                var vertexGraphInputGenerator = new ShaderGenerator();
                var vertexGraphFunctionBuilder = new ShaderStringBuilder();
                var vertexGraphOutputBuilder = new ShaderStringBuilder();

                // Build vertex graph inputs
                ShaderSpliceUtil.BuildType(typeof(UniversalShaderStructs.VertexDescriptionInputs), activeFields, vertexGraphInputGenerator);

                // Build vertex graph outputs
                // Add struct fields to active fields
                SubShaderGenerator.GenerateVertexDescriptionStruct(vertexGraphOutputBuilder, vertexSlots, vertexGraphOutputName, activeFields.baseInstance);

                // Build vertex graph functions from ShaderPass vertex port mask
                SubShaderGenerator.GenerateVertexDescriptionFunction(
                    masterNode.owner as GraphData,
                    vertexGraphFunctionBuilder,
                    functionRegistry,
                    propertyCollector,
                    keywordCollector,
                    mode,
                    masterNode,
                    vertexNodes,
                    vertexNodePermutations,
                    vertexSlots,
                    vertexGraphInputName,
                    vertexGraphFunctionName,
                    vertexGraphOutputName);

                // Generate final shader strings
                vertexBuilder.AppendLines(vertexGraphInputGenerator.GetShaderString(0, false));
                vertexBuilder.AppendNewLine();
                vertexBuilder.AppendLines(vertexGraphOutputBuilder.ToString());
                vertexBuilder.AppendNewLine();
                vertexBuilder.AppendLines(vertexGraphFunctionBuilder.ToString());
                vertexBuilder.AppendNewLine();
            }

            // Add to splice commands
            spliceCommands.Add("GraphVertex", vertexBuilder.ToString());

            // --------------------------------------------------
            // Graph Pixel

            // Setup
            string pixelGraphInputName = "SurfaceDescriptionInputs";
            string pixelGraphOutputName = "SurfaceDescription";
            string pixelGraphFunctionName = "SurfaceDescriptionFunction";
            var pixelGraphInputGenerator = new ShaderGenerator();
            var pixelGraphOutputBuilder = new ShaderStringBuilder();
            var pixelGraphFunctionBuilder = new ShaderStringBuilder();

            // Build pixel graph inputs
            ShaderSpliceUtil.BuildType(typeof(UniversalShaderStructs.SurfaceDescriptionInputs), activeFields, pixelGraphInputGenerator);

            // Build pixel graph outputs
            // Add struct fields to active fields
            SubShaderGenerator.GenerateSurfaceDescriptionStruct(pixelGraphOutputBuilder, pixelSlots, pixelGraphOutputName, activeFields.baseInstance);

            // Build pixel graph functions from ShaderPass pixel port mask
            SubShaderGenerator.GenerateSurfaceDescriptionFunction(
                pixelNodes,
                pixelNodePermutations,
                masterNode,
                masterNode.owner as GraphData,
                pixelGraphFunctionBuilder,
                functionRegistry,
                propertyCollector,
                keywordCollector,
                mode,
                pixelGraphFunctionName,
                pixelGraphOutputName,
                null,
                pixelSlots,
                pixelGraphInputName);

            using (var pixelBuilder = new ShaderStringBuilder())
            {
                // Generate final shader strings
                pixelBuilder.AppendLines(pixelGraphInputGenerator.GetShaderString(0, false));
                pixelBuilder.AppendNewLine();
                pixelBuilder.AppendLines(pixelGraphOutputBuilder.ToString());
                pixelBuilder.AppendNewLine();
                pixelBuilder.AppendLines(pixelGraphFunctionBuilder.ToString());
                pixelBuilder.AppendNewLine();
                
                // Add to splice commands
                spliceCommands.Add("GraphPixel", pixelBuilder.ToString());
            }

            // --------------------------------------------------
            // Graph Functions

            spliceCommands.Add("GraphFunctions", functionBuilder.ToString());

            // --------------------------------------------------
            // Graph Keywords

            using (var keywordBuilder = new ShaderStringBuilder())
            {
                keywordCollector.GetKeywordsDeclaration(keywordBuilder, mode);
                spliceCommands.Add("GraphKeywords", keywordBuilder.ToString());
            }

            // --------------------------------------------------
            // Graph Properties

            using (var propertyBuilder = new ShaderStringBuilder())
            {
                propertyCollector.GetPropertiesDeclaration(propertyBuilder, mode, masterNode.owner.concretePrecision);
                spliceCommands.Add("GraphProperties", propertyBuilder.ToString());
            }

            // --------------------------------------------------
            // Graph Defines

            using (var graphDefines = new ShaderStringBuilder())
            {
                graphDefines.AppendLine("#define SHADERPASS {0}", pass.referenceName);

                if (graphRequirements.permutationCount > 0)
                {
                    List<int> activePermutationIndices;

                    // Depth Texture
                    activePermutationIndices = graphRequirements.allPermutations.instances
                        .Where(p => p.requirements.requiresDepthTexture)
                        .Select(p => p.permutationIndex)
                        .ToList();
                    if (activePermutationIndices.Count > 0)
                    {
                        graphDefines.AppendLine(KeywordUtil.GetKeywordPermutationSetConditional(activePermutationIndices));
                        graphDefines.AppendLine("#define REQUIRE_DEPTH_TEXTURE");
                        graphDefines.AppendLine("#endif");
                    }

                    // Opaque Texture
                    activePermutationIndices = graphRequirements.allPermutations.instances
                        .Where(p => p.requirements.requiresCameraOpaqueTexture)
                        .Select(p => p.permutationIndex)
                        .ToList();
                    if (activePermutationIndices.Count > 0)
                    {
                        graphDefines.AppendLine(KeywordUtil.GetKeywordPermutationSetConditional(activePermutationIndices));
                        graphDefines.AppendLine("#define REQUIRE_OPAQUE_TEXTURE");
                        graphDefines.AppendLine("#endif");
                    }
                }
                else
                {
                    // Depth Texture
                    if (graphRequirements.baseInstance.requirements.requiresDepthTexture)
                        graphDefines.AppendLine("#define REQUIRE_DEPTH_TEXTURE");

                    // Opaque Texture
                    if (graphRequirements.baseInstance.requirements.requiresCameraOpaqueTexture)
                        graphDefines.AppendLine("#define REQUIRE_OPAQUE_TEXTURE");
                }

                // Add to splice commands
                spliceCommands.Add("GraphDefines", graphDefines.ToString());
            }

            // --------------------------------------------------
            // Main

            // Main include is expected to contain vert/frag definitions for the pass
            // This must be defined after all graph code
            spliceCommands.Add("MainInclude", $"#include \"{pass.mainInclude}\"");

            // --------------------------------------------------
            // Debug

            bool debugOutput = false;
            // // debug output all active fields
            // var interpolatorDefines = new ShaderGenerator();
            // 
            // if (debugOutput)
            // {
            //     interpolatorDefines.AddShaderChunk("// ACTIVE FIELDS:");
            //     foreach (string f in activeFields.baseInstance.fields)
            //     {
            //         interpolatorDefines.AddShaderChunk("//   " + f);
            //     }
            // }

            // --------------------------------------------------
            // Finalize

            // Propagate active field requirements using dependencies
            foreach (var instance in activeFields.all.instances)
                ShaderSpliceUtil.ApplyDependencies(instance, k_Dependencies);

            // Get Template
            string templateLocation = GetTemplatePath("PassMesh.template");

            if (!File.Exists(templateLocation))
                return false;

            // Format string for building 'C# qualified assembly type names' for $buildType() commands
            string buildTypeAssemblyNameFormat = "UnityEditor.Rendering.Universal.UniversalShaderStructs+{0}, " + typeof(UniversalSubShaderUtilities).Assembly.FullName.ToString();

            // Get Template preprocessor
            string sharedTemplatePath = Path.Combine(Path.Combine("Packages/com.unity.render-pipelines.universal", "Editor"), "ShaderGraph/DuplicateGenCode");
            var templatePreprocessor = new ShaderSpliceUtil.TemplatePreprocessor(activeFields, spliceCommands, 
                debugOutput, sharedTemplatePath, sourceAssetDependencyPaths, buildTypeAssemblyNameFormat);
            
            // Process Template
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

        public static void BuildRenderStatesFromPass(ShaderPass pass, ref Dictionary<string, string> spliceCommands)
        {
            spliceCommands.Add("Blending", pass.BlendOverride != null ? pass.BlendOverride : string.Empty);
            spliceCommands.Add("Culling", pass.CullOverride != null ? pass.CullOverride : string.Empty);
            spliceCommands.Add("ZTest", pass.ZTestOverride != null ? pass.ZTestOverride : string.Empty);
            spliceCommands.Add("ZWrite", pass.ZWriteOverride != null ? pass.ZWriteOverride : string.Empty);
            spliceCommands.Add("ZClip", pass.ZClipOverride != null ? pass.ZClipOverride : string.Empty);
            spliceCommands.Add("ColorMask", pass.ColorMaskOverride != null ? pass.ColorMaskOverride : string.Empty);

            using(var stencilBuilder = new ShaderStringBuilder())
            {
                if (pass.StencilOverride != null)
                {
                    foreach (var str in pass.StencilOverride)
                        stencilBuilder.AppendLine(str);
                }
                
                spliceCommands.Add("Stencil", stencilBuilder.ToString());
            }
        }
    }
}
