using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    internal static class PortUtil
    {
        internal static IShaderProperty[] GetDefaultPropertiesFromShaderValue(IShaderValue shaderValue, string overrideReferenceName = null)
        {
            IShaderProperty property;
            switch (shaderValue.concreteValueType)
            {
                case ConcreteSlotValueType.Vector4:
                    property = new Vector4ShaderProperty();
                    break;
                case ConcreteSlotValueType.Vector3:
                    property = new Vector3ShaderProperty();
                    break;
                case ConcreteSlotValueType.Vector2:
                    property = new Vector2ShaderProperty();
                    break;
                case ConcreteSlotValueType.Vector1:
                    property = new Vector1ShaderProperty();
                    break;
                case ConcreteSlotValueType.Boolean:
                    property = new BooleanShaderProperty();
                    break;
                case ConcreteSlotValueType.Texture2D:
                    property = new TextureShaderProperty();
                    break;
                case ConcreteSlotValueType.Texture3D:
                    property = new Texture3DShaderProperty();
                    break;
                case ConcreteSlotValueType.Texture2DArray:
                    property = new Texture2DArrayShaderProperty();
                    break;
                case ConcreteSlotValueType.Cubemap:
                    property = new CubemapShaderProperty();
                    break;
                case ConcreteSlotValueType.SamplerState:
                    property = new SamplerStateShaderProperty();
                    break;
                case ConcreteSlotValueType.Matrix2:
                    property = new Matrix2ShaderProperty();
                    break;
                case ConcreteSlotValueType.Matrix3:
                    property = new Matrix3ShaderProperty();
                    break;
                case ConcreteSlotValueType.Matrix4:
                    property = new Matrix4ShaderProperty();
                    break;
                case ConcreteSlotValueType.Gradient:
                    return GetDefaultGradientPropertiesFromShaderValue(shaderValue, overrideReferenceName);
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if(overrideReferenceName != null)
                property.overrideReferenceName = overrideReferenceName;
            property.generatePropertyBlock = false;
            return new IShaderProperty[] { property };
        }

        private static IShaderProperty[] GetDefaultGradientPropertiesFromShaderValue(IShaderValue shaderValue, string overrideReferenceName)
        {
            List<IShaderProperty> properties = new List<IShaderProperty>();
            properties.Add(new Vector1ShaderProperty()
            {
                overrideReferenceName = string.Format("{0}_Type", overrideReferenceName),
                value = (int)shaderValue.value.gradientValue.mode,
                generatePropertyBlock = false
            });

            properties.Add(new Vector1ShaderProperty()
            {
                overrideReferenceName = string.Format("{0}_ColorsLength", overrideReferenceName),
                value = shaderValue.value.gradientValue.colorKeys.Length,
                generatePropertyBlock = false
            });

            properties.Add(new Vector1ShaderProperty()
            {
                overrideReferenceName = string.Format("{0}_AlphasLength", overrideReferenceName),
                value = shaderValue.value.gradientValue.alphaKeys.Length,
                generatePropertyBlock = false
            });

            for (int i = 0; i < 8; i++)
            {
                properties.Add(new Vector4ShaderProperty()
                {
                    overrideReferenceName = string.Format("{0}_ColorKey{1}", overrideReferenceName, i),
                    value = i < shaderValue.value.gradientValue.colorKeys.Length ? GradientUtils.ColorKeyToVector(shaderValue.value.gradientValue.colorKeys[i]) : Vector4.zero,
                    generatePropertyBlock = false
                });
            }

            for (int i = 0; i < 8; i++)
            {
                properties.Add(new Vector4ShaderProperty()
                {
                    overrideReferenceName = string.Format("{0}_AlphaKey{1}", overrideReferenceName, i),
                    value = i < shaderValue.value.gradientValue.alphaKeys.Length ? GradientUtils.AlphaKeyToVector(shaderValue.value.gradientValue.alphaKeys[i]) : Vector2.zero,
                    generatePropertyBlock = false
                });
            }

            var prop = new GradientShaderProperty();
            if(overrideReferenceName != null)
                prop.overrideReferenceName = overrideReferenceName;
            prop.generatePropertyBlock = false;
            prop.value = shaderValue.value.gradientValue;
            prop.OverrideMembers(overrideReferenceName);
            properties.Add(prop);

            return properties.ToArray();
        }
    }
}
