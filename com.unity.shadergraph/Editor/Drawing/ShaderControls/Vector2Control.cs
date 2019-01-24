using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.ShaderGraph.Drawing.Slots;

namespace UnityEditor.ShaderGraph
{
    class Vector2Control : IShaderControl
    {       
        public string[] labels { get; set; }
        public float[] values { get; set; }
        public SerializableValueStore defaultValue { get; }

        public ConcreteSlotValueType[] validPortTypes
        {
            get { return new ConcreteSlotValueType[] { ConcreteSlotValueType.Vector2 }; }
        }

        public Vector2Control()
        {
            labels = new string[] { "X", "Y" };
        }

        public Vector2Control(Vector2 defaultValue, string labelX = "X", string labelY = "Y")
        {
            this.defaultValue = new SerializableValueStore()
            {
                vectorValue = new Vector4(defaultValue.x, defaultValue.y, 0.0f, 0.0f)
            };
            labels = new string[] { labelX, labelY };
        }

        int m_UndoGroup = -1;

        public VisualElement GetControl(IShaderValue shaderValue)
        {
            VisualElement control = new VisualElement() { name = "VectorControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/ShaderControls/VectorControl"));
            
            for (var i = 0; i < 2; i++)
                AddField(control, shaderValue, i, labels[i]);
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
                    shaderValue.UpdateValue(new SerializableValueStore()
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
                        shaderValue.UpdateValue(new SerializableValueStore()
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
