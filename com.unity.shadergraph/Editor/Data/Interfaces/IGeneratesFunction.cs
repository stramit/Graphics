namespace UnityEditor.ShaderGraph
{
    interface IGeneratesFunction
    {
        void GenerateNodeFunction(ShaderSnippetRegistry registry, GraphContext graphContext, GenerationMode generationMode);
    }
}
