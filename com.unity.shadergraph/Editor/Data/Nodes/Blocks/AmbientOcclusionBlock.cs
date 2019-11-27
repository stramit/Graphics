using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    [Title("Basic", "Ambient Occlusion")]
    class AmbientOcclusionBlock : BlockData
    {
        public AmbientOcclusionBlock()
        {
            name = "Ambient Occlusion";
            UpdateNodeAfterDeserialization();
        }
        
        const int kOcclusionId = 0;
        const string kOcclusionName = "Occlusion";

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new Vector1MaterialSlot(kOcclusionId, kOcclusionName, kOcclusionName, SlotType.Input, 1));
            RemoveSlotsNameNotMatching(new[] { kOcclusionId });
        }

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass)
        {
            return null;
        }
    }
}
