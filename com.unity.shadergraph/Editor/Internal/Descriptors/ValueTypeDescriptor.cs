namespace UnityEditor.ShaderGraph.Internal
{
    public abstract class HlslTypeDescriptor
    {
        int m_VectorCount;

        public int vectorCount
        {
            get => m_VectorCount;
            set => m_VectorCount = value;
        }
    }

    public sealed class HlslPrimitiveDescriptor : HlslTypeDescriptor
    {
        readonly string m_HlslName;

        public HlslPrimitiveDescriptor(string hlslName, int vectorCount)
        {
            m_HlslName = hlslName;
            this.vectorCount = vectorCount;
        }

        public string hlslName => m_HlslName;
    }

    public sealed class HlslStructDescriptor : HlslTypeDescriptor
    {
        readonly string m_HlslName;
        readonly FieldDescriptor[] m_FieldDescriptors;

        public HlslStructDescriptor(string hlslName, FieldDescriptor[] fields)
        {
            m_HlslName = hlslName;
            m_FieldDescriptors = fields;
            this.vectorCount = 0;
        }

        public string hlslName => m_HlslName;
        public FieldDescriptor[] fieldDescriptors => m_FieldDescriptors;
    }
}
