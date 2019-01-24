using System;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    internal interface IShaderValue
    {
        INode owner { get; set; }
        int id { get; }

        ConcreteSlotValueType concreteValueType { get; }
        ShaderValueData value { get; }
        string shaderOutputName { get; }

        void UpdateValue(ShaderValueData value);
    }
}
