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

        public ContextView(ContextData data)
        {
            // Create UI scaffold
            m_HeaderLabel = new Label();
            headerContainer.Add(m_HeaderLabel);

            // Hook up data
            userData = m_Data = data;
            ChangeDispatcher.Connect(this, data, OnChange);

            ApplyStyles();
        }

        public ContextData data => m_Data;

        void OnChange()
        {
            m_HeaderLabel.text = data.displayName;
            this.Synchronize(data.blocks, e => AddElement(new BlockView(e)), e => RemoveElement((GraphElement)e));
            inputContainer.Synchronize(data.inputPorts, e => this.AddPort(e), e => this.RemovePort((Port)e));
            outputContainer.Synchronize(data.outputPorts, e => this.AddPort(e), e => this.RemovePort((Port)e));

            SetPosition(new Rect(m_Data.position, Vector2.zero));
        }

        void ApplyStyles()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/ContextView"));
        }

        protected override bool AcceptsElement(GraphElement element, ref int proposedIndex, int maxIndex)
        {
            return element.userData is BlockData;
        }
    }
}
