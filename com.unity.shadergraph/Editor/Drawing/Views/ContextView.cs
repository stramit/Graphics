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

        public ContextView(ContextData data)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/ContextView"));

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
            m_HeaderLabel.text = data.displayName;

            this.Synchronize(data.blocks, e => AddElement(new BlockView(e)), e => RemoveElement((GraphElement)e));
        }

        protected override bool AcceptsElement(GraphElement element, ref int proposedIndex, int maxIndex)
        {
            return element.userData is BlockData;
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
