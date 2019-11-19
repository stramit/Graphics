using UnityEditor.Experimental.GraphView;

namespace UnityEditor.ShaderGraph.Drawing
{
    class BlockView : Node
    {
        readonly BlockData m_Data;

        public BlockData data => m_Data;

        public BlockView(BlockData data)
        {
            userData = m_Data = data;
            ChangeDispatcher.Connect(this, m_Data, OnChange);
        }

        void OnChange()
        {
            title = data.displayName;
        }
    }
}
