using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    class GradientControl : IShaderControl
    {
        public SerializableValueStore defaultValue { get; }

        public SlotValueType[] validPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Gradient }; }
        }

        private ShaderPort m_Port;

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

        public VisualElement GetControl(ShaderPort port)
        {
            m_Port = port;

            VisualElement control = new VisualElement() { name = "GradientControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/Controls/GradientSlotControlView"));

            var gradientField = new GradientField() { value = m_Port.portValue.gradientValue };
            gradientField.RegisterValueChangedCallback(OnValueChanged);
            control.Add(gradientField);
            return control;
        }

        void OnValueChanged(ChangeEvent<Gradient> evt)
        {
            if (!evt.newValue.Equals(m_Port.portValue.gradientValue))
            {
                m_Port.owner.owner.owner.RegisterCompleteObjectUndo("Change Gradient");
                m_Port.portValue.gradientValue = evt.newValue;
                m_Port.owner.Dirty(ModificationScope.Node);
            }
        }
    }
}