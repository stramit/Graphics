using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UnityEditor.ShaderGraph
{
    class IntegerControl : IShaderControl
    {
        public ShaderControlData controlData { get; set; }
        public ShaderValueData defaultValueData { get; }

        public ConcreteSlotValueType[] validPortTypes
        {
            get { return new ConcreteSlotValueType[] { ConcreteSlotValueType.Vector1 }; }
        }

        public int portControlWidth
        {
            get { return 32; }
        }

        public IntegerControl()
        {
        }

        public IntegerControl(int defaultValue)
        {
            this.defaultValueData = new ShaderValueData()
            {
                vectorValue = new Vector4(defaultValue, 0.0f, 0.0f, 0.0f)
            };
        }

        public VisualElement GetControl(IShaderValue shaderValue)
        {
            VisualElement control = new VisualElement() { name = "IntegerControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/ShaderControls/IntegerControl"));

            var integerField = new IntegerField() { value = (int)shaderValue.value.vectorValue.x };
            integerField.RegisterValueChangedCallback((evt) =>
            {
                if (evt.newValue.Equals(shaderValue.value.vectorValue.x))
                    return;
                shaderValue.UpdateValue(new ShaderValueData()
                {
                    vectorValue = new Vector4(evt.newValue, 0.0f, 0.0f, 0.0f)
                });
            });

            control.Add(integerField);
            return control;
        }
    }
}
