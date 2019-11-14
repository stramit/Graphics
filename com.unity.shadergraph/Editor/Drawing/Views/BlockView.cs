using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Graphing.Util;

namespace UnityEditor.ShaderGraph.Drawing
{
    class BlockView : Node
    {
        readonly BlockData m_Data;

        public BlockView(BlockData data)
        {
            // Hook up data
            userData = m_Data = data;
            ChangeDispatcher.Connect(this, m_Data, OnChange);

            ApplyStyles();
        }

        public BlockData data => m_Data;

        void OnChange()
        {
            title = data.displayName;
            inputContainer.Synchronize(data.inputPorts, e => this.AddPort(e), e => this.RemovePort((Port)e));
            outputContainer.Synchronize(data.outputPorts, e => this.AddPort(e), e => this.RemovePort((Port)e));
        }

        void ApplyStyles()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/BlockView"));
            if (data is FieldBlockData fieldBlockData)
            {
                AddToClassList("fieldBlock");
            }
        }
    }
}
