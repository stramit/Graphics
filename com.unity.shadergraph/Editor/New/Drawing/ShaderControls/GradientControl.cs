using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace UnityEditor.ShaderGraph
{
    class GradientControl : IShaderControl
    {
        public ShaderControlData controlData { get; set; }
        public ShaderValueData defaultValueData { get; set; }

        public SlotValueType[] validPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Gradient }; }
        }

        public int portControlWidth
        {
            get { return 32; }
        }

        public GradientControl()
        {
        }

        public GradientControl(Gradient defaultValue)
        {
            this.defaultValueData = new ShaderValueData()
            {
                gradient = defaultValue
            };
        }

        public VisualElement GetControl(IShaderInput shaderInput)
        {
            VisualElement control = new VisualElement() { name = "GradientControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/ShaderControls/GradientControl"));

            var gradientField = new GradientField() { value = shaderInput.value.gradient };
            gradientField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue.Equals(shaderInput.value.gradient))
                    return;
                shaderInput.UpdateValueData(new ShaderValueData()
                {
                    gradient = evt.newValue
                });
            });

            control.Add(gradientField);
            return control;
        }
    }
}