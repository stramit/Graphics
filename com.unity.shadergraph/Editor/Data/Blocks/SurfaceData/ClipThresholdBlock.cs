using System;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    [Title("SurfaceData", "Clip Threshold")]
    class ClipThresholdBlock : BlockData
    {
        public ClipThresholdBlock()
        {
            name = "Clip Threshold";
            UpdateNodeAfterDeserialization();
        }

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new Vector1MaterialSlot(0, "Clip Threshold", "AlphaClipThreshold", SlotType.Input, 0.5f));
            RemoveSlotsNameNotMatching(new[] { 0 });
        }

        public override Type contextType => typeof(FragmentContext);
        public override Type[] requireBlocks => null;

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass)
        {
            return null;
        }
    }
}
