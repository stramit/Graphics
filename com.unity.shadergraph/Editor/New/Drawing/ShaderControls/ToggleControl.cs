using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    class ToggleControl : IShaderControl
    {
        public ShaderControlData controlData { get; set; }
        public ShaderValueData defaultValueData { get; set; }

        public SlotValueType[] validPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Boolean }; }
        }

        public int portControlWidth
        {
            get { return 14; }
        }

        public ToggleControl()
        {
        }

        public ToggleControl(bool defaultValue)
        {
            this.defaultValueData = new ShaderValueData()
            {
                boolean = defaultValue
            };
        }

        public VisualElement GetControl(IShaderInput shaderInput)
        {
            VisualElement control = new VisualElement() { name = "ToggleControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/ShaderControls/ToggleControl"));

            var toggleField = new Toggle() { value = shaderInput.value.boolean };
            toggleField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue.Equals(shaderInput.value.boolean))
                    return;
                shaderInput.UpdateValueData(new ShaderValueData()
                {
                    boolean = evt.newValue
                });
            });
            control.Add(toggleField);
            return control;
        }
    }
}