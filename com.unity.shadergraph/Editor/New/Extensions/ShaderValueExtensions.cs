using System;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    internal static class ShaderValueExtensions
    {

#region Snippets
        internal static string ToVariableSnippet(this IShaderValue shaderValue)
        {
            if(shaderValue is ShaderPort port)
            {
                var matOwner = port.owner as AbstractMaterialNode;
                return string.Format("_{0}_{1}", matOwner.GetVariableNameForNode(), NodeUtils.GetHLSLSafeName(port.outputName));
            }
            if(shaderValue is ShaderParameter parameter)
            {
                var matOwner = parameter.owner as AbstractMaterialNode;
                return string.Format("_{0}_{1}", matOwner.GetVariableNameForNode(), NodeUtils.GetHLSLSafeName(parameter.outputName));
            }
            return string.Format("_{0}_{1}", NodeUtils.GetHLSLSafeName(shaderValue.outputName), GuidEncoder.Encode(shaderValue.guid.guid));
        }

        internal static string ToVariableDefinitionSnippet(this IShaderValue shaderValue, AbstractMaterialNode.OutputPrecision precision)
        {
            return string.Format("{0} {1}",
                NodeUtils.ConvertConcreteSlotValueTypeToString(precision, shaderValue.concreteValueType),
                shaderValue.ToVariableSnippet());
        }
#endregion

    }
}
