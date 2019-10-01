namespace UnityEditor.ShaderGraph.Internal
{
    public class TargetSetupContext
    {
        public IMasterNode masterNode { get; private set; }
        public SubShaderDescriptor descriptor { get; private set; }

        public void SetMasterNode(IMasterNode masterNode)
        {
            this.masterNode = masterNode;
        }

        public void SetupTarget(SubShaderDescriptor descriptor)
        {
            this.descriptor = descriptor;
        }
    }

    public struct SubShaderDescriptor
    {
        public string pipelineTag;
        public string renderQueueOverride;
        public string renderTypeOverride;
        public ShaderPass[] passes;
    }
}
