using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Drawing.Slots;

namespace UnityEditor.ShaderGraph
{
    class PopupControl : IShaderControl
    {
        public SerializableValueStore defaultValue { get; }

        public SlotValueType[] validPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Vector1 }; }
        }

        ShaderPort m_Port;
        PopupField<string> m_PopupField;
        List<string> m_Entries = new List<string>() {""};

        public PopupControl()
        {
        }

        public PopupControl(string[] entries)
        {
            m_Entries = entries.ToList();
        }

        public PopupControl(string[] entries, float defaultValue)
        {
            this.defaultValue = new SerializableValueStore()
            {
                vectorValue = new Vector4(defaultValue, 0.0f, 0.0f, 0.0f)
            };
            m_Entries = entries.ToList();
        }

        public VisualElement GetControl(ShaderPort port)
        {
            m_Port = port;

            VisualElement control = new VisualElement() { name = "PopupControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/Controls/ScreenPositionSlotControlView"));

            m_PopupField = new PopupField<string>(m_Entries, (int)m_Port.portValue.vectorValue.x);
            m_PopupField.RegisterValueChangedCallback(OnValueChanged);
            control.Add(m_PopupField);
            return control;
        }

        void OnValueChanged(ChangeEvent<string> evt)
        {
            if (m_PopupField.index != m_Port.portValue.vectorValue.x)
            {
                m_Port.owner.owner.owner.RegisterCompleteObjectUndo("Change Popup");
                m_Port.portValue.vectorValue = new Vector4(m_PopupField.index, 0.0f, 0.0f, 0.0f);
                m_Port.owner.Dirty(ModificationScope.Graph);
            }
        }
    }
}
