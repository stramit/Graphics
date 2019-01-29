using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    internal static class ShaderValueExtensions
    {
        internal static string ToVariableName(this IShaderValue shaderValue)
        {
            var matOwner = shaderValue.owner as AbstractMaterialNode;
            return string.Format("_{0}_{1}", matOwner.GetVariableNameForNode(), NodeUtils.GetHLSLSafeName(shaderValue.shaderOutputName));
        }

        internal static string ToVariableValue(this IShaderValue shaderValue, AbstractMaterialNode.OutputPrecision precision)
        {
            var matOwner = shaderValue.owner as AbstractMaterialNode;
            var channelCount = SlotValueHelper.GetChannelCount(shaderValue.concreteValueType);
            Matrix4x4 matrix = shaderValue.value.matrixValue;
            switch (shaderValue.concreteValueType)
            {
                case ConcreteSlotValueType.Vector1:
                    return NodeUtils.FloatToShaderValue(shaderValue.value.vectorValue.x);
                case ConcreteSlotValueType.Vector4:
                case ConcreteSlotValueType.Vector3:
                case ConcreteSlotValueType.Vector2:
                    {
                        string values = NodeUtils.FloatToShaderValue(shaderValue.value.vectorValue.x);
                        for (var i = 1; i < channelCount; i++)
                            values += ", " + NodeUtils.FloatToShaderValue(shaderValue.value.vectorValue[i]);
                        return string.Format("{0}{1}({2})", precision, channelCount, values);
                    }
                case ConcreteSlotValueType.Boolean:
                    return (shaderValue.value.booleanValue ? 1 : 0).ToString();
                case ConcreteSlotValueType.Texture2D:
                case ConcreteSlotValueType.Texture3D:
                case ConcreteSlotValueType.Texture2DArray:
                case ConcreteSlotValueType.Cubemap:
                case ConcreteSlotValueType.SamplerState:
                    return matOwner.GetVariableNameForSlot(shaderValue.id);
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
                    return string.Format("Unity{0}()", matOwner.GetVariableNameForSlot(shaderValue.id));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal static string ToVariableDefinition(this IShaderValue shaderValue, AbstractMaterialNode.OutputPrecision precision)
        {
            return string.Format("{0} {1}",
                NodeUtils.ConvertConcreteSlotValueTypeToString(precision, shaderValue.concreteValueType),
                shaderValue.ToVariableName());
        }

        internal static string ToVariableReference(this IShaderValue shaderValue, AbstractMaterialNode.OutputPrecision precision, GenerationMode generationMode)
        {
            if (shaderValue is ShaderParameter parameter)
            {
                if (generationMode.IsPreview())
                    return parameter.ToVariableName();

                return parameter.ToVariableValue(precision);
            }

            if (shaderValue is ShaderPort port)
                return port.InputValue(shaderValue.owner.owner, generationMode);

            return string.Empty;
        }

        internal static AbstractMaterialNode ToProperty(this IShaderValue shaderValue)
        {
            Debug.LogError("TODO: HANDLE CONVERISON TO CONCRETE NODE!");
            return null;
        }

        internal static ShaderProperty[] ToDefaultPropertyArray(this IShaderValue shaderValue, string overrideReferenceName = null)
        {
            if(shaderValue.concreteValueType == ConcreteSlotValueType.Gradient)
                return GradientShaderValueToDefaultPropertyArray(shaderValue, overrideReferenceName);

            ShaderProperty property = shaderValue.concreteValueType.ToShaderProperty();

            //property.generatePropertyBlock = false;

            if(overrideReferenceName != null)
                property.overrideReferenceName = overrideReferenceName;
            
            return new ShaderProperty[] { property };
        }

        private static ShaderProperty[] GradientShaderValueToDefaultPropertyArray(IShaderValue shaderValue, string overrideReferenceName)
        {
            List<ShaderProperty> properties = new List<ShaderProperty>();
            // properties.Add(new Vector1ShaderProperty()
            // {
            //     overrideReferenceName = string.Format("{0}_Type", overrideReferenceName),
            //     value = (int)shaderValue.value.gradientValue.mode,
            //     generatePropertyBlock = false
            // });

            // properties.Add(new Vector1ShaderProperty()
            // {
            //     overrideReferenceName = string.Format("{0}_ColorsLength", overrideReferenceName),
            //     value = shaderValue.value.gradientValue.colorKeys.Length,
            //     generatePropertyBlock = false
            // });

            // properties.Add(new Vector1ShaderProperty()
            // {
            //     overrideReferenceName = string.Format("{0}_AlphasLength", overrideReferenceName),
            //     value = shaderValue.value.gradientValue.alphaKeys.Length,
            //     generatePropertyBlock = false
            // });

            // for (int i = 0; i < 8; i++)
            // {
            //     properties.Add(new Vector4ShaderProperty()
            //     {
            //         overrideReferenceName = string.Format("{0}_ColorKey{1}", overrideReferenceName, i),
            //         value = i < shaderValue.value.gradientValue.colorKeys.Length ? GradientUtils.ColorKeyToVector(shaderValue.value.gradientValue.colorKeys[i]) : Vector4.zero,
            //         generatePropertyBlock = false
            //     });
            // }

            // for (int i = 0; i < 8; i++)
            // {
            //     properties.Add(new Vector4ShaderProperty()
            //     {
            //         overrideReferenceName = string.Format("{0}_AlphaKey{1}", overrideReferenceName, i),
            //         value = i < shaderValue.value.gradientValue.alphaKeys.Length ? GradientUtils.AlphaKeyToVector(shaderValue.value.gradientValue.alphaKeys[i]) : Vector2.zero,
            //         generatePropertyBlock = false
            //     });
            // }

            var prop = new ShaderProperty(PropertyType.Gradient);
            if(overrideReferenceName != null)
                prop.overrideReferenceName = overrideReferenceName;
            // prop.generatePropertyBlock = false;
            // prop.value = shaderValue.value.gradientValue;
            prop.OverrideMembers(overrideReferenceName);
            properties.Add(prop);

            return properties.ToArray();
        }

        internal static PreviewProperty ToPreviewProperty(this IShaderValue shaderValue, string name)
        {
            var propType = shaderValue.concreteValueType.ToPropertyType();
            var pp = new PreviewProperty(propType) { name = name };

            switch (propType)
            {
                case PropertyType.Vector4:
                case PropertyType.Vector3:
                case PropertyType.Vector2:
                    pp.vector4Value = shaderValue.value.vectorValue;
                    break;
                case PropertyType.Vector1:
                    pp.floatValue = shaderValue.value.vectorValue.x;
                    break;
                case PropertyType.Boolean:
                    pp.booleanValue = shaderValue.value.booleanValue;
                    break;
                case PropertyType.Texture2D:
                case PropertyType.Texture3D:
                case PropertyType.Texture2DArray:
                    pp.textureValue = shaderValue.value.textureValue;
                    break;
                case PropertyType.Cubemap: // TODO - Remove PreviewProperty.cubemapValue
                    pp.cubemapValue = (Cubemap)shaderValue.value.textureValue;
                    break;
                case PropertyType.SamplerState:
                    break;
                case PropertyType.Matrix2:
                case PropertyType.Matrix3:
                case PropertyType.Matrix4:
                    break;
                case PropertyType.Gradient:
                    pp.gradientValue = shaderValue.value.gradientValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return pp;
        }
    }
}
