using System;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    internal static class ConcreteValueTypeExtensions
    {

#region Properties
        internal static IShaderProperty ToShaderProperty(this ConcreteSlotValueType concreteSlotValueType)
        {
            switch (concreteSlotValueType)
            {
                case ConcreteSlotValueType.Vector4:
                    return new Vector4ShaderProperty();
                case ConcreteSlotValueType.Vector3:
                    return new Vector3ShaderProperty();
                case ConcreteSlotValueType.Vector2:
                    return new Vector2ShaderProperty();
                case ConcreteSlotValueType.Vector1:
                    return new Vector1ShaderProperty();
                case ConcreteSlotValueType.Boolean:
                    return new BooleanShaderProperty();
                case ConcreteSlotValueType.Texture2D:
                    return new TextureShaderProperty();
                case ConcreteSlotValueType.Texture3D:
                    return new Texture3DShaderProperty();
                case ConcreteSlotValueType.Texture2DArray:
                    return new Texture2DArrayShaderProperty();
                case ConcreteSlotValueType.Cubemap:
                    return new CubemapShaderProperty();
                case ConcreteSlotValueType.SamplerState:
                    return new SamplerStateShaderProperty();
                case ConcreteSlotValueType.Matrix2:
                    return new Matrix2ShaderProperty();
                case ConcreteSlotValueType.Matrix3:
                    return new Matrix3ShaderProperty();
                case ConcreteSlotValueType.Matrix4:
                    return new Matrix4ShaderProperty();
                case ConcreteSlotValueType.Gradient:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal static PropertyType ToPropertyType(this ConcreteSlotValueType concreteSlotValueType)
        {
            switch (concreteSlotValueType)
            {
                case ConcreteSlotValueType.Texture2D:
                    return PropertyType.Texture2D;
                case ConcreteSlotValueType.Texture2DArray:
                    return PropertyType.Texture2DArray;
                case ConcreteSlotValueType.Texture3D:
                    return PropertyType.Texture3D;
                case ConcreteSlotValueType.Cubemap:
                    return PropertyType.Cubemap;
                case ConcreteSlotValueType.Gradient:
                    return PropertyType.Gradient;
                case ConcreteSlotValueType.Boolean:
                    return PropertyType.Boolean;
                case ConcreteSlotValueType.Vector1:
                    return PropertyType.Vector1;
                case ConcreteSlotValueType.Vector2:
                    return PropertyType.Vector2;
                case ConcreteSlotValueType.Vector3:
                    return PropertyType.Vector3;
                case ConcreteSlotValueType.Vector4:
                    return PropertyType.Vector4;
                case ConcreteSlotValueType.Matrix2:
                    return PropertyType.Matrix2;
                case ConcreteSlotValueType.Matrix3:
                    return PropertyType.Matrix3;
                case ConcreteSlotValueType.Matrix4:
                    return PropertyType.Matrix4;
                case ConcreteSlotValueType.SamplerState:
                    return PropertyType.SamplerState;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
#endregion
    
    }
}
