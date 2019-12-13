using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.Rendering.HighDefinition
{
    [Title("HDRP SurfaceData", "Distortion")]
    class DistortionBlock : BlockData
    {
        public DistortionBlock()
        {
            name = "Refraction Index";
            UpdateNodeAfterDeserialization();
        }

        public override Type contextType => typeof(FragmentContext);
        public override Type[] requireBlocks => null;

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new Vector2MaterialSlot(0, "Distortion", "Distortion", SlotType.Input, new Vector2(2.0f, -1.0f), ShaderStageCapability.Fragment));
            RemoveSlotsNameNotMatching(new[] { 0 });
        }

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass, List<BlockData> validBlocks)
        {
            return null;
        }
    }
}
