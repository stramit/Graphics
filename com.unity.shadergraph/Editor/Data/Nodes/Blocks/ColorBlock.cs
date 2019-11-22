using UnityEditor.Graphing;
using UnityEngine;

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

        const int InputSlotId = 0;
        const string kInputSlotName = "Color";

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new DynamicVectorMaterialSlot(InputSlotId, kInputSlotName, kInputSlotName, SlotType.Input, Vector4.zero));
            RemoveSlotsNameNotMatching(new[] { InputSlotId });
        }
    }
}
