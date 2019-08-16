using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data.Util;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
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
        Pass m_UnlitPass = new Pass
        {
            Name = "UnlitPass",
            LightMode = "UniversalForward",
            TemplateName = "universalUnlitPassAF.template",
            MaterialName = "Unlit",
            PixelShaderSlots = new List<int>
            {
                UnlitMasterNode.ColorSlotId,
                UnlitMasterNode.AlphaSlotId,
                UnlitMasterNode.AlphaThresholdSlotId
            },
            VertexShaderSlots = new List<int>()
            {
                UnlitMasterNode.PositionSlotId
            },
            Requirements = new ShaderGraphRequirements()
            {
                requiresNormal = UniversalSubShaderUtilities.k_PixelCoordinateSpace,
                requiresTangent = UniversalSubShaderUtilities.k_PixelCoordinateSpace,
                requiresBitangent = UniversalSubShaderUtilities.k_PixelCoordinateSpace,
                requiresPosition = UniversalSubShaderUtilities.k_PixelCoordinateSpace,
                requiresViewDir = UniversalSubShaderUtilities.k_PixelCoordinateSpace,
                requiresMeshUVs = new List<UVChannel>() { UVChannel.UV1 },
            },
            RequiredFields = new List<string>() 
            {
                "AttributesMesh.uv1",
            },
            ExtraDefines = new List<string>(),
            Includes = new List<string>()
            {
                "#include \"Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/DuplicateIncludes/UnlitForwardPass.hlsl\"",
            },
            UseInPreview = true,
            OnGeneratePassImpl = (IMasterNode node, ref Pass pass, ref ShaderGraphRequirements requirements) =>
            {
                var masterNode = node as UnlitMasterNode;
                GetSurfaceTagsOptions(masterNode, ref pass);
                if (masterNode.IsSlotConnected(PBRMasterNode.AlphaThresholdSlotId))
                    pass.ExtraDefines.Add("#define _AlphaClip 1");
                if (masterNode.surfaceType == SurfaceType.Transparent && masterNode.alphaMode == AlphaMode.Premultiply)
                    pass.ExtraDefines.Add("#define _ALPHAPREMULTIPLY_ON 1");
                if (requirements.requiresDepthTexture)
                    pass.ExtraDefines.Add("#define REQUIRE_DEPTH_TEXTURE");
                if (requirements.requiresCameraOpaqueTexture)
                    pass.ExtraDefines.Add("#define REQUIRE_OPAQUE_TEXTURE");
            }
        };

        Pass m_DepthOnlyPass = new Pass()
        {
            Name = "DepthOnly",
            LightMode = "DepthOnly",
            TemplateName = "universalUnlitPassAF.template",
            MaterialName = "Unlit",
            ZWriteOverride = "ZWrite On",
            ColorMaskOverride = "ColorMask 0",
            PixelShaderSlots = new List<int>()
            {
                PBRMasterNode.AlphaSlotId,
                PBRMasterNode.AlphaThresholdSlotId
            },
            VertexShaderSlots = new List<int>()
            {
                PBRMasterNode.PositionSlotId
            },
            Requirements = new ShaderGraphRequirements()
            {
                requiresNormal = UniversalSubShaderUtilities.k_PixelCoordinateSpace,
                requiresTangent = UniversalSubShaderUtilities.k_PixelCoordinateSpace,
                requiresBitangent = UniversalSubShaderUtilities.k_PixelCoordinateSpace,
                requiresPosition = UniversalSubShaderUtilities.k_PixelCoordinateSpace,
                requiresViewDir = UniversalSubShaderUtilities.k_PixelCoordinateSpace,
                requiresMeshUVs = new List<UVChannel>() { UVChannel.UV1 },
            },
            ExtraDefines = new List<string>()
            {
            },
            Includes = new List<string>()
            {
                "#include \"Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/DuplicateIncludes/DepthOnlyPass.hlsl\"",
            },
            OnGeneratePassImpl = (IMasterNode node, ref Pass pass, ref ShaderGraphRequirements requirements) =>
            {
                var masterNode = node as UnlitMasterNode;
                GetSurfaceTagsOptions(masterNode, ref pass);
                if (masterNode.IsSlotConnected(PBRMasterNode.AlphaThresholdSlotId))
                    pass.ExtraDefines.Add("#define _AlphaClip 1");
                if (masterNode.surfaceType == SurfaceType.Transparent && masterNode.alphaMode == AlphaMode.Premultiply)
                    pass.ExtraDefines.Add("#define _ALPHAPREMULTIPLY_ON 1");
                if (requirements.requiresDepthTexture)
                    pass.ExtraDefines.Add("#define REQUIRE_DEPTH_TEXTURE");
                if (requirements.requiresCameraOpaqueTexture)
                    pass.ExtraDefines.Add("#define REQUIRE_OPAQUE_TEXTURE");
            }
        };

        Pass m_ShadowCasterPass = new Pass()
        {
            Name = "ShadowCaster",
            LightMode = "ShadowCaster",
            TemplateName = "universalUnlitPass.template",
            MaterialName = "Unlit",
            ZWriteOverride = "ZWrite On",
            ZTestOverride = "ZTest LEqual",
            PixelShaderSlots = new List<int>()
            {
                PBRMasterNode.AlphaSlotId,
                PBRMasterNode.AlphaThresholdSlotId
            },
            VertexShaderSlots = new List<int>()
            {
                PBRMasterNode.PositionSlotId
            },
            Requirements = new ShaderGraphRequirements()
            {
                requiresNormal = UniversalSubShaderUtilities.k_PixelCoordinateSpace,
                requiresTangent = UniversalSubShaderUtilities.k_PixelCoordinateSpace,
                requiresBitangent = UniversalSubShaderUtilities.k_PixelCoordinateSpace,
                requiresPosition = UniversalSubShaderUtilities.k_PixelCoordinateSpace,
                requiresViewDir = UniversalSubShaderUtilities.k_PixelCoordinateSpace,
                requiresMeshUVs = new List<UVChannel>() { UVChannel.UV1 },
            },
            ExtraDefines = new List<string>(),
            OnGeneratePassImpl = (IMasterNode node, ref Pass pass, ref ShaderGraphRequirements requirements) =>
            {
                var masterNode = node as UnlitMasterNode;
                GetSurfaceTagsOptions(masterNode, ref pass);
            }
        };
        
        public int GetPreviewPassIndex() { return 0; }

        public static void GetSurfaceTagsOptions(UnlitMasterNode masterNode, ref Pass pass)
        {
            pass.PassTags = ShaderGenerator.BuildMaterialTags(masterNode.surfaceType);
            pass.PassOptions = ShaderGenerator.GetMaterialOptions(masterNode.surfaceType, masterNode.alphaMode, masterNode.twoSided.isOn);
            
            pass.ZWriteOverride = "ZWrite " + pass.PassOptions.zWrite.ToString();
            pass.ZTestOverride = "ZTest " + pass.PassOptions.zTest.ToString();
            pass.CullOverride = "Cull " + pass.PassOptions.cullMode.ToString();
            pass.BlendOpOverride = string.Format("Blend {0} {1}, {2} {3}", pass.PassOptions.srcBlend, pass.PassOptions.dstBlend, pass.PassOptions.alphaSrcBlend, pass.PassOptions.alphaDstBlend);

        }

        private static ActiveFields GetActiveFieldsFromMasterNode(AbstractMaterialNode iMasterNode, Pass pass)
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
                baseActiveFields.Add("AlphaTest");
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
            }
            else
            {
                // opaque-only defines
            }

            return activeFields;
        }

        private static bool GenerateShaderPassUnlit(UnlitMasterNode masterNode, Pass pass, GenerationMode mode, ShaderGenerator result, List<string> sourceAssetDependencyPaths)
        {
            pass.OnGeneratePass(masterNode, pass.Requirements);

            // apply master node options to active fields
            var activeFields = GetActiveFieldsFromMasterNode(masterNode, pass);

            // use standard shader pass generation
            bool vertexActive = masterNode.IsSlotConnected(UnlitMasterNode.PositionSlotId);
            return UniversalSubShaderUtilities.GenerateShaderPass(masterNode, pass, mode, activeFields, result, sourceAssetDependencyPaths, vertexActive, pass.PassTags);
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
                
                //GenerateShaderPassUnlit(unlitMasterNode, m_ShadowCasterPass, mode, subShader, sourceAssetDependencyPaths);
                GenerateShaderPassUnlit(unlitMasterNode, m_DepthOnlyPass, mode, subShader, sourceAssetDependencyPaths);
                GenerateShaderPassUnlit(unlitMasterNode, m_UnlitPass, mode, subShader, sourceAssetDependencyPaths);
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
