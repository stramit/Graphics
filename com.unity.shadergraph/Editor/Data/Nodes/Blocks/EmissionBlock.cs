using UnityEngine;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    [Title("Basic", "Emission")]
    class EmissionBlock : BlockData
    {
        public EmissionBlock()
        {
            name = "Emission";
            UpdateNodeAfterDeserialization();
        }

        const int kEmissionId = 0;
        const string kEmissionName = "Emission";

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new ColorRGBMaterialSlot(kEmissionId, kEmissionName, kEmissionName, SlotType.Input, Color.black, ColorMode.HDR));
            RemoveSlotsNameNotMatching(new[] { kEmissionId });
        }

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass)
        {
            return null;
        }
    }
}
