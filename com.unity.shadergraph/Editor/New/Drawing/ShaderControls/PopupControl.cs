using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UnityEditor.ShaderGraph
{
    class PopupControl : IShaderControl
    {
        public ShaderControlData controlData { get; set; }
        public ShaderValueData defaultValueData { get; }

        public ConcreteSlotValueType[] validPortTypes
        {
            get { return new ConcreteSlotValueType[] { ConcreteSlotValueType.Vector1 }; }
        }

        List<string> m_Entries;
        List<string> entries
        {
            get 
            {
                if(m_Entries == null)
                    m_Entries = controlData.labels.ToList();
                return m_Entries;
            }
        }

        public PopupControl()
        {
            this.controlData = new ShaderControlData()
            {
                labels = new string[] { "A", "B", "C" }
            };
        }

        public PopupControl(string[] entries, float defaultValue = 0)
        {
            this.defaultValueData = new ShaderValueData()
            {
                vectorValue = new Vector4(defaultValue, 0.0f, 0.0f, 0.0f)
            };
            this.controlData = new ShaderControlData()
            {
                labels = entries
            };
        }

        public VisualElement GetControl(IShaderValue shaderValue)
        {
            VisualElement control = new VisualElement() { name = "PopupControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/Controls/ScreenPositionSlotControlView"));

            var popupField = new PopupField<string>(entries, (int)shaderValue.value.vectorValue.x);
            popupField.RegisterValueChangedCallback(evt =>
            {
                if (popupField.index.Equals(shaderValue.value.vectorValue.x))
                    return;
                shaderValue.UpdateValue(new ShaderValueData()
                {
                    vectorValue = new Vector4(popupField.index, 0.0f, 0.0f, 0.0f)
                });
            });

            control.Add(popupField);
            return control;
        }
    }
}
