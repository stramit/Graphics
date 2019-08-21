using System.Collections.Generic;

namespace UnityEditor.ShaderGraph.Internal
{
    public interface IGraphTarget
    {
        string displayName { get; }
        string targetTemplatePath { get; }
        IEnumerable<string> assetDependencyPaths { get; }
        
        void Generate(IGeneratorContext context);
    }
}
