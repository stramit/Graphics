using System;
using UnityEngine;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    [Title("SurfaceData", "Sprite Mask")]
    class SpriteMaskBlock : BlockData
    {
        public SpriteMaskBlock()
        {
            name = "Sprite Mask";
            UpdateNodeAfterDeserialization();
        }
        
        public override Type contextType => typeof(FragmentContext);
        public override Type[] requireBlocks => null;

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new ColorRGBAMaterialSlot(0, "Sprite Mask", "Mask", SlotType.Input, Color.white));
            RemoveSlotsNameNotMatching(new[] { 0 });
        }

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass)
        {
            return null;
        }
    }
}
