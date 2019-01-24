namespace UnityEditor.ShaderGraph
{
    class NodeChangeContext
    {
        private HlslFunctionDescriptor m_Descriptor;
        public HlslFunctionDescriptor descriptor
        {
            get { return m_Descriptor; }
        }

        public void SetHlslFunction(HlslFunctionDescriptor descriptor)
        {
            m_Descriptor = descriptor;
        }
    }
}
