using System;
using System.Collections.Generic;
using UnityEngine;
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

        public abstract Type contextType { get; } 
        public abstract Type[] requireBlocks { get; }

        public abstract ConditionalField[] GetConditionalFields(PassDescriptor pass, List<BlockData> validBlocks);

        // This used to be on IMasterNode
        // Is required for render pipelines to set preview property values
        public virtual void ProcessPreviewMaterial(Material material)
        {
        }
    }
}
