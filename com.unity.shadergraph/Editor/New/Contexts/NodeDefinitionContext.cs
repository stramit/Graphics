namespace UnityEditor.ShaderGraph
{
    public struct NodeDefinitionContext
    {
        internal NodeTypeDescriptor type { get; set; }

        internal void CreateNodeType(NodeTypeDescriptor typeDescriptor)
        {
            type = typeDescriptor;
        }
    }
}
