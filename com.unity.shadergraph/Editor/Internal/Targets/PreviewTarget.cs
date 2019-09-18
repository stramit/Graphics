using System.Collections.Generic;
using UnityEngine.Rendering;

namespace UnityEditor.ShaderGraph.Internal
{
    class PreviewTarget : ITarget
    {
        public string displayName => "PREVIEW";
        public string passTemplatePath => GenerationUtils.GetDefaultTemplatePath("PassMesh.template");
        public string sharedTemplateDirectory => GenerationUtils.GetDefaultSharedTemplateDirectory();

        public bool Validate(RenderPipelineAsset pipelineAsset)
        {
            return false;
        }

        public bool TryGetSubShader(IMasterNode masterNode, out ISubShader subShader)
        {
            subShader = null;
            return false;
        }
    }
}
