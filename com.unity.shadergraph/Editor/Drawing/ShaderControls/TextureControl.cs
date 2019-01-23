using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Drawing.Slots;

namespace UnityEditor.ShaderGraph
{
    class TextureControl<T> : IShaderControl where T : Texture
    {
        public SerializableValueStore defaultValue { get; }

        public SlotValueType[] validPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Texture2D, SlotValueType.Texture3D, SlotValueType.Texture2DArray, SlotValueType.Cubemap }; }
        }

        ShaderPort m_Port;

        public TextureControl()
        {
        }

        public VisualElement GetControl(ShaderPort port)
        {
            m_Port = port;

            VisualElement control = new VisualElement() { name = "TextureControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/Controls/TextureSlotControlView"));

            var objectField = new ObjectField { objectType = typeof(T), value = m_Port.portValue.textureValue };
            objectField.RegisterValueChangedCallback(OnValueChanged);
            control.Add(objectField);
            return control;
        }

        void OnValueChanged(ChangeEvent<Object> evt)
        {
            var texture = evt.newValue as T;
            if (texture != m_Port.portValue.textureValue)
            {
                m_Port.owner.owner.owner.RegisterCompleteObjectUndo("Change Texture");
                m_Port.portValue.textureValue = texture;
                m_Port.owner.Dirty(ModificationScope.Node);
            }
        }
    }
}
