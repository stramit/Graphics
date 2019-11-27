using System.Collections.Generic;

namespace UnityEditor.ShaderGraph.Internal
{
    public class TargetSetupContext
    {
        internal BlockData[] blockDatas { get; private set; }
        public SubShaderDescriptor descriptor { get; private set; }
        public List<string> assetDependencyPaths { get; private set; }

        public TargetSetupContext()
        {
            assetDependencyPaths = new List<string>();
        }

        internal void SetBlockDatas(BlockData[] blockDatas)
        {
            this.blockDatas = blockDatas;
        }

        public void SetupSubShader(SubShaderDescriptor descriptor)
        {
            this.descriptor = descriptor;
        }

        public void AddAssetDependencyPath(string path)
        {
            assetDependencyPaths.Add(path);
        }
    }
}
