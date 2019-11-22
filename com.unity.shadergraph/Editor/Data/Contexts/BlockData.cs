using System;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    abstract class BlockData : AbstractMaterialNode
    {
        public override bool hasPreview
        {
            get { return false; }
        }
    }
}
