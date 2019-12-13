using System;
using System.Collections.Generic;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.Rendering.HighDefinition
{
    [Title("HDRP SurfaceData", "Anisotropy")]
    class AnisotropyBlock : BlockData
    {
        public AnisotropyBlock()
        {
            name = "Anisotropy";
            UpdateNodeAfterDeserialization();
        }

        public override Type contextType => typeof(FragmentContext);
        public override Type[] requireBlocks => null;

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new Vector1MaterialSlot(0, "Anisotropy", "Anisotropy", SlotType.Input, 0, ShaderStageCapability.Fragment));
            RemoveSlotsNameNotMatching(new[] { 0 });
        }

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass, List<BlockData> validBlocks)
        {
            return null;
        }
    }
}
