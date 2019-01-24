using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    class ToggleControl : IShaderControl
    {
        public string[] labels { get; set; }
        public float[] values { get; set; }
        public SerializableValueStore defaultValue { get; }

        public ConcreteSlotValueType[] validPortTypes
        {
            get { return new ConcreteSlotValueType[] { ConcreteSlotValueType.Boolean }; }
        }

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

        public VisualElement GetControl(IShaderValue shaderValue)
        {
            VisualElement control = new VisualElement() { name = "ToggleControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/Controls/BooleanSlotControlView"));

            var toggleField = new Toggle() { value = shaderValue.value.booleanValue };
            toggleField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue.Equals(shaderValue.value.booleanValue))
                    return;
                shaderValue.UpdateValue(new SerializableValueStore()
                {
                    booleanValue = evt.newValue
                });
            });
            control.Add(toggleField);
            return control;
        }
    }
}