using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    class ColorControl : IShaderControl
    {
        public Color defaultValue { get; }
        public bool hdr { get; }

        private ShaderPort m_Port;

        public ColorControl(bool hdr = false)
        {
            this.defaultValue = Color.black;
            this.hdr = hdr;
        }

        public ColorControl(Color defaultValue, bool hdr = false)
        {
            this.defaultValue = defaultValue;
            this.hdr = hdr;
        }

        public SlotValueType[] GetValidPortTypes
        {
            get
            {
                return new SlotValueType[]
                {
                    SlotValueType.Vector3,
                    SlotValueType.Vector4
                };
            }
        }

        public void UpdateDefaultValue(ShaderPort port)
        {
            m_Port = port;
            port.vectorValue = defaultValue;
        }

        public VisualElement GetControl(ShaderPort port)
        {
            VisualElement control = new VisualElement() { name = "ColorControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/Controls/ColorRGBASlotControlView"));

            var value = port.valueType == SlotValueType.Vector4 ? m_Port.vectorValue : new Vector4(m_Port.vectorValue.x, m_Port.vectorValue.y, m_Port.vectorValue.z, 0);
            var alpha = port.valueType == SlotValueType.Vector4;

            var colorField = new ColorField { value = value, showAlpha = alpha, hdr = hdr, showEyeDropper = false };
            colorField.RegisterValueChangedCallback(OnValueChanged);
            control.Add(colorField);
            return control;
        }

        void OnValueChanged(ChangeEvent<Color> evt)
        {
            if (!evt.newValue.Equals(m_Port.vectorValue))
            {
                m_Port.owner.owner.owner.RegisterCompleteObjectUndo("Color Change");
                m_Port.vectorValue = evt.newValue;
                m_Port.owner.Dirty(ModificationScope.Node);
            }
        }
    }
}
