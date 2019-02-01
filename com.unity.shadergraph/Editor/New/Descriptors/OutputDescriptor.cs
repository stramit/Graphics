using System;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    struct OutputDescriptor : IShaderValueDescriptor
    {
        public SerializableGuid guid => new SerializableGuid();
        public int id { get; set; }

        public SlotType portType => SlotType.Output;
        public SlotValueType valueType { get; }

        public string name { get; set; }

        public OutputDescriptor(int id, string name, SlotValueType valueType)
        {
            this.id = id;
            this.name = name;
            this.valueType = valueType;
        }
    }
}
