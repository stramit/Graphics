using UnityEngine;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    [Title("Lighting", "Lit Sprite")]
    class LitSpriteBlock : BlockData
    {
        public LitSpriteBlock()
        {
            name = "Lit Sprite";
            UpdateNodeAfterDeserialization();
        }

        const int kMaskId = 0;
        const string kMaskName = "Mask";

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new ColorRGBAMaterialSlot(kMaskId, kMaskName, kMaskName, SlotType.Input, Color.white));
            RemoveSlotsNameNotMatching(new[] { kMaskId });
        }

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass)
        {
            return null;
        }
    }
}
