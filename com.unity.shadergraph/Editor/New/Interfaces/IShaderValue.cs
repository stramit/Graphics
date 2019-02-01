namespace UnityEditor.ShaderGraph
{
    interface IShaderValue
    {
        SerializableGuid guid { get; }
        int id { get; }

        SlotValueType valueType { get; }
        ConcreteSlotValueType concreteValueType { get; }

        string displayName { get; }
        string outputName { get; }

        IShaderValue Copy();
    }
}
