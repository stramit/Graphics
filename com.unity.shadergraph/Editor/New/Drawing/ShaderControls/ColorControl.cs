using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace UnityEditor.ShaderGraph
{
    class ColorControl : IShaderControl
    {
        public string[] labels { get; set; }
        public float[] values { get; set; }
        public SerializableValueStore defaultValue { get; }

        public ConcreteSlotValueType[] validPortTypes
        {
            get { return new ConcreteSlotValueType[] { ConcreteSlotValueType.Vector3, ConcreteSlotValueType.Vector4 }; }
        }

        public ColorControl()
        {
            values = new float[] { 0 };
        }

        public ColorControl(Color defaultValue, bool hdr = false)
        {
            this.defaultValue = new SerializableValueStore()
            {
                vectorValue = defaultValue
            };
            values = new float[] { hdr ? 1 : 0 };
        }

        public VisualElement GetControl(IShaderValue shaderValue)
        {
            VisualElement control = new VisualElement() { name = "ColorControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/Controls/ColorRGBASlotControlView"));

            var alpha = shaderValue.concreteValueType == ConcreteSlotValueType.Vector4;
            var colorField = new ColorField { value = shaderValue.value.vectorValue, showAlpha = alpha, hdr = values[0] == 1, showEyeDropper = false };
            colorField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue.Equals(shaderValue.value.vectorValue))
                    return;
                shaderValue.UpdateValue(new SerializableValueStore()
                {
                    vectorValue = evt.newValue
                });
            });

            control.Add(colorField);
            return control;
        }
    }
}
