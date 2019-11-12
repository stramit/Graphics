using System;

namespace UnityEditor.ShaderGraph.Internal
{
    [Flags]
    public enum StructFieldOptions
    {
        Static = 0,
        Optional = 1 << 0,
        Generated = 1 << 1,
        Hidden = 1 << 2
    }
}
