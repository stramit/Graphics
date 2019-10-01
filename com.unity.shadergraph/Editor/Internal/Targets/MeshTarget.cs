using System.Collections.Generic;
using UnityEngine.Rendering;

namespace UnityEditor.ShaderGraph.Internal
{
    class MeshTarget : ITarget
    {
        public string displayName => "Mesh";
        public string passTemplatePath => GenerationUtils.GetDefaultTemplatePath("PassMesh.template");
        public string sharedTemplateDirectory => GenerationUtils.GetDefaultSharedTemplateDirectory();

        public bool Validate(IMasterNode masterNode)
        {
            return false;
        }

        public void SetupTarget(ref TargetSetupContext context)
        {

        }
    }
}
