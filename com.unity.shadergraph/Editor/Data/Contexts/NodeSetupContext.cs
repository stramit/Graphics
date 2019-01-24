namespace UnityEditor.ShaderGraph
{
    class NodeSetupContext
    {
        private NodeTypeDescriptor m_Descriptor;
        public NodeTypeDescriptor descriptor
        {
            get { return m_Descriptor; }
        }

        public void CreateType(NodeTypeDescriptor descriptor)
        {
            m_Descriptor = descriptor;
        }
    }
}