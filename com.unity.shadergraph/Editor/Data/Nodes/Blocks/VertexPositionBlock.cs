using System.Linq;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    [Title("Attributes", "Position (Object Space)")]
    class VertexPositionBlock : BlockData, IMayRequirePosition
    {
        public VertexPositionBlock()
        {
            name = "Position (Object Space)";
            UpdateNodeAfterDeserialization();
        }

        const int kPositionId = 0;
        const string kPositionName = "Position";

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new PositionMaterialSlot(kPositionId, kPositionName, kPositionName, CoordinateSpace.Object, ShaderStageCapability.Vertex));
            RemoveSlotsNameNotMatching(new[] { kPositionId });
        }

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass)
        {
            return null;
        }

        public NeededCoordinateSpace RequiresPosition(ShaderStageCapability stageCapability)
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
            return validSlots.OfType<IMayRequirePosition>().Aggregate(NeededCoordinateSpace.None, (mask, node) => mask | node.RequiresPosition(stageCapability));
        }
    }
}
