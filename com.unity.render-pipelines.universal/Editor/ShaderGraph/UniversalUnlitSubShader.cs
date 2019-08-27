using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data.Util;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEditor.Rendering.Universal
{
    [Serializable]
    [FormerName("UnityEngine.Experimental.Rendering.LightweightPipeline.LightWeightUnlitSubShader")]
    [FormerName("UnityEditor.ShaderGraph.LightWeightUnlitSubShader")]
    [FormerName("UnityEditor.Rendering.LWRP.LightWeightUnlitSubShader")]
    [FormerName("UnityEngine.Rendering.LWRP.LightWeightUnlitSubShader")]
    class UniversalUnlitSubShader : IUnlitSubShader
    {
        ShaderPass m_UnlitPass = new ShaderPass
        {
            // Definition
            displayName = "UnlitPass",
            referenceName = "FORWARD_UNLIT",
            lightMode = "UniversalForward",
            useInPreview = true,

            // Port mask
            vertexPorts = new List<int>()
            {
                UnlitMasterNode.PositionSlotId
            },
            pixelPorts = new List<int>
            {
                UnlitMasterNode.ColorSlotId,
                UnlitMasterNode.AlphaSlotId,
                UnlitMasterNode.AlphaThresholdSlotId
            },

            // Includes = new List<string>()
            // {
            //     "#include \"Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/DuplicateIncludes/UnlitForwardPass.hlsl\"",
            // },
            // OnGeneratePassImpl = (IMasterNode node, ref Pass pass, ref ShaderGraphRequirements requirements) =>
            // {
            //     pass.ExtraDefines.Clear();
            //     var masterNode = node as UnlitMasterNode;
            //     GetSurfaceTagsOptions(masterNode, ref pass);
            //     if (requirements.requiresDepthTexture)
            //         pass.ExtraDefines.Add("#define REQUIRE_DEPTH_TEXTURE");
            //     if (requirements.requiresCameraOpaqueTexture)
            //         pass.ExtraDefines.Add("#define REQUIRE_OPAQUE_TEXTURE");
            // }
        };

        ShaderPass m_DepthOnlyPass = new ShaderPass()
        {
            // Definition
            displayName = "DepthOnly",
            referenceName = "DEPTHONLY",
            lightMode = "DepthOnly",

            // Port mask
            vertexPorts = new List<int>()
            {
                PBRMasterNode.PositionSlotId
            },
            pixelPorts = new List<int>()
            {
                PBRMasterNode.AlphaSlotId,
                PBRMasterNode.AlphaThresholdSlotId
            },

            // Render State Overrides
            ZWriteOverride = "ZWrite On",
            ColorMaskOverride = "ColorMask 0",

            // Includes = new List<string>()
            // {
            //     "#include \"Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/DuplicateIncludes/DepthOnlyPass.hlsl\"",
            // },
            // OnGeneratePassImpl = (IMasterNode node, ref Pass pass, ref ShaderGraphRequirements requirements) =>
            // {
            //     pass.ExtraDefines.Clear();
            //     var masterNode = node as UnlitMasterNode;
            //     GetSurfaceTagsOptions(masterNode, ref pass);
            //     if (requirements.requiresDepthTexture)
            //         pass.ExtraDefines.Add("#define REQUIRE_DEPTH_TEXTURE");
            //     if (requirements.requiresCameraOpaqueTexture)
            //         pass.ExtraDefines.Add("#define REQUIRE_OPAQUE_TEXTURE");
            // }
        };

        ShaderPass m_ShadowCasterPass = new ShaderPass()
        {
            // Definition
            displayName = "ShadowCaster",
            referenceName = "SHADOWCASTER",
            lightMode = "ShadowCaster",

            // Render State Overrides
            ZWriteOverride = "ZWrite On",
            ZTestOverride = "ZTest LEqual",
            
            // Port mask
            vertexPorts = new List<int>()
            {
                PBRMasterNode.PositionSlotId
            },
            pixelPorts = new List<int>()
            {
                PBRMasterNode.AlphaSlotId,
                PBRMasterNode.AlphaThresholdSlotId
            },

            // Required fields
            requiredAttributes = new List<string>()
            {
                "Attributes.normalOS", 
            },
            
            // Includes = new List<string>()
            // {
            //     "#include \"Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/DuplicateIncludes/ShadowCasterPass.hlsl\"",
            // },
            // OnGeneratePassImpl = (IMasterNode node, ref Pass pass, ref ShaderGraphRequirements requirements) =>
            // {
            //     var masterNode = node as UnlitMasterNode;
            //     GetSurfaceTagsOptions(masterNode, ref pass);
            // }
        };
        
        public int GetPreviewPassIndex() { return 0; }

        private static ActiveFields GetActiveFieldsFromMasterNode(AbstractMaterialNode iMasterNode, ShaderPass pass)
        {
            var activeFields = new ActiveFields();
            var baseActiveFields = activeFields.baseInstance;

            UnlitMasterNode masterNode = iMasterNode as UnlitMasterNode;
            if (masterNode == null)
            {
                return activeFields;
            }

            if (masterNode.IsSlotConnected(UnlitMasterNode.AlphaThresholdSlotId) ||
                masterNode.GetInputSlots<Vector1MaterialSlot>().First(x => x.id == UnlitMasterNode.AlphaThresholdSlotId).value > 0.0f)
            {
                baseActiveFields.Add("AlphaClip");
            }

            // Keywords for transparent
            // #pragma shader_feature _SURFACE_TYPE_TRANSPARENT
            if (masterNode.surfaceType != ShaderGraph.SurfaceType.Opaque)
            {
                // transparent-only defines
                baseActiveFields.Add("SurfaceType.Transparent");

                // #pragma shader_feature _ _BLENDMODE_ALPHA _BLENDMODE_ADD _BLENDMODE_PRE_MULTIPLY
                if (masterNode.alphaMode == AlphaMode.Alpha)
                {
                    baseActiveFields.Add("BlendMode.Alpha");
                }
                else if (masterNode.alphaMode == AlphaMode.Additive)
                {
                    baseActiveFields.Add("BlendMode.Add");
                }
                else if (masterNode.alphaMode == AlphaMode.Premultiply)
                {
                    baseActiveFields.Add("BlendMode.Premultiply");
                }
            }
            else
            {
                // opaque-only defines
            }

            return activeFields;
        }

        private static bool GenerateShaderPass(UnlitMasterNode masterNode, ShaderPass pass, GenerationMode mode, ShaderGenerator result, List<string> sourceAssetDependencyPaths)
        {
            UniversalSubShaderUtilities.SetRenderState(masterNode.surfaceType, masterNode.alphaMode, masterNode.twoSided.isOn, ref pass);

            // apply master node options to active fields
            var activeFields = GetActiveFieldsFromMasterNode(masterNode, pass);

            // use standard shader pass generation
            return UniversalSubShaderUtilities.GenerateShaderPass(masterNode, pass, mode, activeFields, result, sourceAssetDependencyPaths);
        }

        public string GetSubshader(IMasterNode masterNode, GenerationMode mode, List<string> sourceAssetDependencyPaths = null)
        {
            if (sourceAssetDependencyPaths != null)
            {
                // LightWeightPBRSubShader.cs
                sourceAssetDependencyPaths.Add(AssetDatabase.GUIDToAssetPath("3ef30c5c1d5fc412f88511ef5818b654"));
            }

            // Master Node data
            var unlitMasterNode = masterNode as UnlitMasterNode;
            var subShader = new ShaderGenerator();

            subShader.AddShaderChunk("SubShader", true);
            subShader.AddShaderChunk("{", true);
            subShader.Indent();
            {
                var surfaceTags = ShaderGenerator.BuildMaterialTags(unlitMasterNode.surfaceType);
                var tagsBuilder = new ShaderStringBuilder(0);
                surfaceTags.GetTags(tagsBuilder, "UniversalPipeline");
                subShader.AddShaderChunk(tagsBuilder.ToString());
                
                GenerateShaderPass(unlitMasterNode, m_ShadowCasterPass, mode, subShader, sourceAssetDependencyPaths);
                GenerateShaderPass(unlitMasterNode, m_DepthOnlyPass, mode, subShader, sourceAssetDependencyPaths);
                GenerateShaderPass(unlitMasterNode, m_UnlitPass, mode, subShader, sourceAssetDependencyPaths);
            }
            subShader.Deindent();
            subShader.AddShaderChunk("}", true);

            return subShader.GetShaderString(0);

            //var tags = ShaderGenerator.BuildMaterialTags(unlitMasterNode.surfaceType);
            //var options = ShaderGenerator.GetMaterialOptions(unlitMasterNode.surfaceType, unlitMasterNode.alphaMode, unlitMasterNode.twoSided.isOn);
//
            // Passes
            //var passes = new Pass[] { m_UnlitPass, m_DepthShadowPass };
//
            //return UniversalSubShaderUtilities.GetSubShader<UnlitMasterNode>(unlitMasterNode, tags, options, 
            //    passes, mode, sourceAssetDependencyPaths: sourceAssetDependencyPaths);
        }

        public bool IsPipelineCompatible(RenderPipelineAsset renderPipelineAsset)
        {
            return renderPipelineAsset is UniversalRenderPipelineAsset;
        }

        public UniversalUnlitSubShader() { }
    }
}
