using System;

namespace UnityEditor.ShaderGraph.Internal
{
    interface ITargetImplementation
    {
        Type targetType { get; } 
        string displayName { get; }
        string passTemplatePath { get; }
        string sharedTemplateDirectory { get; }

        void SetupTarget(ref TargetSetupContext context);
    }
}
