using System;
using System.Collections.Generic;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.Rendering.HighDefinition
{
    [Title("HDRP SurfaceData", "Thickness")]
    class ThicknessBlock : BlockData
    {
        public ThicknessBlock()
        {
            name = "Thickness";
            UpdateNodeAfterDeserialization();
        }

        public override Type contextType => typeof(FragmentContext);
        public override Type[] requireBlocks => null;

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new Vector1MaterialSlot(0, "Thickness", "Thickness", SlotType.Input, 1, ShaderStageCapability.Fragment));
            RemoveSlotsNameNotMatching(new[] { 0 });
        }

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass, List<BlockData> validBlocks)
        {
            return null;
        }
    }
}
