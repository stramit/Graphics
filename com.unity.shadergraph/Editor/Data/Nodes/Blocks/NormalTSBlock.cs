using System.Linq;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    [Title("Normal Map", "Normal (Tangent Space)")]
    class NormalTSBlock : BlockData, IMayRequireNormal
    {
        public NormalTSBlock()
        {
            name = "Normal (Tangent Space)";
            UpdateNodeAfterDeserialization();
        }

        const int kNormalId = 0;
        const string kNormalName = "Normal";

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new NormalMaterialSlot(kNormalId, kNormalName, kNormalName, CoordinateSpace.Tangent));
            RemoveSlotsNameNotMatching(new[] { kNormalId });
        }

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass)
        {
            return new ConditionalField[]
            {
                new ConditionalField(Fields.Normal, true),
            };
        }

        public NeededCoordinateSpace RequiresNormal(ShaderStageCapability stageCapability)
        {
            List<MaterialSlot> slots = new List<MaterialSlot>();
            GetSlots(slots);

            List<MaterialSlot> validSlots = new List<MaterialSlot>();
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].stageCapability != ShaderStageCapability.All && slots[i].stageCapability != stageCapability)
                    continue;

                validSlots.Add(slots[i]);
            }
            return validSlots.OfType<IMayRequireNormal>().Aggregate(NeededCoordinateSpace.None, (mask, node) => mask | node.RequiresNormal(stageCapability));
        }
    }
}
