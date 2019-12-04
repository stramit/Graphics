using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Graphing.Util;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.ShaderGraph.Drawing
{
    class ContextView : StackNode
    {
        readonly ContextData m_Data;
        readonly Label m_HeaderLabel;

        public ContextData data => m_Data;
        public UQueryState<Node> blocks { get; private set; }

        public ContextView(ContextData data)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/ContextView"));

            // Create UI scaffold
            m_HeaderLabel = new Label() { name = "headerLabel" };
            headerContainer.Add(m_HeaderLabel);

            // Hook up data
            userData = m_Data = data;
            ChangeDispatcher.Connect(this, data, OnChange);
        }

        public void OnChange()
        {
            SetPosition(new Rect(m_Data.position, Vector2.zero));
            m_HeaderLabel.text = data.displayName;

            this.Synchronize(data.blocks, AddElement, RemoveElement);
            inputContainer.Synchronize(data.inputPorts, e => AddPort(e), e => RemovePort((Port)e));
            outputContainer.Synchronize(data.outputPorts, e => AddPort(e), e => RemovePort((Port)e));

            // Rebuild Blocks query
            blocks = contentContainer.Query<Node>().Build();

            // TODO: This is bad
            // Use "owner" concept to access up to GraphData?
            var graphEditorView = GetFirstAncestorOfType<GraphEditorView>();
            graphEditorView.graphView.graph.UpdateSupportedBlocks();
            graphEditorView.graphView.graph.targetBlock.Dirty(Graphing.ModificationScope.Graph);
        }

        void AddElement(BlockData blockData)
        {
            // Need to add Blocks via GraphEditorView because it needs to hook into
            // Preview and Color Managers etc. This needs to be rewritten.
            var graphEditorView = GetFirstAncestorOfType<GraphEditorView>();
            var nodeView = graphEditorView.AddBlockNode(this, blockData);
        }

        void RemoveElement(VisualElement element)
        {
            if(element is MaterialNodeView nodeView)
            {
                Remove(nodeView);
            }
        }

        void AddPort(PortData portData)
        {
            var orientation = portData.orientation == PortData.Orientation.Horizontal ? Orientation.Horizontal : Orientation.Vertical;
            var direction = portData.direction == PortData.Direction.Input ? Direction.Input : Direction.Output;
            var capacity = direction == Direction.Input ? Port.Capacity.Single : Port.Capacity.Multi;
            var container = direction == Direction.Input ? inputContainer : outputContainer;

            var port = Port.Create<UnityEditor.Experimental.GraphView.Edge>(orientation, direction, capacity, portData.valueType.type);
            port.userData = portData;
            port.portName = portData.displayName;
            container.Add(port);
        }

        void RemovePort(Port port)
        {
            var container = port.direction == Direction.Input ? inputContainer : outputContainer;
            container.Remove(port);
        }

        protected override bool AcceptsElement(GraphElement element, ref int proposedIndex, int maxIndex)
        {
            return element.userData is BlockData blockData && blockData.contextType == data.contextType.type;
        }

        public void InsertElements(int insertIndex, IEnumerable<GraphElement> elements)
        {
            var blockDatas = elements.Select(x => x.userData as BlockData).ToArray();
            for(int i = 0; i < blockDatas.Length; i++)
            {
                data.blocks.Remove(blockDatas[i]);
            }
            
            data.blocks.InsertRange(insertIndex, blockDatas);
        }
    }
}
