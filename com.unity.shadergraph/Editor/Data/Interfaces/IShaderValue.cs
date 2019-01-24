using System;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    internal interface IShaderValue
    {
        ConcreteSlotValueType concreteValueType { get; }
        SerializableValueStore value { get; }
        string shaderOutputName { get; }
        void UpdateValue(SerializableValueStore value);
    }
}
