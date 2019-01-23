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
        public SerializableValueStore defaultValue { get; }

        public SlotValueType[] validPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Vector1 }; }
        }

        ShaderPort m_Port;
        Slider m_Slider;
        FloatField m_SliderInput;
        float m_Minimum = 0.0f;
        float m_Maximum = 1.0f;

        public SliderControl()
        {
        }

        public SliderControl(float defaultValue, float minimum, float maximum)
        {
            this.defaultValue = new SerializableValueStore()
            {
                vectorValue = new Vector4(defaultValue, 0.0f, 0.0f, 0.0f)
            };
            m_Minimum = minimum;
            m_Maximum = maximum;
        }

        public VisualElement GetControl(ShaderPort port)
        {
            m_Port = port;

            VisualElement control = new VisualElement() { name = "SliderControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/ShaderControls/SliderControl"));

            m_Slider = new Slider(m_Minimum, m_Maximum) { value = m_Port.portValue.vectorValue.x };
            m_Slider.RegisterValueChangedCallback((evt) =>
            {
                var newValue = evt.newValue;
                if (newValue != m_Port.portValue.vectorValue.x)
                {
                    m_Port.owner.owner.owner.RegisterCompleteObjectUndo("Change Slider");
                    m_SliderInput.value = newValue;
                    m_Port.portValue.vectorValue = new Vector4(newValue, 0.0f, 0.0f, 0.0f);
                    m_Port.owner.Dirty(ModificationScope.Node);
                    //this.MarkDirtyRepaint();
                }
            });

            m_SliderInput = new FloatField { value = m_Port.portValue.vectorValue.x };
            m_SliderInput.RegisterValueChangedCallback(evt =>
            {
                m_Port.portValue.vectorValue = new Vector4((float)evt.newValue, 0.0f, 0.0f, 0.0f);
                m_Port.owner.Dirty(ModificationScope.Node);
                //this.MarkDirtyRepaint();
            });
            m_SliderInput.Q("unity-text-input").RegisterCallback<FocusOutEvent>(evt =>
            {
                float newValue = Mathf.Max(Mathf.Min(m_Port.portValue.vectorValue.x, m_Maximum), m_Minimum);
                m_Port.owner.Dirty(ModificationScope.Node);
                m_Slider.value = m_Port.portValue.vectorValue.x;
                //UpdateSlider();
                //this.MarkDirtyRepaint();
            });

            control.Add(m_Slider);
            control.Add(m_SliderInput);
            return control;
        }
    }
}
