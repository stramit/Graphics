using System.Collections.Generic;
using UnityEngine.Rendering;

namespace UnityEditor.ShaderGraph.Internal
{
    interface ITarget
    {
        string displayName { get; }
        string passTemplatePath { get; }
        string sharedTemplateDirectory { get; }

        bool Validate(IMasterNode masterNode);
        void SetupTarget(ref TargetSetupContext context);
    }
}
