using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.ShaderGraph
{
    internal interface IShaderControl
    {
        ShaderValueData defaultValueData { get; }
        ShaderControlData controlData { get; set; }
        ConcreteSlotValueType[] validPortTypes { get; }
        int portControlWidth { get; }

        VisualElement GetControl(IShaderValue shaderValue);
    }
}
