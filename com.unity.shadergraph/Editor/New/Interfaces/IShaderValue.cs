using System;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    internal interface IShaderValue
    {
        INode owner { get; set; }
        Guid guid { get; }
        int id { get; }

        ConcreteSlotValueType concreteValueType { get; }
        ShaderValueData value { get; }
        string shaderOutputName { get; }
        string displayName { get; set; }

        void UpdateValue(ShaderValueData value);

        INode ToConcreteNode();
    }
}
