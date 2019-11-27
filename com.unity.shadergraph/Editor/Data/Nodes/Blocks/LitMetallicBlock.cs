using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    [Title("Lighting", "Lit Metallic")]
    class LitMetallicBlock : BlockData
    {
        public LitMetallicBlock()
        {
            name = "Lit Metallic";
            UpdateNodeAfterDeserialization();
        }

        const int kMetallicId = 0;
        const int kSmoothnessId = 1;
        const string kMetallicName = "Metallic";
        const string kSmoothnessName = "Smoothness";

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new Vector1MaterialSlot(kMetallicId, kMetallicName, kMetallicName, SlotType.Input, 0));
            AddSlot(new Vector1MaterialSlot(kSmoothnessId, kSmoothnessName, kSmoothnessName, SlotType.Input, 0.5f));
            RemoveSlotsNameNotMatching(new[] { kMetallicId, kSmoothnessId });
        }

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass)
        {
            return null;
        }
    }
}
