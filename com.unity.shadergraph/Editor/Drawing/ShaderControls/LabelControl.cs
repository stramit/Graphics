using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    class LabelControl : IShaderControl
    {
        public SerializableValueStore defaultValue { get; }

        public ConcreteSlotValueType[] validPortTypes
        {
            get { return (ConcreteSlotValueType[])Enum.GetValues(typeof(ConcreteSlotValueType)); }
        }

        private string m_Label = "";

        public LabelControl()
        {
        }

        public LabelControl(string label)
        {
            m_Label = label;
        }

        public LabelControl(string label, SerializableValueStore value)
        {
            defaultValue = value;
            m_Label = label;
        }

        public VisualElement GetControl(IShaderValue shaderValue)
        {
            VisualElement control = new VisualElement() { name = "LabelControl" };
            Label label = new Label(m_Label);
            control.Add(label);
            return control;
        }
    }
}
