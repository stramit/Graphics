using System;

namespace UnityEditor.ShaderGraph.NodeLibrary
{
    internal class ColorNode : ShaderNode, IGeneratesBodyCode, IPropertyFromNode
    {
        InputDescriptor m_Color = new InputDescriptor(0, "Color", ConcreteSlotValueType.Vector4, new ColorControl());
        InputDescriptor m_Mode = new InputDescriptor(1, "Mode", ConcreteSlotValueType.Vector1, new PopupControl(Enum.GetNames(typeof(ColorMode))));
        OutputDescriptor m_Out = new OutputDescriptor(2, "Out", ConcreteSlotValueType.Vector4);

        internal override void Setup(ref NodeDefinitionContext context)
        {
            context.CreateNodeType(new NodeTypeDescriptor
            {
                path = "INTERNAL",
                name = "Color",
                outPorts = new OutputDescriptor[] { m_Out },
                parameters = new InputDescriptor[] { m_Color, m_Mode },
                preview = false
            });
        }

        public void GenerateNodeCode(ShaderGenerator visitor, GraphContext graphContext, GenerationMode generationMode)
        {
            var colorShaderValue = GetShaderValue(m_Color);
            var outShaderValue = GetShaderValue(m_Out);

            visitor.AddShaderChunk(string.Format("{0} = {1};", 
                outShaderValue.ToVariableDefinition(precision),
                colorShaderValue.ToVariableReference(precision, generationMode)));
        }

        public IShaderProperty AsShaderProperty()
        {
            var colorParameter = GetShaderValue(m_Color);
            var modeParameter = GetShaderValue(m_Mode);
            return new ColorShaderProperty 
            { 
                value = colorParameter.value.vectorValue, 
                colorMode = (ColorMode)modeParameter.value.vectorValue.x 
            };
        }

        public int outputSlotId { get { return m_Out.id; } }
    }
}
