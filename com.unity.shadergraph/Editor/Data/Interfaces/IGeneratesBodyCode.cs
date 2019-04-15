namespace UnityEditor.ShaderGraph
{
    interface IGeneratesBodyCode
    {
        void GenerateNodeCode(ShaderSnippetRegistry registry, GraphContext graphContext, GenerationMode generationMode);
    }
}
