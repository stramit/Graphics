namespace UnityEditor.ShaderGraph
{
    interface IShaderValue
    {
        SerializableGuid guid { get; }

        SlotValueType valueType { get; }
        ConcreteSlotValueType concreteValueType { get; }

        string displayName { get; }
        string outputName { get; }

        IShaderValue Copy();
    }
}
