namespace UnityEditor.ShaderGraph
{
    interface IPropertyFromNode
    {
        ShaderProperty AsShaderProperty();
        int outputSlotId { get; }
    }
}
