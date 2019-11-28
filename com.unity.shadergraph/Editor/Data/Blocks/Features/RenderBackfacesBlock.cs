using System;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    [Title("Features", "Render Backfaces")]
    class RenderBackfacesBlock : BlockData
    {
        public RenderBackfacesBlock()
        {
            name = "Render Backfaces";
        }

        public override Type contextType => typeof(OutputContext);
        public override Type[] requireBlocks => null;

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass)
        {
            return new ConditionalField[]
            {
                new ConditionalField(Fields.DoubleSided, true),
            };
        }
    }
}
