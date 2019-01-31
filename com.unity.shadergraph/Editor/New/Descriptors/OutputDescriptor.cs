using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    class OutputDescriptor : IShaderValueDescriptor
    {
        public SerializableGuid guid => new SerializableGuid();

        public SlotType portType => SlotType.Input;
        public SlotValueType valueType { get; }

        public string name { get; }

        public OutputDescriptor(string name, SlotValueType valueType)
        {
            this.name = name;
            this.valueType = valueType;
        }
    }
}
