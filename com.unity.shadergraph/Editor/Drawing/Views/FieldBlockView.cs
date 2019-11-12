using UnityEditor.Experimental.GraphView;

namespace UnityEditor.ShaderGraph.Drawing
{
    class FieldBlockView : Node
    {
        readonly FieldBlockData m_Data;

        public FieldBlockData data => m_Data;

        public FieldBlockView(FieldBlockData data)
        {
            userData = m_Data = data;
            ChangeDispatcher.Connect(this, m_Data, OnChange);
        }

        void OnChange()
        {
            title = $"{m_Data.field.tag}.{m_Data.field.name}";
        }
    }
}
