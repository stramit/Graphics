using UnityEngine.UIElements;

namespace UnityEditor.ShaderGraph
{
    interface IShaderControl
    {
        ShaderValueData defaultValueData { get; set; }
        ShaderControlData controlData { get; set; }

        SlotValueType[] validPortTypes { get; }
        int portControlWidth { get; }

        VisualElement GetControl(IShaderInput shaderInput);
    }
}