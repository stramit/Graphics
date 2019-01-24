using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Drawing.Slots;

namespace UnityEditor.ShaderGraph
{
    class TextureControl<T> : IShaderControl where T : Texture
    {
        public ShaderControlData controlData { get; set; }
        public ShaderValueData defaultValueData { get; }

        public ConcreteSlotValueType[] validPortTypes
        {
            get { return new ConcreteSlotValueType[] 
                { 
                    ConcreteSlotValueType.Texture2D,
                    ConcreteSlotValueType.Texture3D,
                    ConcreteSlotValueType.Texture2DArray,
                    ConcreteSlotValueType.Cubemap 
                }; 
            }
        }

        public int portControlWidth
        {
            get { return 84; }
        }

        public TextureControl()
        {
        }

        public VisualElement GetControl(IShaderValue shaderValue)
        {
            VisualElement control = new VisualElement() { name = "TextureControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/ShaderControls/TextureControl"));

            var objectField = new ObjectField { objectType = typeof(T), value = shaderValue.value.textureValue };
            objectField.RegisterValueChangedCallback(evt =>
            {
                var texture = evt.newValue as T;
                if (texture.Equals(shaderValue.value.textureValue))
                    return;
                shaderValue.UpdateValue(new ShaderValueData()
                {
                    textureValue = texture
                });
            });
            control.Add(objectField);
            return control;
        }
    }
}
