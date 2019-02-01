using System;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    internal static class ShaderValueExtensions
    {
        internal static string ToVariableSnippet(this IShaderValue shaderValue)
        {
            return string.Format("_{0}_{1}", NodeUtils.GetHLSLSafeName(shaderValue.outputName), GuidEncoder.Encode(shaderValue.guid.guid));
        }
    }
}
