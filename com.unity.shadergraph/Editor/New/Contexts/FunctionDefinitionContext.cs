namespace UnityEditor.ShaderGraph
{
    public struct FunctionDefinitionContext
    {
        internal HlslFunctionDescriptor function { get; set; }

        internal void SetHlslFunction(HlslFunctionDescriptor functionDescriptor)
        {
            function = functionDescriptor;
        }
    }
}
