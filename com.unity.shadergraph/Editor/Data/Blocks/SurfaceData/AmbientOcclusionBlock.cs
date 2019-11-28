using System;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    [Title("SurfaceData", "Ambient Occlusion")]
    class AmbientOcclusionBlock : BlockData
    {
        public AmbientOcclusionBlock()
        {
            name = "Ambient Occlusion";
            UpdateNodeAfterDeserialization();
        }
        
        public override Type contextType => typeof(FragmentContext);
        public override Type[] requireBlocks => null;

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new Vector1MaterialSlot(0, "Ambient Occlusion", "Occlusion", SlotType.Input, 1));
            RemoveSlotsNameNotMatching(new[] { 0 });
        }

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass)
        {
            return null;
        }
    }
}
