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
        public ShaderValueData defaultValueData { get; set; }

        public SlotValueType[] validPortTypes
        {
            get { return new SlotValueType[] 
                { 
                    SlotValueType.Texture2D,
                    SlotValueType.Texture3D,
                    SlotValueType.Texture2DArray,
                    SlotValueType.Cubemap 
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

        public VisualElement GetControl(IShaderInput shaderInput)
        {
            VisualElement control = new VisualElement() { name = "TextureControl" };
            control.styleSheets.Add(Resources.Load<StyleSheet>("Styles/ShaderControls/TextureControl"));

            var objectField = new ObjectField { objectType = typeof(T), value = shaderInput.value.texture };
            objectField.RegisterValueChangedCallback(evt =>
            {
                var texture = evt.newValue as T;
                if (texture.Equals(shaderInput.value.texture))
                    return;
                shaderInput.UpdateValueData(new ShaderValueData()
                {
                    texture = texture
                });
            });
            control.Add(objectField);
            return control;
        }
    }
}
