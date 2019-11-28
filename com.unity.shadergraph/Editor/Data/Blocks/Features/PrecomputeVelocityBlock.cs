using System;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    [Title("Features", "Precompute Velocity")]
    class PrecomputeVelocityBlock : BlockData
    {
        public PrecomputeVelocityBlock()
        {
            name = "Precompute Velocity";
        }

        public override Type contextType => typeof(OutputContext);
        public override Type[] requireBlocks => null;

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass)
        {
            return new ConditionalField[]
            {
                new ConditionalField(Fields.VelocityPrecomputed, true),
            };
        }
    }
}
