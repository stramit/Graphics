namespace UnityEditor.ShaderGraph
{
    interface IShaderInput : IShaderValue
    {
        ShaderValueData value { get; set; }
        IShaderControl control { get; set; }

        void UpdateValueData(ShaderValueData value);
        void CopyValueFrom(IShaderInput shaderInput);        
    }
}
