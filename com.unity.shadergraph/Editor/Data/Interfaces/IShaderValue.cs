using System;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    internal interface IShaderValue
    {
        SlotValueType valueType { get; }
        SerializableValueStore value { get; }
        void UpdateValue(SerializableValueStore value);
    }
}
