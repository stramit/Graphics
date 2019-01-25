using System;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Title("INTERNAL", "Color")]
    internal class ColorNodeType : ShaderNode, IGeneratesBodyCode, IPropertyFromNode
    {
        InputDescriptor m_Color = new InputDescriptor(0, "Color", ConcreteSlotValueType.Vector4, new ColorControl());
        InputDescriptor m_Mode = new InputDescriptor(1, "Mode", ConcreteSlotValueType.Vector1, new PopupControl(Enum.GetNames(typeof(ColorMode))));
        OutputDescriptor m_Out = new OutputDescriptor(2, "Out", ConcreteSlotValueType.Vector4);

        public ColorNodeType()
        {
            DefineNode(new NodeTypeDescriptor()
            {
                name = "Color",
                outPorts = new OutputDescriptor[] { m_Out },
                parameters = new InputDescriptor[] { m_Color, m_Mode }
            });
        }

        public void GenerateNodeCode(ShaderGenerator visitor, GraphContext graphContext, GenerationMode generationMode)
        {
            var colorShaderValue = FindShaderValue(m_Color.id);
            var outShaderValue = FindShaderValue(m_Out.id);

            visitor.AddShaderChunk(string.Format(
                    @"{0}4 {1} = IsGammaSpace() ? {0}4({2}, {3}, {4}, {5}) : {0}4(SRGBToLinear({0}3({2}, {3}, {4})), {5});"
                    , precision
                    , outShaderValue.ToShaderVariableName()
                    , NodeUtils.FloatToShaderValue(colorShaderValue.value.vectorValue.x)
                    , NodeUtils.FloatToShaderValue(colorShaderValue.value.vectorValue.y)
                    , NodeUtils.FloatToShaderValue(colorShaderValue.value.vectorValue.z)
                    , NodeUtils.FloatToShaderValue(colorShaderValue.value.vectorValue.w)), true);

            // visitor.AddShaderChunk(string.Format("{0} = {1};",
            //     outShaderValue.ToShaderVariableDefinition(precision),
            //     colorShaderValue.ToShaderVariableReference(precision, generationMode)));
        }

        public IShaderProperty AsShaderProperty()
        {
            var colorParameter = FindShaderValue(m_Color.id);
            var modeParameter = FindShaderValue(m_Mode.id);
            return new ColorShaderProperty 
            { 
                value = colorParameter.value.vectorValue, 
                colorMode = (ColorMode)modeParameter.value.vectorValue.x 
            };
        }

        public int outputSlotId { get { return m_Out.id; } }
    }
}
