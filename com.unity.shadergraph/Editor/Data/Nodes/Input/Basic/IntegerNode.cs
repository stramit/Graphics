using System.Collections.Generic;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Title("Input", "Basic", "Integer")]
    class IntegerNode : AbstractMaterialNode, IGeneratesBodyCode, IPropertyFromNode
    {
        [SerializeField]
        private int m_Value;

        public const int OutputSlotId = 0;
        private const string kOutputSlotName = "Out";

        public IntegerNode()
        {
            name = "Integer";
            UpdateNodeAfterDeserialization();
        }

        public override string documentationURL
        {
            get { return "https://github.com/Unity-Technologies/ShaderGraph/wiki/Integer-Node"; }
        }

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new Vector1MaterialSlot(OutputSlotId, kOutputSlotName, kOutputSlotName, SlotType.Output, 0));
            RemoveSlotsNameNotMatching(new[] { OutputSlotId });
        }

        [IntegerControl("")]
        public int value
        {
            get { return m_Value; }
            set
            {
                if (m_Value == value)
                    return;

                m_Value = value;

                Dirty(ModificationScope.Node);
            }
        }

        public override void CollectGraphInputs(PropertyCollector properties, GenerationMode generationMode)
        {
            if (!generationMode.IsPreview())
                return;

            properties.AddGraphInput(new ShaderProperty(PropertyType.Vector1)
            {
                overrideReferenceName = GetVariableNameForNode(),
                //generatePropertyBlock = false,
                //value = value,
                //floatType = FloatType.Integer
            });
        }

        public void GenerateNodeCode(ShaderGenerator visitor, GraphContext graphContext, GenerationMode generationMode)
        {
            if (generationMode.IsPreview())
                return;

            visitor.AddShaderChunk(precision + " " + GetVariableNameForNode() + " = " + m_Value + ";", true);
        }

        public override string GetVariableNameForSlot(int slotId)
        {
            return GetVariableNameForNode();
        }

        public override void CollectPreviewMaterialProperties(List<PreviewProperty> properties)
        {
            properties.Add(new PreviewProperty(PropertyType.Vector1)
            {
                name = GetVariableNameForNode(),
                floatValue = m_Value
            });
        }

        public ShaderProperty AsShaderProperty()
        {
            return new ShaderProperty(PropertyType.Vector1) { /*value = value, floatType = FloatType.Integer*/ };
        }

        public int outputSlotId { get { return OutputSlotId; } }
    }
}
