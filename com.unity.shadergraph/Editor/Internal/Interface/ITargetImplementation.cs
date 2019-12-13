using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.ShaderGraph.Internal
{
    interface ITargetImplementation
    {
        Type targetType { get; } 
        string displayName { get; }
        string passTemplatePath { get; }
        string sharedTemplateDirectory { get; }

        Type[] requireBlocks { get; }

        void SetupTarget(ref TargetSetupContext context);
        IEnumerable<Type> GetSupportedBlocks(IEnumerable<BlockData> currentBlocks);
    }
}
