namespace UnityEditor.ShaderGraph.NodeLibrary
{
    sealed class AddNode : ShaderNode
    {
        InputDescriptor m_A = new InputDescriptor(0, "A", SlotValueType.DynamicVector);
        InputDescriptor m_B = new InputDescriptor(1, "B", SlotValueType.DynamicVector);
        OutputDescriptor m_Out = new OutputDescriptor(2, "Out", SlotValueType.DynamicVector);

        internal override void Setup(ref NodeDefinitionContext context)
        {
            context.CreateNodeType(new NodeTypeDescriptor
            {
                path = "INTERNAL",
                name = "Add",
                inPorts = new InputDescriptor[] { m_A, m_B },
                outPorts = new OutputDescriptor[] { m_Out },
                preview = true
            });
        }
        
        internal override void OnGenerateFunction(ref FunctionDefinitionContext context)
        {
            IShaderValue shaderValue = GetShaderValue(m_Out);
            context.SetHlslFunction(new HlslFunctionDescriptor
            {
                name = string.Format("Unity_Add{0}", shaderValue.concreteValueType.GetChannelCount()),
                source = HlslSource.String("Out = A + B;"),
                inArguments = new InputDescriptor[] { m_A, m_B },
                outArguments = new OutputDescriptor[] { m_Out }
            });
        }
    }
}
