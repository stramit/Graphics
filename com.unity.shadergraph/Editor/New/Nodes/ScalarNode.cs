namespace UnityEditor.ShaderGraph.NodeLibrary
{
    [Title("INTERNAL", "Scalar")]
    sealed class ScalarNode : ShaderNode, IGeneratesBodyCode, IPropertyFromNode
    {
        InputDescriptor m_X = new InputDescriptor(0, "X", ConcreteSlotValueType.Vector1);
        OutputDescriptor m_Out = new OutputDescriptor(1, "Out", ConcreteSlotValueType.Vector1);

        public ScalarNode()
        {
            DefineNode(new NodeTypeDescriptor()
            {
                path = "INTERNAL",
                name = "Scalar",
                inPorts = new InputDescriptor[] { m_X },
                outPorts = new OutputDescriptor[] { m_Out },
                preview = true
            });
        }

        public void GenerateNodeCode(ShaderGenerator visitor, GraphContext graphContext, GenerationMode generationMode)
        {
            var xShaderValue = GetShaderValue(m_X);
            var outShaderValue = GetShaderValue(m_Out);

            visitor.AddShaderChunk(string.Format("{0} = {1};", 
                outShaderValue.ToVariableDefinition(precision),
                xShaderValue.ToVariableReference(precision, generationMode)));
        }

        public IShaderProperty AsShaderProperty()
        {
            var xShaderValue = GetShaderValue(m_X);
            return new Vector1ShaderProperty 
            { 
                value = xShaderValue.value.vectorValue.x
            };
        }

        public int outputSlotId { get { return m_Out.id; } }
    }
}
