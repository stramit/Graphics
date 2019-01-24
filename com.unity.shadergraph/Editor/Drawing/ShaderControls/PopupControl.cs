using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UnityEditor.ShaderGraph
{
    class PopupControl : IShaderControl
    {
        public SerializableValueStore defaultValue { get; }

        public ConcreteSlotValueType[] validPortTypes
        {
            get { return new ConcreteSlotValueType[] { ConcreteSlotValueType.Vector1 }; }
        }

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

        public VisualElement GetControl(IShaderValue shaderValue)
        {
            VisualElement control = new VisualElement() { name = "PopupControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/Controls/ScreenPositionSlotControlView"));

            var popupField = new PopupField<string>(m_Entries, (int)shaderValue.value.vectorValue.x);
            popupField.RegisterValueChangedCallback(evt =>
            {
                if (popupField.index.Equals(shaderValue.value.vectorValue.x))
                    return;
                shaderValue.UpdateValue(new SerializableValueStore()
                {
                    vectorValue = new Vector4(popupField.index, 0.0f, 0.0f, 0.0f)
                });
            });

            control.Add(popupField);
            return control;
        }
    }
}
