using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    class LabelControl : IShaderControl
    {
        public ShaderControlData controlData { get; set; }
        public ShaderValueData defaultValueData { get; set; }

        public SlotValueType[] validPortTypes
        {
            get { return (SlotValueType[])Enum.GetValues(typeof(SlotValueType)); }
        }

        public int portControlWidth
        {
            get { return 42; }
        }

        public LabelControl()
        {
            this.controlData = new ShaderControlData()
            {
                labels = new string[] { "Label" }
            };
        }

        public LabelControl(string label, ShaderValueData value = new ShaderValueData())
        {
            defaultValueData = value;

            this.controlData = new ShaderControlData()
            {
                labels = new string[] { label }
            };
        }

        public VisualElement GetControl(IShaderInput shaderInput)
        {
            VisualElement control = new VisualElement() { name = "LabelControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/ShaderControls/LabelControl"));

            Label label = new Label(controlData.labels[0]);
            control.Add(label);
            return control;
        }
    }
}
