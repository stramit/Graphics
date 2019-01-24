using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.ShaderGraph
{
    internal interface IShaderValue
    {
        SerializableValueStore value { get; }
        void UpdateValue(SerializableValueStore value);
    }
}
