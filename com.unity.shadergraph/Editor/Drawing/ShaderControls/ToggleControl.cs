using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    class ToggleControl : IShaderControl
    {
        public bool defaultValue { get; }

        private ShaderPort m_Port;

        public ToggleControl()
        {
            this.defaultValue = false;
        }

        public ToggleControl(bool defaultValue)
        {
            this.defaultValue = defaultValue;
        }

        public SlotValueType[] GetValidPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Boolean }; }
        }

        public void UpdateDefaultValue(ShaderPort port)
        {
            m_Port = port;
            port.booleanValue = defaultValue;
        }

        public VisualElement GetControl(ShaderPort port)
        {
            VisualElement control = new VisualElement() { name = "ToggleControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/Controls/BooleanSlotControlView"));

            var toggleField = new Toggle() { value = m_Port.booleanValue };
            toggleField.RegisterValueChangedCallback(OnValueChanged);
            control.Add(toggleField);
            return control;
        }

        void OnValueChanged(ChangeEvent<bool> evt)
        {
            if (!evt.newValue.Equals(m_Port.booleanValue))
            {
                m_Port.owner.owner.owner.RegisterCompleteObjectUndo("Boolean Change");
                m_Port.booleanValue = evt.newValue;
                m_Port.owner.Dirty(ModificationScope.Node);
            }
        }
    }
}
