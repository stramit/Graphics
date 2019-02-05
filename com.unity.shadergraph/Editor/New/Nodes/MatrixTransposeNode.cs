using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph.NodeLibrary
{
    sealed class TransposeNode : ShaderNode
    {
        InputDescriptor m_In = new InputDescriptor(0, "In", SlotValueType.DynamicMatrix);
        OutputDescriptor m_Out = new OutputDescriptor(1, "Out", SlotValueType.DynamicMatrix);

        internal override void Setup(ref NodeDefinitionContext context)
        {
            context.CreateNodeType(new NodeTypeDescriptor
            {
                path = "INTERNAL",
                name = "Matrix Transpose",
                inPorts = new InputDescriptor[] { m_In },
                outPorts = new OutputDescriptor[] { m_Out },
                preview = false
            });
        }
        
        internal override void OnGenerateFunction(ref FunctionDefinitionContext context)
        {
            IShaderValue shaderValue = GetShaderValue(m_Out);
            context.SetHlslFunction(new HlslFunctionDescriptor
            {
                name = string.Format("Unity_Tranpose{0}", NodeUtils.GetSlotDimension(shaderValue.concreteValueType)),
                source = HlslSource.String("Out = transpose(In);"),
                inArguments = new InputDescriptor[] { m_In },
                outArguments = new OutputDescriptor[] { m_Out }
            });
        }
    }
}
