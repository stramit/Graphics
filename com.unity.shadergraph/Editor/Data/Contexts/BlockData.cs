using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    internal abstract class BlockData : AbstractMaterialNode
    {
        public override bool hasPreview
        {
            get { return false; }
        }

        public abstract Type contextType { get; } 
        public abstract Type[] requireBlocks { get; }

        public abstract ConditionalField[] GetConditionalFields(PassDescriptor pass);

        protected override void OnSlotsChanged()
        {
            owner?.contextManager.DirtyOutput();
            owner?.ClearErrorsForNode(this);
        }
    }
}
