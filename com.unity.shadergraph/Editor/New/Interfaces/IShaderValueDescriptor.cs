using System;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    internal interface IShaderValueDescriptor
    {
        int id { get; set; }
        string name { get; set; }
        ConcreteSlotValueType valueType { get; }
        SlotType portType { get; }
    }
}