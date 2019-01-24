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

        public TextureControl()
        {
        }

        public VisualElement GetControl(IShaderValue shaderValue)
        {
            VisualElement control = new VisualElement() { name = "TextureControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/Controls/TextureSlotControlView"));

            var objectField = new ObjectField { objectType = typeof(T), value = shaderValue.value.textureValue };
            objectField.RegisterValueChangedCallback(evt =>
            {
                var texture = evt.newValue as T;
                if (texture.Equals(shaderValue.value.textureValue))
                    return;
                shaderValue.UpdateValue(new SerializableValueStore()
                {
                    textureValue = texture
                });
            });
            control.Add(objectField);
            return control;
        }
    }
}
