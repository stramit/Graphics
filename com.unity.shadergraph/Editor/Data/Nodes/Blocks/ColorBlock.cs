using UnityEditor.Graphing;
using UnityEngine;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    [Title("Basic", "Color")]
    class ColorBlock : BlockData
    {
        public ColorBlock()
        {
            name = "Color";
            UpdateNodeAfterDeserialization();
        }
        
        const int kColorId = 0;
        const string kColorName = "Color";

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new DynamicVectorMaterialSlot(kColorId, kColorName, kColorName, SlotType.Input, Vector4.zero));
            RemoveSlotsNameNotMatching(new[] { kColorId });
        }

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass)
        {
            return null;
        }
    }
}
