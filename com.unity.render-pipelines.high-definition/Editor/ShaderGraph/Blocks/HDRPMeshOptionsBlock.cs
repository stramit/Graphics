using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine.Rendering.HighDefinition;

using UnityEditor.Graphing;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEditor.Rendering.HighDefinition.Drawing;

// Include material common properties names
using static UnityEngine.Rendering.HighDefinition.HDMaterialProperties;

namespace UnityEditor.Rendering.HighDefinition.ShaderGraph
{
    [Title("HDRP", "HDRP Mesh Options")]
    class HDRPMeshOptionsBlock : BlockData, IHasSettings
    {
        // Must match HDShaderUtils.ShaderID layout
        // Index 0 here is Count_Standard
        public enum LightingType
        {
            Unlit,
            Lit,
        }

        public enum MaterialType
        {
            Standard,
            SubsurfaceScattering,
            Anisotropy,
            Iridescence,
            SpecularColor,
            Translucent
        } 

        [SerializeField]
        LightingType m_LightingType;

        public HDRPMeshOptionsBlock()
        {
            name = "HDRP Mesh Options";
        }

        public override Type contextType => typeof(OutputContext);
        public override Type[] requireBlocks => GetRequiredBlockTypes(true);

        [EnumControl("Lighting")]
        public LightingType lightingType
        {
            get => m_LightingType;
            set 
            {
                if (m_LightingType == value)
                    return;

                m_LightingType = value;
                owner.contextManager.DirtyBlock(this);
            }
        }

#region Lit
        // This includes all the shared settings

        [SerializeField]
        SurfaceType m_SurfaceType;

        public SurfaceType surfaceType
        {
            get { return m_SurfaceType; }
            set
            {
                if (m_SurfaceType == value)
                    return;

                m_SurfaceType = value;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        BlendMode m_BlendMode;

        public BlendMode blendMode
        {
            get { return m_BlendMode; }
            set
            {
                if (m_BlendMode == value)
                    return;

                m_BlendMode = value;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        HDRenderQueue.RenderQueueType m_RenderingPass = HDRenderQueue.RenderQueueType.Opaque;

        public HDRenderQueue.RenderQueueType renderingPass
        {
            get { return m_RenderingPass; }
            set
            {
                if (m_RenderingPass == value)
                    return;

                m_RenderingPass = value;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        bool m_BlendPreserveSpecular = true;

        public ToggleData blendPreserveSpecular
        {
            get { return new ToggleData(m_BlendPreserveSpecular); }
            set
            {
                if (m_BlendPreserveSpecular == value.isOn)
                    return;

                m_BlendPreserveSpecular = value.isOn;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        bool m_TransparencyFog = true;

        public ToggleData transparencyFog
        {
            get { return new ToggleData(m_TransparencyFog); }
            set
            {
                if (m_TransparencyFog == value.isOn)
                    return;

                m_TransparencyFog = value.isOn;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField, Obsolete("Kept for data migration")]
        internal bool m_DrawBeforeRefraction;

        [SerializeField]
        ScreenSpaceRefraction.RefractionModel m_RefractionModel;

        public ScreenSpaceRefraction.RefractionModel refractionModel
        {
            get { return m_RefractionModel; }
            set
            {
                if (m_RefractionModel == value)
                    return;

                m_RefractionModel = value;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        bool m_Distortion;

        public ToggleData distortion
        {
            get { return new ToggleData(m_Distortion); }
            set
            {
                if (m_Distortion == value.isOn)
                    return;
                
                m_Distortion = value.isOn;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        DistortionMode m_DistortionMode;

        public DistortionMode distortionMode
        {
            get { return m_DistortionMode; }
            set
            {
                if (m_DistortionMode == value)
                    return;

                m_DistortionMode = value;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        bool m_DistortionDepthTest = true;

        public ToggleData distortionDepthTest
        {
            get { return new ToggleData(m_DistortionDepthTest); }
            set
            {
                if (m_DistortionDepthTest == value.isOn)
                    return;

                m_DistortionDepthTest = value.isOn;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        bool m_AlphaTest;

        public ToggleData alphaTest
        {
            get { return new ToggleData(m_AlphaTest); }
            set
            {
                if (m_AlphaTest == value.isOn)
                    return;
                
                m_AlphaTest = value.isOn;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        bool m_AlphaTestDepthPrepass;

        public ToggleData alphaTestDepthPrepass
        {
            get { return new ToggleData(m_AlphaTestDepthPrepass); }
            set
            {
                if (m_AlphaTestDepthPrepass == value.isOn)
                    return;
                
                m_AlphaTestDepthPrepass = value.isOn;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        bool m_AlphaTestDepthPostpass;

        public ToggleData alphaTestDepthPostpass
        {
            get { return new ToggleData(m_AlphaTestDepthPostpass); }
            set
            {
                if (m_AlphaTestDepthPostpass == value.isOn)
                    return;

                m_AlphaTestDepthPostpass = value.isOn;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        bool m_TransparentWritesMotionVec;

        public ToggleData transparentWritesMotionVec
        {
            get { return new ToggleData(m_TransparentWritesMotionVec); }
            set
            {
                if (m_TransparentWritesMotionVec == value.isOn)
                    return;

                m_TransparentWritesMotionVec = value.isOn;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        bool m_AlphaTestShadow;

        public ToggleData alphaTestShadow
        {
            get { return new ToggleData(m_AlphaTestShadow); }
            set
            {
                if (m_AlphaTestShadow == value.isOn)
                    return;

                m_AlphaTestShadow = value.isOn;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        bool m_BackThenFrontRendering;

        public ToggleData backThenFrontRendering
        {
            get { return new ToggleData(m_BackThenFrontRendering); }
            set
            {
                if (m_BackThenFrontRendering == value.isOn)
                    return;
                
                m_BackThenFrontRendering = value.isOn;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        int m_SortPriority;

        public int sortPriority
        {
            get { return m_SortPriority; }
            set
            {
                if (m_SortPriority == value)
                    return;
                
                m_SortPriority = value;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        DoubleSidedMode m_DoubleSidedMode;

        public DoubleSidedMode doubleSidedMode
        {
            get { return m_DoubleSidedMode; }
            set
            {
                if (m_DoubleSidedMode == value)
                    return;

                m_DoubleSidedMode = value;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        MaterialType m_MaterialType;

        public MaterialType materialType
        {
            get { return m_MaterialType; }
            set
            {
                if (m_MaterialType == value)
                    return;

                m_MaterialType = value;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        bool m_SSSTransmission = true;

        public ToggleData sssTransmission
        {
            get { return new ToggleData(m_SSSTransmission); }
            set
            {
                if (m_SSSTransmission == value.isOn)
                    return;

                m_SSSTransmission = value.isOn;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        bool m_ReceiveDecals = true;

        public ToggleData receiveDecals
        {
            get { return new ToggleData(m_ReceiveDecals); }
            set
            {
                if (m_ReceiveDecals == value.isOn)
                    return;
                
                m_ReceiveDecals = value.isOn;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        bool m_ReceivesSSR = true;
        public ToggleData receiveSSR
        {
            get { return new ToggleData(m_ReceivesSSR); }
            set
            {
                if (m_ReceivesSSR == value.isOn)
                    return;

                m_ReceivesSSR = value.isOn;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        bool m_AddPrecomputedVelocity = false;

        public ToggleData addPrecomputedVelocity
        {
            get { return new ToggleData(m_AddPrecomputedVelocity); }
            set
            {
                if (m_AddPrecomputedVelocity == value.isOn)
                    return;
                    
                m_AddPrecomputedVelocity = value.isOn;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        bool m_EnergyConservingSpecular = true;

        public ToggleData energyConservingSpecular
        {
            get { return new ToggleData(m_EnergyConservingSpecular); }
            set
            {
                if (m_EnergyConservingSpecular == value.isOn)
                    return;

                m_EnergyConservingSpecular = value.isOn;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        bool m_SpecularAA;

        public ToggleData specularAA
        {
            get { return new ToggleData(m_SpecularAA); }
            set
            {
                if (m_SpecularAA == value.isOn)
                    return;

                m_SpecularAA = value.isOn;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        float m_SpecularAAScreenSpaceVariance;

        public float specularAAScreenSpaceVariance
        {
            get { return m_SpecularAAScreenSpaceVariance; }
            set
            {
                if (m_SpecularAAScreenSpaceVariance == value)
                    return;

                m_SpecularAAScreenSpaceVariance = value;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        float m_SpecularAAThreshold;

        public float specularAAThreshold
        {
            get { return m_SpecularAAThreshold; }
            set
            {
                if (m_SpecularAAThreshold == value)
                    return;

                m_SpecularAAThreshold = value;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        SpecularOcclusionMode m_SpecularOcclusionMode;

        public SpecularOcclusionMode specularOcclusionMode
        {
            get { return m_SpecularOcclusionMode; }
            set
            {
                if (m_SpecularOcclusionMode == value)
                    return;

                m_SpecularOcclusionMode = value;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        int m_DiffusionProfile;

        public int diffusionProfile
        {
            get { return m_DiffusionProfile; }
            set
            {
                if (m_DiffusionProfile == value)
                    return;

                m_DiffusionProfile = value;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        bool m_overrideBakedGI;

        public ToggleData overrideBakedGI
        {
            get { return new ToggleData(m_overrideBakedGI); }
            set
            {
                if (m_overrideBakedGI == value.isOn)
                    return;
                    
                m_overrideBakedGI = value.isOn;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        bool m_depthOffset;

        public ToggleData depthOffset
        {
            get { return new ToggleData(m_depthOffset); }
            set
            {
                if (m_depthOffset == value.isOn)
                    return;

                m_depthOffset = value.isOn;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        bool m_DOTSInstancing = false;
        public ToggleData dotsInstancing
        {
            get { return new ToggleData(m_DOTSInstancing); }
            set
            {
                if (m_DOTSInstancing == value.isOn)
                    return;

                m_DOTSInstancing = value.isOn;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        bool m_ZWrite = false;
        public ToggleData zWrite
        {
            get { return new ToggleData(m_ZWrite); }
            set
            {
                if (m_ZWrite == value.isOn)
                    return;

                m_ZWrite = value.isOn;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        TransparentCullMode m_transparentCullMode = TransparentCullMode.Back;
        public TransparentCullMode transparentCullMode
        {
            get => m_transparentCullMode;
            set
            {
                if (m_transparentCullMode == value)
                    return;

                m_transparentCullMode = value;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        CompareFunction m_ZTest = CompareFunction.LessEqual;
        public CompareFunction zTest
        {
            get => m_ZTest;
            set
            {
                if (m_ZTest == value)
                    return;

                m_ZTest = value;
                owner.contextManager.DirtyBlock(this);
            }
        }

        [SerializeField]
        bool m_SupportLodCrossFade;

        public ToggleData supportLodCrossFade
        {
            get { return new ToggleData(m_SupportLodCrossFade); }
            set
            {
                if (m_SupportLodCrossFade == value.isOn)
                    return;

                m_SupportLodCrossFade = value.isOn;
                owner.contextManager.DirtyBlock(this);
            }
        }

        // HDUnlit Specific
        [SerializeField]
        bool m_DistortionOnly = true;

        public ToggleData distortionOnly
        {
            get { return new ToggleData(m_DistortionOnly); }
            set
            {
                if (m_DistortionOnly == value.isOn)
                    return;
                
                m_DistortionOnly = value.isOn;
                owner.contextManager.DirtyBlock(this);
            }
        }

        ConditionalField[] GetConditionalFieldsLit(PassDescriptor pass, List<BlockData> validBlocks)
        {
            // We need this to know if there are any Dots properties active
            // Ideally we do this another way but HDLit needs this for conditional pragmas
            var shaderProperties = new PropertyCollector();
            owner.CollectShaderProperties(shaderProperties, GenerationMode.ForReals);
            bool hasDotsProperties = shaderProperties.GetDotsInstancingPropertiesCount(GenerationMode.ForReals) > 0;

            return new ConditionalField[]
            {
                new ConditionalField(Fields.LodCrossFade,                   supportLodCrossFade.isOn),
                
                // Structs
                new ConditionalField(HDStructFields.FragInputs.IsFrontFace,doubleSidedMode != DoubleSidedMode.Disabled &&
                                                                                !pass.Equals(HDPasses.HDLit.MotionVectors)),
                
                // Dots
                new ConditionalField(HDFields.DotsInstancing,               dotsInstancing.isOn),
                new ConditionalField(HDFields.DotsProperties,               hasDotsProperties),

                // Material
                new ConditionalField(HDFields.Anisotropy,                   materialType == MaterialType.Anisotropy),
                new ConditionalField(HDFields.Iridescence,                  materialType == MaterialType.Iridescence),
                new ConditionalField(HDFields.SpecularColor,                materialType == MaterialType.SpecularColor),
                new ConditionalField(HDFields.Standard,                     materialType == MaterialType.Standard),
                new ConditionalField(HDFields.SubsurfaceScattering,         materialType == MaterialType.SubsurfaceScattering &&
                                                                                surfaceType != SurfaceType.Transparent),
                new ConditionalField(HDFields.Transmission,                (materialType == MaterialType.SubsurfaceScattering && sssTransmission.isOn) ||
                                                                                (materialType == MaterialType.Translucent)),
                new ConditionalField(HDFields.Translucent,                 materialType == MaterialType.Translucent),

                // Surface Type
                new ConditionalField(Fields.SurfaceOpaque,                  surfaceType == SurfaceType.Opaque),
                new ConditionalField(Fields.SurfaceTransparent,             surfaceType != SurfaceType.Opaque),
                
                // Blend Mode
                new ConditionalField(Fields.BlendAdd,                       surfaceType != SurfaceType.Opaque && blendMode == BlendMode.Additive),
                new ConditionalField(Fields.BlendAlpha,                     surfaceType != SurfaceType.Opaque && blendMode == BlendMode.Alpha),
                new ConditionalField(Fields.BlendPremultiply,               surfaceType != SurfaceType.Opaque && blendMode == BlendMode.Premultiply),

                // Double Sided
                new ConditionalField(HDFields.DoubleSided,                  doubleSidedMode != DoubleSidedMode.Disabled),
                new ConditionalField(HDFields.DoubleSidedFlip,              doubleSidedMode == DoubleSidedMode.FlippedNormals &&
                                                                                !pass.Equals(HDPasses.HDLit.MotionVectors)),
                new ConditionalField(HDFields.DoubleSidedMirror,            doubleSidedMode == DoubleSidedMode.MirroredNormals &&
                                                                                !pass.Equals(HDPasses.HDLit.MotionVectors)),

                // Specular Occlusion
                new ConditionalField(HDFields.SpecularOcclusionFromAO,      specularOcclusionMode == SpecularOcclusionMode.FromAO),
                new ConditionalField(HDFields.SpecularOcclusionFromAOBentNormal, specularOcclusionMode == SpecularOcclusionMode.FromAOAndBentNormal),
                new ConditionalField(HDFields.SpecularOcclusionCustom,      specularOcclusionMode == SpecularOcclusionMode.Custom),

                //Distortion
                new ConditionalField(HDFields.TransparentDistortion,        surfaceType != SurfaceType.Opaque && distortion.isOn),

                // Refraction
                new ConditionalField(HDFields.Refraction,                   HasRefraction()),
                new ConditionalField(HDFields.RefractionBox,                HasRefraction() && refractionModel == ScreenSpaceRefraction.RefractionModel.Box),
                new ConditionalField(HDFields.RefractionSphere,             HasRefraction() && refractionModel == ScreenSpaceRefraction.RefractionModel.Sphere),

                // Misc
                new ConditionalField(Fields.AlphaTest,                      alphaTest.isOn),
                new ConditionalField(HDFields.AlphaTestShadow,              alphaTest.isOn && alphaTestShadow.isOn),
                new ConditionalField(HDFields.AlphaTestPrepass,             alphaTest.isOn && alphaTestDepthPrepass.isOn),
                new ConditionalField(HDFields.AlphaTestPostpass,            alphaTest.isOn && alphaTestDepthPostpass.isOn),
                new ConditionalField(HDFields.AlphaFog,                     surfaceType != SurfaceType.Opaque && transparencyFog.isOn),
                new ConditionalField(HDFields.BlendPreserveSpecular,        surfaceType != SurfaceType.Opaque && blendPreserveSpecular.isOn),
                new ConditionalField(HDFields.TransparentWritesMotionVec,   surfaceType != SurfaceType.Opaque && transparentWritesMotionVec.isOn),
                new ConditionalField(HDFields.DisableDecals,                !receiveDecals.isOn),
                new ConditionalField(HDFields.DisableSSR,                   !receiveSSR.isOn),
                new ConditionalField(Fields.VelocityPrecomputed,                addPrecomputedVelocity.isOn),
                new ConditionalField(HDFields.SpecularAA,                   specularAA.isOn),
                new ConditionalField(HDFields.EnergyConservingSpecular,     energyConservingSpecular.isOn),
                new ConditionalField(HDFields.BentNormal,                   validBlocks.Any(x => x is BentNormalTSBlock)),
                new ConditionalField(HDFields.AmbientOcclusion,             validBlocks.Any(x => x is AmbientOcclusionBlock)),
                new ConditionalField(HDFields.CoatMask,                     validBlocks.Any(x => x is CoatMaskBlock)),
                new ConditionalField(HDFields.Tangent,                      validBlocks.Any(x => x is TangentBlock)),
                new ConditionalField(HDFields.LightingGI,                   validBlocks.Any(x => x is BakedGIBlock)),
                new ConditionalField(HDFields.BackLightingGI,               validBlocks.Any(x => x is BakedBackGIBlock)),
                new ConditionalField(HDFields.DepthOffset,                  depthOffset.isOn && validBlocks.Any(x => x is DepthOffsetBlock)),
                new ConditionalField(HDFields.TransparentBackFace,          surfaceType != SurfaceType.Opaque && backThenFrontRendering.isOn),
                new ConditionalField(HDFields.TransparentDepthPrePass,      surfaceType != SurfaceType.Opaque && alphaTestDepthPrepass.isOn),
                new ConditionalField(HDFields.TransparentDepthPostPass,     surfaceType != SurfaceType.Opaque && alphaTestDepthPrepass.isOn),
            };
        }

        void ProcessPreviewMaterialLit(Material material)
        {
            // Fixup the material settings:
            material.SetFloat(kSurfaceType, (int)surfaceType);
            material.SetFloat(kDoubleSidedNormalMode, (int)doubleSidedMode);
            material.SetFloat(kAlphaCutoffEnabled, alphaTest.isOn ? 1 : 0);
            material.SetFloat(kBlendMode, (int)blendMode);
            material.SetFloat(kEnableFogOnTransparent, transparencyFog.isOn ? 1.0f : 0.0f);
            material.SetFloat(kZTestTransparent, (int)zTest);
            material.SetFloat(kTransparentCullMode, (int)transparentCullMode);
            material.SetFloat(kZWrite, zWrite.isOn ? 1.0f : 0.0f);
            // No sorting priority for shader graph preview
            material.renderQueue = (int)HDRenderQueue.ChangeType(renderingPass, offset: 0, alphaTest: alphaTest.isOn);
        }
#endregion

#region Unlit
        [SerializeField]
        bool m_DoubleSided;

        public ToggleData doubleSided
        {
            get { return new ToggleData(m_DoubleSided); }
            set
            {
                if (m_DoubleSided == value.isOn)
                    return;
                
                m_DoubleSided = value.isOn;
                owner.contextManager.DirtyBlock(this);
            }
        }

        ConditionalField[] GetConditionalFieldsUnlit(PassDescriptor pass, List<BlockData> validBlocks)
        {
            return new ConditionalField[]
            {
                // Distortion
                new ConditionalField(HDFields.DistortionDepthTest,          distortionDepthTest.isOn),
                new ConditionalField(HDFields.DistortionAdd,                distortionMode == DistortionMode.Add),
                new ConditionalField(HDFields.DistortionMultiply,           distortionMode == DistortionMode.Multiply),
                new ConditionalField(HDFields.DistortionReplace,            distortionMode == DistortionMode.Replace),
                new ConditionalField(HDFields.TransparentDistortion,        surfaceType != SurfaceType.Opaque && distortion.isOn),
                
                // Misc
                new ConditionalField(Fields.AlphaTest,                      alphaTest.isOn),
                new ConditionalField(HDFields.AlphaFog,                     surfaceType != SurfaceType.Opaque && transparencyFog.isOn),
                new ConditionalField(Fields.VelocityPrecomputed,            addPrecomputedVelocity.isOn),
            };
        }

        void ProcessPreviewMaterialUnlit(Material material)
        {
            // Fixup the material settings:
            material.SetFloat(kSurfaceType, (int)surfaceType);
            material.SetFloat(kDoubleSidedEnable, doubleSided.isOn ? 1.0f : 0.0f);
            material.SetFloat(kAlphaCutoffEnabled, alphaTest.isOn ? 1 : 0);
            material.SetFloat(kBlendMode, (int)blendMode);
            material.SetFloat(kEnableFogOnTransparent, transparencyFog.isOn ? 1.0f : 0.0f);
            material.SetFloat(kZTestTransparent, (int)zTest);
            material.SetFloat(kTransparentCullMode, (int)transparentCullMode);
            material.SetFloat(kZWrite, zWrite.isOn ? 1.0f : 0.0f);
            // No sorting priority for shader graph preview
            material.renderQueue = (int)HDRenderQueue.ChangeType(renderingPass, offset: 0, alphaTest: alphaTest.isOn);  
        }
#endregion

#region Helpers
        public bool HasRefraction()
        {
            return (surfaceType == SurfaceType.Transparent && renderingPass != HDRenderQueue.RenderQueueType.PreRefraction && refractionModel != ScreenSpaceRefraction.RefractionModel.None);
        }

        public bool HasDistortion()
        {
            return (surfaceType == SurfaceType.Transparent && distortion.isOn);
        }
#endregion

        public Type[] GetRequiredBlockTypes(bool onlyRequired)
        {
            List<Type> types = new List<Type>();

            if(!onlyRequired) types.Add(typeof(VertexPositionBlock));
            if(!onlyRequired) types.Add(typeof(VertexNormalBlock));
            if(!onlyRequired) types.Add(typeof(VertexTangentBlock));

            types.Add(typeof(BaseColorBlock));

            if(!onlyRequired) types.Add(typeof(EmissionBlock));

            if(HasDistortion())
            {
                types.Add(typeof(DistortionBlock));
                types.Add(typeof(DistortionBlurBlock));
            }

            if(surfaceType == SurfaceType.Transparent || alphaTest.isOn)
            {
                types.Add(typeof(AlphaBlock));
            }

            if(alphaTest.isOn)
            {
                types.Add(typeof(ClipThresholdBlock));
            }

            if(lightingType == LightingType.Lit)
            {
                types.Add(typeof(SmoothnessBlock));

                if(!onlyRequired) types.Add(typeof(NormalTSBlock));
                if(!onlyRequired) types.Add(typeof(BentNormalTSBlock));
                if(!onlyRequired) types.Add(typeof(CoatMaskBlock));
                if(!onlyRequired) types.Add(typeof(AmbientOcclusionBlock));

                switch(materialType)
                {
                    case MaterialType.Standard:
                        types.Add(typeof(MetallicBlock));
                        break;
                    case MaterialType.SubsurfaceScattering:
                        types.Add(typeof(SubsurfaceMaskBlock));
                        types.Add(typeof(DiffusionProfileHashBlock));
                        if(sssTransmission.isOn)
                        {
                            types.Add(typeof(ThicknessBlock));
                        }
                        break;
                    case MaterialType.Anisotropy:
                        types.Add(typeof(MetallicBlock));
                        types.Add(typeof(AnisotropyBlock));
                        types.Add(typeof(TangentBlock));
                        break;
                    case MaterialType.Iridescence:
                        types.Add(typeof(MetallicBlock));
                        types.Add(typeof(IridescenceMaskBlock));
                        types.Add(typeof(IridescenceThicknessBlock));
                        break;
                    case MaterialType.SpecularColor:
                        types.Add(typeof(SpecularColorBlock));
                        break;
                    case MaterialType.Translucent:
                        types.Add(typeof(ThicknessBlock));
                        types.Add(typeof(DiffusionProfileHashBlock));
                        break;
                }

                if(alphaTest.isOn)
                {
                    if(alphaTestDepthPrepass.isOn && surfaceType == SurfaceType.Transparent)
                    {
                        types.Add(typeof(ClipThresholdDepthPrepassBlock));
                    } 
                    if(alphaTestDepthPostpass.isOn && surfaceType == SurfaceType.Transparent)
                    {
                        types.Add(typeof(ClipThresholdDepthPostpassBlock));
                    } 
                    if(alphaTestShadow.isOn)
                    {
                        types.Add(typeof(ClipThresholdShadowBlock));
                    } 
                }

                if(specularOcclusionMode == SpecularOcclusionMode.Custom)
                {
                    types.Add(typeof(SpecularOcclusionBlock));
                }

                if(overrideBakedGI.isOn)
                {
                    types.Add(typeof(BakedGIBlock));
                    types.Add(typeof(BakedBackGIBlock));
                }

                if(specularAA.isOn)
                {
                    types.Add(typeof(SpecularAAScreenSpaceVarianceBlock));
                    types.Add(typeof(SpecularAAThresholdBlock));
                }

                if(HasRefraction())
                {
                    types.Add(typeof(RefractionIndexBlock));
                    types.Add(typeof(RefractionColorBlock));
                    types.Add(typeof(RefractionDistanceBlock));
                    types.Add(typeof(ThicknessBlock));
                }

                if(depthOffset.isOn)
                {
                    types.Add(typeof(DepthOffsetBlock));
                }
            }

            return types.ToArray();
        }

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass, List<BlockData> validBlocks)
        {
            switch(lightingType)
            {
                case LightingType.Lit:
                    return GetConditionalFieldsLit(pass, validBlocks);
                case LightingType.Unlit:
                    return GetConditionalFieldsUnlit(pass, validBlocks);
                default:
                    throw new Exception($"Lighting type {lightingType} not implemented.");
            }
        }

        public override void ProcessPreviewMaterial(Material material)
        {
            // TODO: Currently HDRP preview only works if these are set
            switch(lightingType)
            {
                case LightingType.Lit:
                    ProcessPreviewMaterialLit(material);
                    break;
                case LightingType.Unlit:
                    ProcessPreviewMaterialUnlit(material);
                    break;
                default:
                    throw new Exception($"Lighting type {lightingType} not implemented.");
            }
        }

        public VisualElement CreateSettingsElement()
        {
            switch(lightingType)
            {
                case LightingType.Lit:
                    return new HDLitSettingsView(this);
                case LightingType.Unlit:
                    return new HDUnlitSettingsView(this);
                default:
                    throw new Exception($"Lighting type {lightingType} not implemented.");
            }
        }

        public override void CollectShaderProperties(PropertyCollector collector, GenerationMode generationMode)
        {
            // Trunk currently relies on checking material property "_EmissionColor" to allow emissive GI. If it doesn't find that property, or it is black, GI is forced off.
            // ShaderGraph doesn't use this property, so currently it inserts a dummy color (white). This dummy color may be removed entirely once the following PR has been merged in trunk: Pull request #74105
            // The user will then need to explicitly disable emissive GI if it is not needed.
            // To be able to automatically disable emission based on the ShaderGraph config when emission is black,
            // we will need a more general way to communicate this to the engine (not directly tied to a material property).
            collector.AddShaderProperty(new ColorShaderProperty()
            {
                overrideReferenceName = "_EmissionColor",
                hidden = true,
                value = new Color(1.0f, 1.0f, 1.0f, 1.0f)
            });

            // ShaderGraph only property used to send the RenderQueueType to the material
            collector.AddShaderProperty(new Vector1ShaderProperty
            {
                overrideReferenceName = "_RenderQueueType",
                hidden = true,
                value = (int)renderingPass,
            });

            //See SG-ADDITIONALVELOCITY-NOTE
            if (addPrecomputedVelocity.isOn)
            {
                collector.AddShaderProperty(new BooleanShaderProperty
                {
                    value  = true,
                    hidden = true,
                    overrideReferenceName = kAddPrecomputedVelocity,
                });
            }

            var hasRefraction = lightingType != LightingType.Unlit && HasRefraction();
            var hasReceiveSSR = lightingType != LightingType.Unlit && receiveSSR.isOn;
            var hasBackThenFrontRendering = lightingType != LightingType.Unlit && backThenFrontRendering.isOn;
            var hasAlphaTestShadow = lightingType != LightingType.Unlit && alphaTestShadow.isOn;
            var hasDoubleSidedMode = lightingType != LightingType.Unlit ?
                doubleSidedMode : 
                (doubleSided.isOn ? DoubleSidedMode.Enabled : DoubleSidedMode.Disabled);

            HDSubShaderUtilities.AddStencilShaderProperties(collector, hasRefraction, hasReceiveSSR);
            HDSubShaderUtilities.AddBlendingStatesShaderProperties(
                collector,
                surfaceType,
                blendMode,
                sortPriority,
                zWrite.isOn,
                transparentCullMode,
                zTest,
                hasBackThenFrontRendering
            );
            HDSubShaderUtilities.AddAlphaCutoffShaderProperties(collector, alphaTest.isOn, hasAlphaTestShadow);
            HDSubShaderUtilities.AddDoubleSidedProperty(collector, hasDoubleSidedMode);

            base.CollectShaderProperties(collector, generationMode);
        }
    }
}
