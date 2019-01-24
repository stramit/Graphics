using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.ShaderGraph
{
    internal interface IShaderControl
    {
        SerializableValueStore defaultValue { get; }
        ConcreteSlotValueType[] validPortTypes { get; }
        string[] labels { get; set; }
        float[] values { get; set; }
        VisualElement GetControl(IShaderValue shaderValue);
    }
}
