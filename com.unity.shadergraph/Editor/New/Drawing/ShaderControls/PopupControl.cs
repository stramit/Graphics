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
        public ShaderValueData defaultValueData { get; set; }

        public SlotValueType[] validPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Vector1 }; }
        }

        public int portControlWidth
        {
            get { return 84; }
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
                vector = new Vector4(defaultValue, 0.0f, 0.0f, 0.0f)
            };
            this.controlData = new ShaderControlData()
            {
                labels = entries
            };
        }

        public VisualElement GetControl(IShaderInput shaderInput)
        {
            VisualElement control = new VisualElement() { name = "PopupControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/ShaderControls/PopupControl"));

            var popupField = new PopupField<string>(entries, (int)shaderInput.valueData.vector.x);
            popupField.RegisterValueChangedCallback(evt =>
            {
                if (popupField.index.Equals(shaderInput.valueData.vector.x))
                    return;
                shaderInput.UpdateValueData(new ShaderValueData()
                {
                    vector = new Vector4(popupField.index, 0.0f, 0.0f, 0.0f)
                });
            });

            control.Add(popupField);
            return control;
        }
    }
}
