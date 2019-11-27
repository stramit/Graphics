using System.Linq;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    [Title("Attributes", "Tangent (Object Space)")]
    class VertexTangentBlock : BlockData, IMayRequireTangent
    {
        public VertexTangentBlock()
        {
            name = "Tangent (Object Space)";
            UpdateNodeAfterDeserialization();
        }

        const int kTangentId = 0;
        const string kTangentName = "Tangent";

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new TangentMaterialSlot(kTangentId, kTangentName, kTangentName, CoordinateSpace.Object, ShaderStageCapability.Vertex));
            RemoveSlotsNameNotMatching(new[] { kTangentId });
        }

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass)
        {
            return null;
        }

        public NeededCoordinateSpace RequiresTangent(ShaderStageCapability stageCapability)
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
            return validSlots.OfType<IMayRequireTangent>().Aggregate(NeededCoordinateSpace.None, (mask, node) => mask | node.RequiresTangent(stageCapability));
        }
    }
}
