using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Drawing.Slots;

namespace UnityEditor.ShaderGraph
{
    class SliderControl : IShaderControl
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

        public SliderControl()
        {
            this.controlData = new ShaderControlData()
            {
                values = new float[] { 0, 1 }
            };
        }

        public SliderControl(float defaultValue, float minimum, float maximum)
        {
            this.defaultValueData = new ShaderValueData()
            {
                vector = new Vector4(defaultValue, 0.0f, 0.0f, 0.0f)
            };
            this.controlData = new ShaderControlData()
            {
                values = new float[] { minimum, maximum }
            };
        }

        public VisualElement GetControl(IShaderInput shaderInput)
        {
            VisualElement control = new VisualElement() { name = "SliderControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/ShaderControls/SliderControl"));

            Slider slider = null;
            FloatField floatField = null;

            slider = new Slider(controlData.values[0], controlData.values[1]) { value = shaderInput.valueData.vector.x };
            slider.RegisterValueChangedCallback((evt) =>
            {
                if (evt.newValue.Equals(shaderInput.valueData.vector.x))
                    return;
                floatField.value = evt.newValue;
                shaderInput.UpdateValueData(new ShaderValueData()
                {
                    vector = new Vector4((float)evt.newValue, 0.0f, 0.0f, 0.0f)
                });
            });

            floatField = new FloatField { value = shaderInput.valueData.vector.x };
            floatField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue.Equals(shaderInput.valueData.vector.x))
                    return;
                shaderInput.UpdateValueData(new ShaderValueData()
                {
                    vector = new Vector4((float)evt.newValue, 0.0f, 0.0f, 0.0f)
                });
            });
            floatField.Q("unity-text-input").RegisterCallback<FocusOutEvent>(evt =>
            {
                float newValue = Mathf.Max(Mathf.Min(shaderInput.valueData.vector.x, controlData.values[1]), controlData.values[0]);
                if (newValue.Equals(shaderInput.valueData.vector.x))
                    return;
                slider.value = newValue;
                shaderInput.UpdateValueData(new ShaderValueData()
                {
                    vector = new Vector4(newValue, 0.0f, 0.0f, 0.0f)
                });
            });

            control.Add(slider);
            control.Add(floatField);
            return control;
        }
    }
}
