// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using UnityEditor;
// using UnityEditor.Graphing;
// using UnityEditor.ShaderGraph;
// using UnityEngine.Rendering;
// using UnityEngine.Rendering.Universal;
// using UnityEditor.Rendering.Universal;
// using Data.Util;

// namespace UnityEditor.Experimental.Rendering.Universal
// {
//     [Serializable]
//     [FormerName("UnityEditor.Experimental.Rendering.LWRP.LightWeightSpriteUnlitSubShader")]
//     class UniversalSpriteUnlitSubShader : ISpriteUnlitSubShader
//     {
//         Pass m_UnlitPass = new Pass
//         {
//             Name = "UnlitPass",
//             TemplateName = "universalPBRTemplateAF.template",
//             LightMode = "UniversalForward",
//             ShaderPassName = "SPRITE_UNLIT",
//             PixelShaderSlots = new List<int>
//             {
//                 SpriteUnlitMasterNode.ColorSlotId,
//             },
//             VertexShaderSlots = new List<int>()
//             {
//                 SpriteUnlitMasterNode.PositionSlotId,
//             },
//             Requirements = new ShaderGraphRequirements()
//             {
//                 requiresVertexColor = true,
//                 requiresMeshUVs = new List<UVChannel>() { UVChannel.UV0 },
//             },
//             RequiredFields = new List<string>()
//             {
//                 "Attributes.color",
//                 "Attributes.uv0",
//                 "Varyings.color",
//                 "Varyings.texCoord0",
//                 "SurfaceDescriptionInputs.uv0",
//                 "SurfaceDescriptionInputs.VertexColor",
//                 "features.sprite",
//             },
//             ExtraDefines = new List<string>(),
//             Includes = new List<string>()
//             {
//                 "#include \"Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/DuplicateIncludes/SpriteUnlitPass.hlsl\"",
//             },
//             OnGeneratePassImpl = (IMasterNode node, ref Pass pass, ref ShaderGraphRequirements requirements) =>
//             {
//                 var masterNode = node as SpriteUnlitMasterNode;
//                 GetSurfaceTagsOptions(masterNode, ref pass);
//                 if (requirements.requiresDepthTexture)
//                     pass.ExtraDefines.Add("#define REQUIRE_DEPTH_TEXTURE");
//                 if (requirements.requiresCameraOpaqueTexture)
//                     pass.ExtraDefines.Add("#define REQUIRE_OPAQUE_TEXTURE");
//             }
//         };

//         public int GetPreviewPassIndex() { return 0; }

//         public static void GetSurfaceTagsOptions(SpriteUnlitMasterNode masterNode, ref Pass pass)
//         {
//             pass.PassOptions = ShaderGenerator.GetMaterialOptions(SurfaceType.Transparent, AlphaMode.Alpha, true);
            
//             pass.ZWriteOverride = "ZWrite " + pass.PassOptions.zWrite.ToString();
//             pass.ZTestOverride = "ZTest " + pass.PassOptions.zTest.ToString();
//             pass.CullOverride = "Cull " + pass.PassOptions.cullMode.ToString();
//             pass.BlendOpOverride = string.Format("Blend {0} {1}, {2} {3}", pass.PassOptions.srcBlend, pass.PassOptions.dstBlend, pass.PassOptions.alphaSrcBlend, pass.PassOptions.alphaDstBlend);

//         }

//         private static ActiveFields GetActiveFieldsFromMasterNode(AbstractMaterialNode iMasterNode, Pass pass)
//         {
//             var activeFields = new ActiveFields();
//             var baseActiveFields = activeFields.baseInstance;

//             SpriteUnlitMasterNode masterNode = iMasterNode as SpriteUnlitMasterNode;
//             if (masterNode == null)
//             {
//                 return activeFields;
//             }

//             baseActiveFields.Add("SurfaceType.Transparent");
//             baseActiveFields.Add("BlendMode.Alpha");

//             return activeFields;
//         }

//         private static bool GenerateShaderPassSpriteUnlit(SpriteUnlitMasterNode masterNode, Pass pass, GenerationMode mode, ShaderGenerator result, List<string> sourceAssetDependencyPaths)
//         {
//             pass.OnGeneratePass(masterNode, pass.Requirements);

//             // apply master node options to active fields
//             var activeFields = GetActiveFieldsFromMasterNode(masterNode, pass);

//             // use standard shader pass generation
//             bool vertexActive = masterNode.IsSlotConnected(UnlitMasterNode.PositionSlotId);
//             return UniversalSubShaderUtilities.GenerateShaderPass(masterNode, pass, mode, activeFields, result, sourceAssetDependencyPaths, vertexActive, pass.PassTags);
//         }
//         public string GetSubshader(IMasterNode masterNode, GenerationMode mode, List<string> sourceAssetDependencyPaths = null)
//         {
//             if (sourceAssetDependencyPaths != null)
//             {
//                 // LightWeightSpriteUnlitSubShader.cs
//                 sourceAssetDependencyPaths.Add(AssetDatabase.GUIDToAssetPath("f2df349d00ec920488971bb77440b7bc"));
//             }

//             // Master Node data
//             var unlitMasterNode = masterNode as SpriteUnlitMasterNode;
//             var subShader = new ShaderGenerator();

//             subShader.AddShaderChunk("SubShader", true);
//             subShader.AddShaderChunk("{", true);
//             subShader.Indent();
//             {
//                 var surfaceTags = ShaderGenerator.BuildMaterialTags(SurfaceType.Transparent);
//                 var tagsBuilder = new ShaderStringBuilder(0);
//                 surfaceTags.GetTags(tagsBuilder, "UniversalPipeline");
//                 subShader.AddShaderChunk(tagsBuilder.ToString());

//                 GenerateShaderPassSpriteUnlit(unlitMasterNode, m_UnlitPass, mode, subShader, sourceAssetDependencyPaths);
//             }
//             subShader.Deindent();
//             subShader.AddShaderChunk("}", true);

//             return subShader.GetShaderString(0);

//             //var tags = ShaderGenerator.BuildMaterialTags(SurfaceType.Transparent);
//             //var options = ShaderGenerator.GetMaterialOptions(SurfaceType.Transparent, AlphaMode.Alpha, true);
// //
//             // Passes
//             //var passes = new Pass[] { m_UnlitPass };
// //
//             //return UniversalSubShaderUtilities.GetSubShader<SpriteUnlitMasterNode>(unlitMasterNode, tags, options, 
//             //    passes, mode, sourceAssetDependencyPaths: sourceAssetDependencyPaths);
//         }

//         public bool IsPipelineCompatible(RenderPipelineAsset renderPipelineAsset)
//         {
//             return renderPipelineAsset is UniversalRenderPipelineAsset;
//         }

//         public UniversalSpriteUnlitSubShader() { }
//     }
// }