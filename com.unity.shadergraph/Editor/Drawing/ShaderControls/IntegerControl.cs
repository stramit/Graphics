using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Drawing.Slots;

namespace UnityEditor.ShaderGraph
{
    class IntegerControl : IShaderControl
    {
        public SerializableValueStore defaultValue { get; }

        public SlotValueType[] validPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Vector1 }; }
        }

        ShaderPort m_Port;

        public IntegerControl()
        {
        }

        public IntegerControl(int defaultValue)
        {
            this.defaultValue = new SerializableValueStore()
            {
                vectorValue = new Vector4(defaultValue, 0.0f, 0.0f, 0.0f)
            };
        }

        public VisualElement GetControl(ShaderPort port)
        {
            m_Port = port;

            VisualElement control = new VisualElement() { name = "IntegerControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/Controls/MultiFloatSlotControlView"));

            var integerField = new IntegerField() { value = (int)m_Port.portValue.vectorValue.x };
            integerField.RegisterValueChangedCallback((evt) =>
            {
                if (evt.newValue != m_Port.portValue.vectorValue.x)
                {
                    m_Port.owner.owner.owner.RegisterCompleteObjectUndo("Change Integer");
                    integerField.value = evt.newValue;
                    m_Port.portValue.vectorValue = new Vector4(evt.newValue, 0.0f, 0.0f, 0.0f);
                    m_Port.owner.Dirty(ModificationScope.Node);
                }
            });

            control.Add(integerField);
            return control;
        }
    }
}
