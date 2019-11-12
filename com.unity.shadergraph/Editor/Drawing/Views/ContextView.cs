using System;
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

        public ContextView(ContextData data)
        {
            // Create UI scaffold
            m_HeaderLabel = new Label();
            headerContainer.Add(m_HeaderLabel);

            // Hook up data
            userData = m_Data = data;
            ChangeDispatcher.Connect(this, data, OnChange);
        }

        void OnChange()
        {
            SetPosition(new Rect(m_Data.position, Vector2.zero));

            switch (m_Data.stage)
            {
                case ShaderStage.Fragment:
                    m_HeaderLabel.text = "Fragment";
                    break;
                case ShaderStage.Vertex:
                    m_HeaderLabel.text = "Vertex";
                    break;
                default:
                    m_HeaderLabel.text = "Unknown stage";
                    break;
            }

            this.Synchronize(data.blocks, AddBlock, e => RemoveElement((GraphElement)e));
        }

        void AddBlock(BlockData blockData)
        {
            switch (blockData)
            {
                case FieldBlockData fieldBlockData:
                    AddElement(new FieldBlockView(fieldBlockData));
                    break;
                default:
                    throw new InvalidOperationException("Unknown BlockData type");
            }
        }

        protected override bool AcceptsElement(GraphElement element, ref int proposedIndex, int maxIndex)
        {
            return element.userData is BlockData;
        }
    }
}
