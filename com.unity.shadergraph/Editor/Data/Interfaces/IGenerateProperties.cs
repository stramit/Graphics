namespace UnityEditor.ShaderGraph
{
    interface IGenerateProperties
    {
        void CollectGraphInputs(PropertyCollector properties, GenerationMode generationMode);
    }
}
