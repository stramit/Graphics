using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UnityEditor.ShaderGraph
{
    class IntegerControl : IShaderControl
    {
        public string[] labels { get; set; }
        public float[] values { get; set; }
        public SerializableValueStore defaultValue { get; }

        public ConcreteSlotValueType[] validPortTypes
        {
            get { return new ConcreteSlotValueType[] { ConcreteSlotValueType.Vector1 }; }
        }

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

        public VisualElement GetControl(IShaderValue shaderValue)
        {
            VisualElement control = new VisualElement() { name = "IntegerControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/Controls/MultiFloatSlotControlView"));

            var integerField = new IntegerField() { value = (int)shaderValue.value.vectorValue.x };
            integerField.RegisterValueChangedCallback((evt) =>
            {
                if (evt.newValue.Equals(shaderValue.value.vectorValue.x))
                    return;
                shaderValue.UpdateValue(new SerializableValueStore()
                {
                    vectorValue = new Vector4(evt.newValue, 0.0f, 0.0f, 0.0f)
                });
            });

            control.Add(integerField);
            return control;
        }
    }
}
