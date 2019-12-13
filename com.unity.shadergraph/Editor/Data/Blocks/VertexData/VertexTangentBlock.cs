using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    [Title("VertexData", "Tangent (Object)")]
    class VertexTangentBlock : BlockData, IMayRequireTangent
    {
        public VertexTangentBlock()
        {
            name = "Tangent (Object)";
            UpdateNodeAfterDeserialization();
        }

        public override Type contextType => typeof(VertexContext);
        public override Type[] requireBlocks => null;

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new TangentMaterialSlot(0, "Tangent (Object)", "Tangent", CoordinateSpace.Object, ShaderStageCapability.Vertex));
            RemoveSlotsNameNotMatching(new[] { 0 });
        }

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass, List<BlockData> validBlocks)
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
