using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    [Title("Basic", "Alpha Test")]
    class AlphaTestBlock : BlockData
    {
        public AlphaTestBlock()
        {
            name = "Alpha Test";
            UpdateNodeAfterDeserialization();
        }

        const int kThresholdId = 0;
        const string kThresholdName = "Threshold";

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new Vector1MaterialSlot(kThresholdId, kThresholdName, "AlphaClipThreshold", SlotType.Input, 0.5f));
            RemoveSlotsNameNotMatching(new[] { kThresholdId });
        }

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass)
        {
            return new ConditionalField[]
            {
                new ConditionalField(Fields.AlphaClip, true),
                new ConditionalField(Fields.AlphaTest, true),
            };
        }
    }
}
