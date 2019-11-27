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
            AddSlot(new ColorRGBMaterialSlot(kColorId, kColorName, kColorName, SlotType.Input, Color.gray, ColorMode.Default));
            RemoveSlotsNameNotMatching(new[] { kColorId });
        }

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass)
        {
            return null;
        }
    }
}
