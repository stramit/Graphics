using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    struct ValueDescriptor : IShaderValueDescriptor
    {
        public SerializableGuid guid => new SerializableGuid();

        public SlotType portType => SlotType.Input;
        public SlotValueType valueType { get; }

        public string name { get; }

        public ValueDescriptor(string name, SlotValueType valueType)
        {
            this.name = name;
            this.valueType = valueType;
        }
    }
}
