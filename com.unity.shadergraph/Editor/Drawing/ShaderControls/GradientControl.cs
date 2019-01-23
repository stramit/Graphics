using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Graphing;
using System.Linq;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    class SerializedGradient
    {
        [NonSerialized]
        Gradient m_Gradient = new Gradient();

        [SerializeField]
        Vector4[] m_SerializableColorKeys = { new Vector4(1f, 1f, 1f, 0f), new Vector4(0f, 0f, 0f, 1f), };

        [SerializeField]
        Vector2[] m_SerializableAlphaKeys = { new Vector2(1f, 0f), new Vector2(1f, 1f) };

        [SerializeField]
        int m_SerializableMode = 0;
        
        public Gradient gradient
        {
            get
            {
                if (m_SerializableAlphaKeys != null && m_SerializableColorKeys != null)
                {
                    m_Gradient = new Gradient();
                    var colorKeys = m_SerializableColorKeys.Select(k => new GradientColorKey(new Color(k.x, k.y, k.z, 1f), k.w)).ToArray();
                    var alphaKeys = m_SerializableAlphaKeys.Select(k => new GradientAlphaKey(k.x, k.y)).ToArray();
                    m_SerializableAlphaKeys = null;
                    m_SerializableColorKeys = null;
                    m_Gradient.SetKeys(colorKeys, alphaKeys);
                    m_Gradient.mode = (GradientMode)m_SerializableMode;
                }

                return m_Gradient;
            }
            set
            {
                if (!GradientUtils.CheckEquivalency(gradient, value))
                {
                    var newColorKeys = value.colorKeys;
                    var newAlphaKeys = value.alphaKeys;
                    m_Gradient.SetKeys(newColorKeys, newAlphaKeys);
                    m_Gradient.mode = value.mode;
                }
            }
        }

        public void Serialize(Gradient value)
        {
            m_SerializableColorKeys = value.colorKeys.Select(k => new Vector4(k.color.r, k.color.g, k.color.b, k.time)).ToArray();
            m_SerializableAlphaKeys = value.alphaKeys.Select(k => new Vector2(k.alpha, k.time)).ToArray();
            m_SerializableMode = (int)value.mode;
        }
    }

    class GradientControl : IShaderControl
    {
        public Gradient defaultValue { get; }

        private ShaderPort m_Port;

        public GradientControl()
        {
            this.defaultValue = new Gradient() { colorKeys = new GradientColorKey[] { new GradientColorKey(Color.black, 0), new GradientColorKey(Color.white, 1) } };
        }

        public GradientControl(Gradient defaultValue)
        {
            this.defaultValue = defaultValue;
        }

        public SlotValueType[] GetValidPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Gradient }; }
        }

        public void UpdateDefaultValue(ShaderPort port)
        {
            m_Port = port;
            port.gradientValue = defaultValue;
        }

        public VisualElement GetControl(ShaderPort port)
        {
            VisualElement control = new VisualElement() { name = "GradientControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/Controls/GradientSlotControlView"));

            var gradientField = new GradientField() { value = m_Port.gradientValue };
            gradientField.RegisterValueChangedCallback(OnValueChanged);
            control.Add(gradientField);
            return control;
        }

        void OnValueChanged(ChangeEvent<Gradient> evt)
        {
            m_Port.owner.owner.owner.RegisterCompleteObjectUndo("Change Gradient");
            m_Port.SetGradientValue(evt.newValue);
            //m_Port.gradientValue = m_Port.gradientValue;
            m_Port.owner.Dirty(ModificationScope.Node);

            if (!evt.newValue.Equals(m_Port.gradientValue))
            {
                //m_Port.gradientValue.SetKeys(evt.newValue.colorKeys, evt.newValue.alphaKeys);
                //m_Port.gradientValue.mode = evt.newValue.mode;
                m_Port.owner.owner.owner.RegisterCompleteObjectUndo("Change Gradient");
                m_Port.SetGradientValue(evt.newValue);
                //m_Port.gradientValue = m_Port.gradientValue;
                m_Port.owner.Dirty(ModificationScope.Node);
            }
        }
    }
}
