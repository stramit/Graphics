using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    class LabelControl : IShaderControl
    {
        public string[] labels { get; set; }
        public float[] values { get; set; }
        public SerializableValueStore defaultValue { get; }

        public ConcreteSlotValueType[] validPortTypes
        {
            get { return (ConcreteSlotValueType[])Enum.GetValues(typeof(ConcreteSlotValueType)); }
        }

        public LabelControl()
        {
            labels = new string[] { "Label" };
        }

        public LabelControl(string label, SerializableValueStore value = null)
        {
            if(value != null)
                defaultValue = value;

            labels = new string[] { label };
        }

        public VisualElement GetControl(IShaderValue shaderValue)
        {
            VisualElement control = new VisualElement() { name = "LabelControl" };
            Label label = new Label(labels[0]);
            control.Add(label);
            return control;
        }
    }
}
