namespace UnityEditor.ShaderGraph
{
    internal abstract class ShaderNodeType
    {
        internal abstract void Setup(ref NodeSetupContext context);

        internal abstract void OnNodeAdded(NodeChangeContext context, NodeRef node);

        internal virtual void OnNodeModified(NodeChangeContext context, NodeRef node)
        {
        }
    }
}
