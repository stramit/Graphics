using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    interface IPortDescriptor
    {
        int id { get; }
        string name { get; }
        SlotType portType { get; }
        SlotValueType valueType { get; }
    }
}
