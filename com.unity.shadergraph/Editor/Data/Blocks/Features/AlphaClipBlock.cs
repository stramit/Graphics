using System;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    [Title("Features", "Alpha Clip")]
    class AlphaClipBlock : BlockData
    {
        public AlphaClipBlock()
        {
            name = "Alpha Clip";
        }

        public override Type contextType => typeof(OutputContext);
        public override Type[] requireBlocks => new Type[] { typeof(AlphaBlock), typeof(ClipThresholdBlock) };

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass)
        {
            return new ConditionalField[]
            {
                new ConditionalField(Fields.AlphaClip, true),
                new ConditionalField(Fields.AlphaTest, true),
            };
        }
    }
}
