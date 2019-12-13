using System;
using System.Collections.Generic;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    [Title("SurfaceData", "Alpha")]
    class AlphaBlock : BlockData
    {
        public AlphaBlock()
        {
            name = "Alpha";
            UpdateNodeAfterDeserialization();
        }

        public override Type contextType => typeof(FragmentContext);
        public override Type[] requireBlocks => null;

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new Vector1MaterialSlot(0, "Alpha", "Alpha", SlotType.Input, 1));
            RemoveSlotsNameNotMatching(new[] { 0 });
        }

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass, List<BlockData> validBlocks)
        {
            return null;
        }
    }
}
