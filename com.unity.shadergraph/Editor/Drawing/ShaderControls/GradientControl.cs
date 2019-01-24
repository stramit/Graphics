using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace UnityEditor.ShaderGraph
{
    class GradientControl : IShaderControl
    {
        public SerializableValueStore defaultValue { get; }

        public SlotValueType[] validPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Gradient }; }
        }

        public GradientControl()
        {
        }

        public GradientControl(Gradient defaultValue)
        {
            this.defaultValue = new SerializableValueStore()
            {
                gradientValue = defaultValue
            };
        }

        public VisualElement GetControl(IShaderValue shaderValue)
        {
            VisualElement control = new VisualElement() { name = "GradientControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/Controls/GradientSlotControlView"));

            var gradientField = new GradientField() { value = shaderValue.value.gradientValue };
            gradientField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue.Equals(shaderValue.value.gradientValue))
                    return;
                shaderValue.UpdateValue(new SerializableValueStore()
                {
                    gradientValue = evt.newValue
                });
            });

            control.Add(gradientField);
            return control;
        }
    }
}