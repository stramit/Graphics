using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UnityEditor.ShaderGraph
{
    class IntegerControl : IShaderControl
    {
        public ShaderControlData controlData { get; set; }
        public ShaderValueData defaultValueData { get; set; }

        public SlotValueType[] validPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Vector1 }; }
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
                vector = new Vector4(defaultValue, 0.0f, 0.0f, 0.0f)
            };
        }

        public VisualElement GetControl(IShaderInput shaderInput)
        {
            VisualElement control = new VisualElement() { name = "IntegerControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/ShaderControls/IntegerControl"));

            var integerField = new IntegerField() { value = (int)shaderInput.valueData.vector.x };
            integerField.RegisterValueChangedCallback((evt) =>
            {
                if (evt.newValue.Equals(shaderInput.valueData.vector.x))
                    return;
                shaderInput.UpdateValueData(new ShaderValueData()
                {
                    vector = new Vector4(evt.newValue, 0.0f, 0.0f, 0.0f)
                });
            });

            control.Add(integerField);
            return control;
        }
    }
}
