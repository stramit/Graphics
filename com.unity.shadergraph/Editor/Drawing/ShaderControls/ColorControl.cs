using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace UnityEditor.ShaderGraph
{
    class ColorControl : IShaderControl
    {
        public SerializableValueStore defaultValue { get; }

        public SlotValueType[] validPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Vector3, SlotValueType.Vector4 }; }
        }

        bool m_Hdr;

        public ColorControl()
        {
        }

        public ColorControl(bool hdr = false)
        {
            m_Hdr = hdr;
        }

        public ColorControl(Color defaultValue, bool hdr = false)
        {
            this.defaultValue = new SerializableValueStore()
            {
                vectorValue = defaultValue
            };
            m_Hdr = hdr;
        }

        public VisualElement GetControl(IShaderValue shaderValue)
        {
            VisualElement control = new VisualElement() { name = "ColorControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/Controls/ColorRGBASlotControlView"));

            var alpha = true;//port.valueType == SlotValueType.Vector4; // TODO: Fix
            var colorField = new ColorField { value = shaderValue.value.vectorValue, showAlpha = alpha, hdr = m_Hdr, showEyeDropper = false };
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
