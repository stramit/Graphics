using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEditor.ShaderGraph.Drawing.Controls;

namespace UnityEditor.ShaderGraph
{
    [Title("Hidden", "Target")]
    class TargetBlock : BlockData
    {
        public TargetBlock()
        {
            name = "Target";
            UpdateNodeAfterDeserialization();
        }

        bool m_Target = false;
        
        [TargetControl]
        public bool target
        {
            get => m_Target;
            set => m_Target = value;
        }

        public override Type contextType => typeof(OutputContext);
        public override Type[] requireBlocks => null;

        public override bool canDeleteNode => false;

        public override ConditionalField[] GetConditionalFields(PassDescriptor pass)
        {
            return null;
        }
    }
}
