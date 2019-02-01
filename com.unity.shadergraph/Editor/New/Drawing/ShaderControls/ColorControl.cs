using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace UnityEditor.ShaderGraph
{
    class ColorControl : IShaderControl
    {
        public ShaderControlData controlData { get; set; }
        public ShaderValueData defaultValueData { get; set; }

        public SlotValueType[] validPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Vector3, SlotValueType.Vector4 }; }
        }

        public int portControlWidth
        {
            get { return 32; }
        }

        public ColorControl()
        {
            this.controlData = new ShaderControlData()
            {
                values = new float[] { 0 }
            };
        }

        public ColorControl(Color defaultValue, bool hdr = false)
        {
            this.defaultValueData = new ShaderValueData()
            {
                vector = defaultValue
            };
            this.controlData = new ShaderControlData()
            {
                values = new float[] { hdr ? 1 : 0 }
            };
        }

        public VisualElement GetControl(IShaderInput shaderInput)
        {
            VisualElement control = new VisualElement() { name = "ColorControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/ShaderControls/ColorControl"));

            var alpha = shaderInput.concreteValueType == ConcreteSlotValueType.Vector4;
            var colorField = new ColorField { value = shaderInput.value.vector, showAlpha = alpha, hdr = controlData.values[0] == 1, showEyeDropper = false };
            colorField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue.Equals(shaderInput.value.vector))
                    return;
                shaderInput.UpdateValueData(new ShaderValueData()
                {
                    vector = evt.newValue
                });
            });

            control.Add(colorField);
            return control;
        }
    }
}
