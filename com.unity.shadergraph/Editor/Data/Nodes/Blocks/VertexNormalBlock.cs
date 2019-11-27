using System.Linq;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    [Title("Attributes", "Normal (Object Space)")]
    class VertexNormalBlock : BlockData, IMayRequireNormal
    {
        public VertexNormalBlock()
        {
            name = "Normal (Object Space)";
            UpdateNodeAfterDeserialization();
        }

        const int kNormalId = 0;
        const string kNormalName = "Normal";

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new NormalMaterialSlot(kNormalId, kNormalName, kNormalName, CoordinateSpace.Object, ShaderStageCapability.Vertex));
            RemoveSlotsNameNotMatching(new[] { kNormalId });
        }

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass)
        {
            return null;
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
