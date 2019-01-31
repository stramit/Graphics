namespace UnityEditor.ShaderGraph
{
    interface IShaderInput : IShaderValue
    {
        ShaderValueData value { get; set; }

        IShaderControl control { get; set; }

        void CopyValueFrom(IShaderInput shaderInput);
    }
}
