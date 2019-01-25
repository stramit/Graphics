using System;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    internal interface IShaderValueDescriptor
    {
        int id { get; }
        string name { get; }
        ConcreteSlotValueType valueType { get; }
        SlotType portType { get; }
    }
}
