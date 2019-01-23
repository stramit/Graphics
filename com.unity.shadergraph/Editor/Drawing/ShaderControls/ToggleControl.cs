using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    class ToggleControl : IShaderControl
    {
        public SerializableValueStore defaultValue { get; }

        public SlotValueType[] validPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Boolean }; }
        }

        private ShaderPort m_Port;

        public ToggleControl()
        {
        }

        public ToggleControl(bool defaultValue)
        {
            this.defaultValue = new SerializableValueStore()
            {
                booleanValue = defaultValue
            };
        }

        public VisualElement GetControl(ShaderPort port)
        {
            m_Port = port;

            VisualElement control = new VisualElement() { name = "ToggleControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/Controls/BooleanSlotControlView"));

            var toggleField = new Toggle() { value = m_Port.portValue.booleanValue };
            toggleField.RegisterValueChangedCallback(OnValueChanged);
            control.Add(toggleField);
            return control;
        }

        void OnValueChanged(ChangeEvent<bool> evt)
        {
            if (!evt.newValue.Equals(m_Port.portValue.booleanValue))
            {
                m_Port.owner.owner.owner.RegisterCompleteObjectUndo("Boolean Change");
                m_Port.portValue.booleanValue = evt.newValue;
                m_Port.owner.Dirty(ModificationScope.Node);
            }
        }
    }
}