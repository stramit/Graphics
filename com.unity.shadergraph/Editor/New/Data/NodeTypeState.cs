using System.Collections.Generic;
using System.Linq;

namespace UnityEditor.ShaderGraph
{
    abstract class NodeTypeState
    {
        public int id;
        public AbstractMaterialGraph owner;
        public NodeTypeDescriptor type;
        public List<InputDescriptor> inputPorts = new List<InputDescriptor>();
        public List<OutputDescriptor> outputPorts = new List<OutputDescriptor>();
        public List<InputDescriptor> parameters = new List<InputDescriptor>();
        public List<HlslSource> hlslSources = new List<HlslSource>();

        #region Change lists for consumption by IShaderNode implementation

        // TODO: Need to also store node ID versions somewhere
        public IndexSet addedNodes = new IndexSet();
        public IndexSet modifiedNodes = new IndexSet();

        #endregion

        public bool isDirty => addedNodes.Any() || modifiedNodes.Any();

        public void ClearChanges()
        {
            addedNodes.Clear();
            modifiedNodes.Clear();
        }

        public abstract ShaderNodeType baseNodeType { get; set; }

        public abstract void DispatchChanges(NodeChangeContext context);
    }

    // This construction allows us to move the virtual call to outside the loop. The calls to the ShaderNodeType in
    // DispatchChanges are to a generic type parameter, and thus will be devirtualized if T is a sealed class.
    sealed class NodeTypeState<T> : NodeTypeState where T : ShaderNodeType
    {
        public T nodeType { get; set; }

        public override ShaderNodeType baseNodeType
        {
            get => nodeType;
            set => nodeType = (T)value;
        }

        public override void DispatchChanges(NodeChangeContext context)
        {
            foreach (var node in addedNodes)
            {
                // would be better to do this somewhere else, but easiest to hack it in here for now -- ctchou
                ShaderNodeInstance nodeInstance = (ShaderNodeInstance) owner.m_Nodes[node];
                NodeRef nodeRef = new NodeRef(owner, owner.currentContextId, nodeInstance);
                //proxyNode.InstantiateControls(nodeRef, context.m_CreatedControls);
                nodeType.OnNodeAdded(context, nodeRef);
            }

            foreach (var node in modifiedNodes)
            {
                nodeType.OnNodeModified(context, new NodeRef(owner, owner.currentContextId, (ShaderNodeInstance)owner.m_Nodes[node]));
            }
        }
    }
}