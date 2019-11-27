using System;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    internal abstract class BlockData : AbstractMaterialNode
    {
        public override bool hasPreview
        {
            get { return false; }
        }

        public abstract ConditionalField[] GetConditionalFields(PassDescriptor pass);
    }
}
