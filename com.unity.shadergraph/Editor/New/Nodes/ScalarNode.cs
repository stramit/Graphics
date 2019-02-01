namespace UnityEditor.ShaderGraph.NodeLibrary
{
    sealed class ScalarNode : ShaderNode, IGeneratesBodyCode, IPropertyFromNode
    {
        InputDescriptor m_X = new InputDescriptor(0, "X", SlotValueType.Vector1);
        OutputDescriptor m_Out = new OutputDescriptor(1, "Out", SlotValueType.Vector1);

        internal override void Setup(ref NodeDefinitionContext context)
        {
            context.CreateNodeType(new NodeTypeDescriptor
            {
                path = "INTERNAL",
                name = "Scalar",
                inPorts = new InputDescriptor[] { m_X },
                outPorts = new OutputDescriptor[] { m_Out },
                preview = false
            });
        }

        public void GenerateNodeCode(ShaderGenerator visitor, GraphContext graphContext, GenerationMode generationMode)
        {
            var xShaderValue = GetShaderValue(m_X) as IShaderInput;
            var outShaderValue = GetShaderValue(m_Out);

            visitor.AddShaderChunk(string.Format("{0} = {1};", 
                outShaderValue.ToVariableDefinitionSnippet(precision),
                xShaderValue.ToValueReferenceSnippet(precision, generationMode)));
        }

        public IShaderProperty AsShaderProperty()
        {
            var xShaderValue = GetShaderValue(m_X) as IShaderInput;
            return new Vector1ShaderProperty 
            { 
                value = xShaderValue.valueData.vector.x
            };
        }

        public int outputSlotId { get { return m_Out.id; } }
    }
}
