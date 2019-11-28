using System;
using System.Linq;
using UnityEditor.ShaderGraph.Internal;
using System.Collections.Generic;

namespace UnityEditor.ShaderGraph
{
    [Title("SurfaceData", "Normal (Tangent)")]
    class NormalTSBlock : BlockData, IMayRequireNormal
    {
        public NormalTSBlock()
        {
            name = "Normal (Tangent)";
            UpdateNodeAfterDeserialization();
        }

        public override Type contextType => typeof(FragmentContext);
        public override Type[] requireBlocks => null;

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new NormalMaterialSlot(0, "Normal (Tangent)", "Normal", CoordinateSpace.Tangent));
            RemoveSlotsNameNotMatching(new[] { 0 });
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
