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

        private ShaderPort m_Node;

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
            m_Node = port;
            port.vectorValue = defaultValue;
        }

        public VisualElement GetControl(ShaderPort port)
        {
            VisualElement control = new VisualElement() { name = "ColorControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/Controls/ColorRGBASlotControlView"));

            var value = port.valueType == SlotValueType.Vector4 ? m_Node.vectorValue : new Vector4(m_Node.vectorValue.x, m_Node.vectorValue.y, m_Node.vectorValue.z, 0);
            var alpha = port.valueType == SlotValueType.Vector4;

            var colorField = new ColorField { value = value, showAlpha = alpha, hdr = hdr, showEyeDropper = false };
            colorField.RegisterValueChangedCallback(OnValueChanged);
            control.Add(colorField);
            return control;
        }

        void OnValueChanged(ChangeEvent<Color> evt)
        {
            m_Node.owner.owner.owner.RegisterCompleteObjectUndo("Color Change");
            m_Node.vectorValue = evt.newValue;
            m_Node.owner.Dirty(ModificationScope.Node);
        }
    }
}
