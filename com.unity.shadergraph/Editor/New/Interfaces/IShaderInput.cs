namespace UnityEditor.ShaderGraph
{
    interface IShaderInput : IShaderValue
    {
        ShaderValueData valueData { get; set; }
        IShaderControl control { get; set; }

        void UpdateValueData(ShaderValueData value);   
    }
}
