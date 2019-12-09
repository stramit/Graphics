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
        }

        void AddElement(BlockData blockData)
        {
            // Need to add Blocks via GraphEditorView because it needs to hook into
            // Preview and Color Managers etc. This needs to be rewritten.
            var graphEditorView = GetFirstAncestorOfType<GraphEditorView>();
            var nodeView = graphEditorView.AddBlockNode(this, blockData);

            blocks = contentContainer.Query<Node>().Build();
            data.owner.contextManager.UpdateBlocks();
        }

        void RemoveElement(VisualElement element)
        {
            if(element is MaterialNodeView nodeView)
            {
                Remove(nodeView);
                blocks = contentContainer.Query<Node>().Build();
                data.owner.contextManager.UpdateBlocks();
            }
        }

        void AddPort(PortData portData)
        {
            // GraphEditorView is required for the EdgeConnectorListener
            // Should we create any listener somewhere easier to get to?
            var graphEditorView = GetFirstAncestorOfType<GraphEditorView>();
            var port = ShaderPort.Create(portData, graphEditorView.edgeConnectorListener);
            var container = portData.direction == PortData.Direction.Input ? inputContainer : outputContainer;
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
