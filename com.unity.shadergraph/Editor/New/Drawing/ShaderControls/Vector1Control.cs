using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.ShaderGraph.Drawing.Slots;

namespace UnityEditor.ShaderGraph
{
    class Vector1Control : IShaderControl
    {
        public ShaderControlData controlData { get; set; }
        public ShaderValueData defaultValueData { get; }

        public ConcreteSlotValueType[] validPortTypes
        {
            get { return new ConcreteSlotValueType[] { ConcreteSlotValueType.Vector1 }; }
        }

        public int portControlWidth
        {
            get { return 42; }
        }

        public Vector1Control()
        {
            this.controlData = new ShaderControlData()
            {
                labels = new string[] { "X" }
            };
        }

        public Vector1Control(float defaultValue, string labelX = "X")
        {
            this.defaultValueData = new ShaderValueData()
            {
                vectorValue = new Vector4(defaultValue, 0.0f, 0.0f, 0.0f)
            };
            this.controlData = new ShaderControlData()
            {
                labels = new string[] { labelX }
            };
        }

        int m_UndoGroup = -1;

        public VisualElement GetControl(IShaderValue shaderValue)
        {
            VisualElement control = new VisualElement() { name = "VectorControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/ShaderControls/VectorControl"));
            
            AddField(control, shaderValue, 0, controlData.labels[0]);
            return control;
        }

        void AddField(VisualElement element, IShaderValue shaderValue, int index, string subLabel)
        {
            var dummy = new VisualElement { name = "dummy" };
            var label = new Label(subLabel);
            dummy.Add(label);
            element.Add(dummy);
            var field = new FloatField { userData = index, value = shaderValue.value.vectorValue[index] };
            var dragger = new FieldMouseDragger<float>(field);
            dragger.SetDragZone(label);
            field.RegisterValueChangedCallback(evt =>
                {
                    if(evt.newValue.Equals(shaderValue.value.vectorValue))
                        return;
                    var value = shaderValue.value.vectorValue;
                    value[index] = (float)evt.newValue;
                    shaderValue.UpdateValue(new ShaderValueData()
                    {
                        vectorValue = value
                    });
                });
            field.Q("unity-text-input").RegisterCallback<InputEvent>(evt =>
                {
                    if (m_UndoGroup == -1)
                    {
                        m_UndoGroup = Undo.GetCurrentGroup();
                        shaderValue.UpdateValue(shaderValue.value);
                    }
                    float newValue;
                    if (!float.TryParse(evt.newData, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out newValue))
                        newValue = 0f;
                    var value = shaderValue.value.vectorValue;
                    if (Mathf.Abs(value[index] - newValue) > 1e-9)
                    {
                        value[index] = newValue;
                        shaderValue.UpdateValue(new ShaderValueData()
                        {
                            vectorValue = value
                        });
                    }
                });
            field.Q("unity-text-input").RegisterCallback<KeyDownEvent>(evt =>
                {
                    if (evt.keyCode == KeyCode.Escape && m_UndoGroup > -1)
                    {
                        Undo.RevertAllDownToGroup(m_UndoGroup);
                        m_UndoGroup = -1;
                        evt.StopPropagation();
                    }
                    element.MarkDirtyRepaint();
                });
            element.Add(field);
        }
    }
}
