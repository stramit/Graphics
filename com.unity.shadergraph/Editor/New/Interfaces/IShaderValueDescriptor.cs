using System;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    interface IShaderValueDescriptor
    {
        SerializableGuid guid { get; }
        int id { get; }

        SlotType portType { get; }
        SlotValueType valueType { get; }

        string name { get; }
    }
}
