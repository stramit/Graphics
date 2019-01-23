using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    class ColorControl : IShaderControl
    {
        public SerializableValueStore defaultValue { get; }

        public SlotValueType[] validPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Vector3, SlotValueType.Vector4 }; }
        }

        private ShaderPort m_Port;
        private bool m_Hdr;

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

        public VisualElement GetControl(ShaderPort port)
        {
            m_Port = port;

            VisualElement control = new VisualElement() { name = "ColorControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/Controls/ColorRGBASlotControlView"));

            var value = port.valueType == SlotValueType.Vector4 ? m_Port.portValue.vectorValue : new Vector4(m_Port.portValue.vectorValue.x, m_Port.portValue.vectorValue.y, m_Port.portValue.vectorValue.z, 0);
            var alpha = port.valueType == SlotValueType.Vector4;

            var colorField = new ColorField { value = value, showAlpha = alpha, hdr = m_Hdr, showEyeDropper = false };
            colorField.RegisterValueChangedCallback(OnValueChanged);
            control.Add(colorField);
            return control;
        }

        void OnValueChanged(ChangeEvent<Color> evt)
        {
            if (!evt.newValue.Equals(m_Port.portValue.vectorValue))
            {
                m_Port.owner.owner.owner.RegisterCompleteObjectUndo("Color Change");
                m_Port.portValue.vectorValue = evt.newValue;
                m_Port.owner.Dirty(ModificationScope.Node);
            }
        }
    }
}
