namespace UnityEditor.ShaderGraph.Internal
{
    public interface IGeneratorContext
    {
        GraphInfo graphInfo { get; }

        void SetPipelineTag(string value);
        void SetDefaultEditorPath(string value);
        void AddPass(ShaderPass pass);
    }
}
