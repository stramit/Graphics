using System;

namespace UnityEditor.ShaderGraph.Internal
{
    class DefaultVFXTarget : ITargetImplementation
    {
        public Type targetType => typeof(VFXTarget);
        public string displayName => "Default";
        public string passTemplatePath => null;
        public string sharedTemplateDirectory => null;

        public Type[] requireBlocks => null;

        public void SetupTarget(ref TargetSetupContext context)
        {
        }
    }
}
