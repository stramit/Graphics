using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Data.Util;

namespace UnityEditor.Rendering.Universal
{
    [Serializable]
    [FormerName("UnityEditor.Experimental.Rendering.LightweightPipeline.LightWeightPBRSubShader")]
    [FormerName("UnityEditor.ShaderGraph.LightWeightPBRSubShader")]
    [FormerName("UnityEditor.Rendering.LWRP.LightWeightPBRSubShader")]
    class UniversalPBRSubShader : IPBRSubShader
    {
        Pass m_ForwardPassMetallic = new Pass
        {
            Name = "Universal Forward Metallic",
            LightMode = "UniversalForward",
            TemplateName = "universalPBRTemplateAF.template",
            MaterialName = "PBR",
            PixelShaderSlots = new List<int>
            {
                PBRMasterNode.AlbedoSlotId,
                PBRMasterNode.NormalSlotId,
                PBRMasterNode.EmissionSlotId,
                PBRMasterNode.MetallicSlotId,
                PBRMasterNode.SmoothnessSlotId,
                PBRMasterNode.OcclusionSlotId,
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
            RequiredFields = new List<string>()
            {
                "VaryingsMeshToPS.positionWS",
                "VaryingsMeshToPS.normalWS",
                "VaryingsMeshToPS.tangentWS", //needed for vertex lighting
                "VaryingsMeshToPS.texCoord1", //fog and vertex lighting, vert input is dependency
                "VaringsMeshToPS.texCoord2", //shadow coord, vert input is dependency
            },
            ExtraDefines = new List<string>(),
            Includes = new List<string>()
            {
                "#include \"Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/DuplicateIncludes/DepthOnlyPass.hlsl\"",
            },
            OnGeneratePassImpl = (IMasterNode node, ref Pass pass, ref ShaderGraphRequirements requirements) =>
            {
                pass.ExtraDefines.Clear();
                var masterNode = node as PBRMasterNode;
                GetSurfaceTagsOptions(masterNode, ref pass);
                if (masterNode.IsSlotConnected(PBRMasterNode.NormalSlotId))
                    pass.ExtraDefines.Add("#define _NORMALMAP 1");
                if (requirements.requiresDepthTexture)
                    pass.ExtraDefines.Add("#define REQUIRE_DEPTH_TEXTURE");
                if (requirements.requiresCameraOpaqueTexture)
                    pass.ExtraDefines.Add("#define REQUIRE_OPAQUE_TEXTURE");
            }
        };

        Pass m_ForwardPassSpecular = new Pass()
        {
            Name = "UniversalForward",
            TemplateName = "universalPBRForwardPass.template",
            PixelShaderSlots = new List<int>()
            {
                PBRMasterNode.AlbedoSlotId,
                PBRMasterNode.NormalSlotId,
                PBRMasterNode.EmissionSlotId,
                PBRMasterNode.SpecularSlotId,
                PBRMasterNode.SmoothnessSlotId,
                PBRMasterNode.OcclusionSlotId,
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
                var masterNode = node as PBRMasterNode;

                pass.ExtraDefines.Add("#define _SPECULAR_SETUP 1");
                if (masterNode.IsSlotConnected(PBRMasterNode.NormalSlotId))
                    pass.ExtraDefines.Add("#define _NORMALMAP 1");
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

        Pass m_ForwardPassMetallic2D = new Pass
        {
            Name = "Universal2D",
            TemplateName = "universal2DPBRPass.template",
            PixelShaderSlots = new List<int>
            {
                PBRMasterNode.AlbedoSlotId,
                PBRMasterNode.NormalSlotId,
                PBRMasterNode.EmissionSlotId,
                PBRMasterNode.MetallicSlotId,
                PBRMasterNode.SmoothnessSlotId,
                PBRMasterNode.OcclusionSlotId,
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
                var masterNode = node as PBRMasterNode;

                if (masterNode.IsSlotConnected(PBRMasterNode.NormalSlotId))
                    pass.ExtraDefines.Add("#define _NORMALMAP 1");
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

        Pass m_ForwardPassSpecular2D = new Pass()
        {
            Name = "Universal2D",
            TemplateName = "universal2DPBRPass.template",
            PixelShaderSlots = new List<int>()
            {
                PBRMasterNode.AlbedoSlotId,
                PBRMasterNode.NormalSlotId,
                PBRMasterNode.EmissionSlotId,
                PBRMasterNode.SpecularSlotId,
                PBRMasterNode.SmoothnessSlotId,
                PBRMasterNode.OcclusionSlotId,
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
                var masterNode = node as PBRMasterNode;

                pass.ExtraDefines.Add("#define _SPECULAR_SETUP 1");
                if (masterNode.IsSlotConnected(PBRMasterNode.NormalSlotId))
                    pass.ExtraDefines.Add("#define _NORMALMAP 1");
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
            TemplateName = "universalPBRTemplateAF.template",
            MaterialName = "PBR",
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
            ExtraDefines = new List<string>()
            {
            },
            Includes = new List<string>()
            {
                "#include \"Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/DuplicateIncludes/DepthOnlyPass.hlsl\"",
            },
            OnGeneratePassImpl = (IMasterNode node, ref Pass pass, ref ShaderGraphRequirements requirements) =>
            {
                pass.ExtraDefines.Clear();
                var masterNode = node as PBRMasterNode;
                GetSurfaceTagsOptions(masterNode, ref pass);
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
            TemplateName = "universalPBRTemplateAF.template",
            MaterialName = "PBR",
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
            RequiredFields = new List<string>()
            {
                "VaryingsMeshToPS.positionWS",
                "VaryingsMeshToPS.normalWS",
                "VaryingsMeshToPS.tangentWS", //fields needed for shadow bias in vert function
            },
            ExtraDefines = new List<string>(),
            Includes = new List<string>()
            {
                "#include \"Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/DuplicateIncludes/ShadowCasterPass.hlsl\"",
            },
            OnGeneratePassImpl = (IMasterNode node, ref Pass pass, ref ShaderGraphRequirements requirements) =>
            {
                var masterNode = node as PBRMasterNode;
                GetSurfaceTagsOptions(masterNode, ref pass);
            }
        };
        Pass m_LitMetaPass = new Pass()
        {
            Name = "meta",
            LightMode = "Meta",
            TemplateName = "universalPBRTemplateAF.template",
            MaterialName = "PBR",
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
            RequiredFields = new List<string>()
            {
                "AttributesMesh.uv1", //needed for meta vertex position
            },
            ExtraDefines = new List<string>(),
            Includes = new List<string>()
            {
                "#include \"Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/DuplicateIncludes/LightingMetaPass.hlsl\"",
            },
            OnGeneratePassImpl = (IMasterNode node, ref Pass pass, ref ShaderGraphRequirements requirements) =>
            {
                var masterNode = node as PBRMasterNode;
                GetSurfaceTagsOptions(masterNode, ref pass);
            }
        };

        public int GetPreviewPassIndex() { return 0; }

        public static void GetSurfaceTagsOptions(PBRMasterNode masterNode, ref Pass pass)
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

            PBRMasterNode masterNode = iMasterNode as PBRMasterNode;
            if (masterNode == null)
            {
                return activeFields;
            }

            if (masterNode.IsSlotConnected(PBRMasterNode.AlphaThresholdSlotId) ||
                masterNode.GetInputSlots<Vector1MaterialSlot>().First(x => x.id == PBRMasterNode.AlphaThresholdSlotId).value > 0.0f)
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

        private static bool GenerateShaderPassUnlit(PBRMasterNode masterNode, Pass pass, GenerationMode mode, ShaderGenerator result, List<string> sourceAssetDependencyPaths)
        {
            pass.OnGeneratePass(masterNode, pass.Requirements);

            // apply master node options to active fields
            var activeFields = GetActiveFieldsFromMasterNode(masterNode, pass);

            // use standard shader pass generation
            bool vertexActive = masterNode.IsSlotConnected(PBRMasterNode.PositionSlotId);
            return UniversalSubShaderUtilities.GenerateShaderPass(masterNode, pass, mode, activeFields, result, sourceAssetDependencyPaths, vertexActive, pass.PassTags);
        }

        public string GetSubshader(IMasterNode masterNode, GenerationMode mode, List<string> sourceAssetDependencyPaths = null)
        {
            if (sourceAssetDependencyPaths != null)
            {
                // UniversalPBRSubShader.cs
                sourceAssetDependencyPaths.Add(AssetDatabase.GUIDToAssetPath("ca91dbeb78daa054c9bbe15fef76361c"));
            }

            // Master Node data
            var pbrMasterNode = masterNode as PBRMasterNode;
            var subShader = new ShaderGenerator();

            subShader.AddShaderChunk("SubShader", true);
            subShader.AddShaderChunk("{", true);
            subShader.Indent();
            {
                var surfaceTags = ShaderGenerator.BuildMaterialTags(pbrMasterNode.surfaceType);
                var tagsBuilder = new ShaderStringBuilder(0);
                surfaceTags.GetTags(tagsBuilder, "UniversalPipeline");
                subShader.AddShaderChunk(tagsBuilder.ToString());
                
                GenerateShaderPassUnlit(pbrMasterNode, m_ShadowCasterPass, mode, subShader, sourceAssetDependencyPaths);
                GenerateShaderPassUnlit(pbrMasterNode, m_DepthOnlyPass, mode, subShader, sourceAssetDependencyPaths);
                GenerateShaderPassUnlit(pbrMasterNode, m_ForwardPassMetallic, mode, subShader, sourceAssetDependencyPaths);
                //GenerateShaderPassUnlit(pbrMasterNode, m_LitMetaPass, mode, subShader, sourceAssetDependencyPaths);
            }
            subShader.Deindent();
            subShader.AddShaderChunk("}", true);

            return subShader.GetShaderString(0);

            // Passes
            //var forwardPass = pbrMasterNode.model == PBRMasterNode.Model.Metallic ? m_ForwardPassMetallic : m_ForwardPassSpecular;
            //var forward2DPass = pbrMasterNode.model == PBRMasterNode.Model.Metallic ? m_ForwardPassMetallic2D : m_ForwardPassSpecular2D;
            //var passes = new Pass[] { forwardPass, m_DepthShadowPass, forward2DPass };

            //return UniversalSubShaderUtilities.GetSubShader<PBRMasterNode>(pbrMasterNode, tags, options,
            //    passes, mode, "UnityEditor.ShaderGraph.PBRMasterGUI", sourceAssetDependencyPaths);
        }

        public bool IsPipelineCompatible(RenderPipelineAsset renderPipelineAsset)
        {
            return renderPipelineAsset is UniversalRenderPipelineAsset;
        }

        public UniversalPBRSubShader() { }
    }
}
