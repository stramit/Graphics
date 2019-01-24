using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace UnityEditor.ShaderGraph
{
    class GradientControl : IShaderControl
    {
        public ShaderControlData controlData { get; set; }
        public ShaderValueData defaultValueData { get; }

        public ConcreteSlotValueType[] validPortTypes
        {
            get { return new ConcreteSlotValueType[] { ConcreteSlotValueType.Gradient }; }
        }

        public GradientControl()
        {
        }

        public GradientControl(Gradient defaultValue)
        {
            this.defaultValueData = new ShaderValueData()
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
                shaderValue.UpdateValue(new ShaderValueData()
                {
                    gradientValue = evt.newValue
                });
            });

            control.Add(gradientField);
            return control;
        }
    }
}