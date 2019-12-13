using System;
using System.Collections.Generic;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.Rendering.HighDefinition
{
    [Title("HDRP SurfaceData", "Clip Threshold Depth Prepass")]
    class ClipThresholdDepthPrepassBlock : BlockData
    {
        public ClipThresholdDepthPrepassBlock()
        {
            name = "Clip Threshold Depth Prepass";
            UpdateNodeAfterDeserialization();
        }

        public override Type contextType => typeof(FragmentContext);
        public override Type[] requireBlocks => null;

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new Vector1MaterialSlot(0, "ClipThresholdDepthPrepass", "AlphaClipThresholdDepthPrepass", SlotType.Input, 0.5f, ShaderStageCapability.Fragment));
            RemoveSlotsNameNotMatching(new[] { 0 });
        }

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass, List<BlockData> validBlocks)
        {
            return null;
        }
    }
}
