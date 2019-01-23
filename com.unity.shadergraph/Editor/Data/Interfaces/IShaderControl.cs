using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.ShaderGraph
{
    internal interface IShaderControl
    {
        SerializableValueStore defaultValue { get; }
        SlotValueType[] validPortTypes { get; }
        VisualElement GetControl(ShaderPort port);
    }
}
