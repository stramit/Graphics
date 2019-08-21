using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    class GeneratorContext : IGeneratorContext
    {
        GraphInfo m_GraphInfo;
        public GraphInfo graphInfo => m_GraphInfo;

        public List<ShaderPass> passes;
        public string pipelineTag { get; private set; }
        public string defaultEditorPath { get; private set; }

        internal GeneratorContext(GraphInfo graphInfo)
        {
            m_GraphInfo = graphInfo;
            passes = new List<ShaderPass>();
        }

        public void SetPipelineTag(string value)
        {
            pipelineTag = value;
        }

        public void SetDefaultEditorPath(string value)
        {
            defaultEditorPath = value;
        }

        public void AddPass(ShaderPass pass)
        {
            passes.Add(pass);
        }
    }
}
