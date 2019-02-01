using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    internal static class ShaderInputExtensions
    {

#region Snippets
        internal static string ToValueSnippet(this IShaderInput shaderInput, AbstractMaterialNode.OutputPrecision precision)
        {
            var channelCount = SlotValueHelper.GetChannelCount(shaderInput.concreteValueType);
            Matrix4x4 matrix = shaderInput.valueData.matrix;
            switch (shaderInput.concreteValueType)
            {
                case ConcreteSlotValueType.Vector1:
                    return NodeUtils.FloatToShaderValue(shaderInput.valueData.vector.x);
                case ConcreteSlotValueType.Vector4:
                case ConcreteSlotValueType.Vector3:
                case ConcreteSlotValueType.Vector2:
                    {
                        string values = NodeUtils.FloatToShaderValue(shaderInput.valueData.vector.x);
                        for (var i = 1; i < channelCount; i++)
                            values += ", " + NodeUtils.FloatToShaderValue(shaderInput.valueData.vector[i]);
                        return string.Format("{0}{1}({2})", precision, channelCount, values);
                    }
                case ConcreteSlotValueType.Boolean:
                    return (shaderInput.valueData.boolean ? 1 : 0).ToString();
                case ConcreteSlotValueType.Texture2D:
                case ConcreteSlotValueType.Texture3D:
                case ConcreteSlotValueType.Texture2DArray:
                case ConcreteSlotValueType.Cubemap:
                case ConcreteSlotValueType.SamplerState:
                    return shaderInput.ToVariableNameSnippet();
                case ConcreteSlotValueType.Matrix2:
                    return string.Format("{0}2x2 ({1},{2},{3},{4})", precision, 
                        matrix.m00, matrix.m01, 
                        matrix.m10, matrix.m11);
                case ConcreteSlotValueType.Matrix3:
                    return string.Format("{0}3x3 ({1},{2},{3},{4},{5},{6},{7},{8},{9})", precision, 
                        matrix.m00, matrix.m01, matrix.m02, 
                        matrix.m10, matrix.m11, matrix.m12,
                        matrix.m20, matrix.m21, matrix.m22);
                case ConcreteSlotValueType.Matrix4:
                    return string.Format("{0}4x4 ({1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16})", precision, 
                        matrix.m00, matrix.m01, matrix.m02, matrix.m03, 
                        matrix.m10, matrix.m11, matrix.m12, matrix.m13,
                        matrix.m20, matrix.m21, matrix.m22, matrix.m23,
                        matrix.m30, matrix.m31, matrix.m32, matrix.m33);
                case ConcreteSlotValueType.Gradient:
                    return string.Format("Unity{0}()", shaderInput.ToVariableNameSnippet());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal static string ToValueReferenceSnippet(this IShaderValue shaderValue, AbstractMaterialNode.OutputPrecision precision, GenerationMode generationMode)
        {
            if (shaderValue is ShaderParameter parameter)
            {
                if (generationMode.IsPreview())
                    return parameter.ToVariableNameSnippet();

                return parameter.ToValueSnippet(precision);
            }

            if (shaderValue is ShaderPort port)
                return port.InputValue(port.owner.owner, generationMode);

            return string.Empty;
        }
#endregion

#region Properties
        internal static IShaderProperty[] ToDefaultPropertyArray(this IShaderInput shaderInput, string overrideReferenceName = null)
        {
            if(shaderInput.concreteValueType == ConcreteSlotValueType.Gradient)
                return GradientShaderValueToDefaultPropertyArray(shaderInput, overrideReferenceName);

            IShaderProperty property = shaderInput.concreteValueType.ToShaderProperty();

            property.generatePropertyBlock = false;

            if(overrideReferenceName != null)
                property.overrideReferenceName = overrideReferenceName;
            
            return new IShaderProperty[] { property };
        }

        private static IShaderProperty[] GradientShaderValueToDefaultPropertyArray(IShaderInput shaderInput, string overrideReferenceName)
        {
            List<IShaderProperty> properties = new List<IShaderProperty>();
            properties.Add(new Vector1ShaderProperty()
            {
                overrideReferenceName = string.Format("{0}_Type", overrideReferenceName),
                value = (int)shaderInput.valueData.gradient.mode,
                generatePropertyBlock = false
            });
            properties.Add(new Vector1ShaderProperty()
            {
                overrideReferenceName = string.Format("{0}_ColorsLength", overrideReferenceName),
                value = shaderInput.valueData.gradient.colorKeys.Length,
                generatePropertyBlock = false
            });
            properties.Add(new Vector1ShaderProperty()
            {
                overrideReferenceName = string.Format("{0}_AlphasLength", overrideReferenceName),
                value = shaderInput.valueData.gradient.alphaKeys.Length,
                generatePropertyBlock = false
            });
            for (int i = 0; i < 8; i++)
            {
                properties.Add(new Vector4ShaderProperty()
                {
                    overrideReferenceName = string.Format("{0}_ColorKey{1}", overrideReferenceName, i),
                    value = i < shaderInput.valueData.gradient.colorKeys.Length ? GradientUtils.ColorKeyToVector(shaderInput.valueData.gradient.colorKeys[i]) : Vector4.zero,
                    generatePropertyBlock = false
                });
            }
            for (int i = 0; i < 8; i++)
            {
                properties.Add(new Vector4ShaderProperty()
                {
                    overrideReferenceName = string.Format("{0}_AlphaKey{1}", overrideReferenceName, i),
                    value = i < shaderInput.valueData.gradient.alphaKeys.Length ? GradientUtils.AlphaKeyToVector(shaderInput.valueData.gradient.alphaKeys[i]) : Vector2.zero,
                    generatePropertyBlock = false
                });
            }

            var prop = new GradientShaderProperty();
            if(overrideReferenceName != null)
                prop.overrideReferenceName = overrideReferenceName;
            prop.generatePropertyBlock = false;
            prop.value = shaderInput.valueData.gradient;
            prop.OverrideMembers(overrideReferenceName);
            properties.Add(prop);

            return properties.ToArray();
        }

        internal static PreviewProperty ToPreviewProperty(this IShaderInput shaderInput, string name)
        {
            var propType = shaderInput.concreteValueType.ToPropertyType();
            var pp = new PreviewProperty(propType) { name = name };

            switch (propType)
            {
                case PropertyType.Vector4:
                case PropertyType.Vector3:
                case PropertyType.Vector2:
                    pp.vector4Value = shaderInput.valueData.vector;
                    break;
                case PropertyType.Vector1:
                    pp.floatValue = shaderInput.valueData.vector.x;
                    break;
                case PropertyType.Boolean:
                    pp.booleanValue = shaderInput.valueData.boolean;
                    break;
                case PropertyType.Texture2D:
                case PropertyType.Texture3D:
                case PropertyType.Texture2DArray:
                    pp.textureValue = shaderInput.valueData.texture;
                    break;
                case PropertyType.Cubemap: // TODO - Remove PreviewProperty.cubemapValue
                    pp.cubemapValue = (Cubemap)shaderInput.valueData.texture;
                    break;
                case PropertyType.SamplerState:
                    break;
                case PropertyType.Matrix2:
                case PropertyType.Matrix3:
                case PropertyType.Matrix4:
                    break;
                case PropertyType.Gradient:
                    pp.gradientValue = shaderInput.valueData.gradient;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return pp;
        }
#endregion

    }
}
