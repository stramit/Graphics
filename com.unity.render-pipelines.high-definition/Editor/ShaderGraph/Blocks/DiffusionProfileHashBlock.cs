using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine.Rendering.HighDefinition;

namespace UnityEditor.Rendering.HighDefinition
{
    [Title("HDRP SurfaceData", "DiffusionProfileHash")]
    class DiffusionProfileHashBlock : BlockData
    {
        public DiffusionProfileHashBlock()
        {
            name = "DiffusionProfileHash";
            UpdateNodeAfterDeserialization();
        }

        public override Type contextType => typeof(FragmentContext);
        public override Type[] requireBlocks => null;

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new DiffusionProfileInputMaterialSlot(0, "DiffusionProfileHash", "DiffusionProfileHash", ShaderStageCapability.Fragment));
            RemoveSlotsNameNotMatching(new[] { 0 });
        }

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass, List<BlockData> validBlocks)
        {
            return null;
        }

        protected override bool CalculateNodeHasError(ref string errorMessage)
        {
            if (!(HDRenderPipeline.currentAsset is HDRenderPipelineAsset hdPipelineAsset))
            {
                errorMessage = $"Render pipeline asset is invalid.";
                return true;
            }

            var diffusionProfileSlot = FindSlot<DiffusionProfileInputMaterialSlot>(0);
            if (diffusionProfileSlot == null)
            {
                errorMessage = $"Slot {diffusionProfileSlot.RawDisplayName()} is invalid.";
                return true;
            }

            if ((diffusionProfileSlot.diffusionProfile) != null && !hdPipelineAsset.diffusionProfileSettingsList.Any(d => d == diffusionProfileSlot.diffusionProfile))
            {
                errorMessage = $"Diffusion profile '{diffusionProfileSlot.diffusionProfile.name}' is not referenced in the current HDRP asset.";
                return true;
            }

            return base.CalculateNodeHasError(ref errorMessage);
        }
    }
}
