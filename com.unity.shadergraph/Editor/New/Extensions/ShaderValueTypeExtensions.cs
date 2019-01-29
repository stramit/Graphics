using System;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    internal static class ShaderValueTypeExtensions
    {
        internal static ShaderProperty ToShaderProperty(this ConcreteSlotValueType concreteSlotValueType)
        {
            switch (concreteSlotValueType)
            {
                case ConcreteSlotValueType.Vector4:
                    return new ShaderProperty(PropertyType.Vector4);// Vector4ShaderProperty();
                case ConcreteSlotValueType.Vector3:
                    return new ShaderProperty(PropertyType.Vector3);//return new Vector3ShaderProperty();
                case ConcreteSlotValueType.Vector2:
                    return new ShaderProperty(PropertyType.Vector2);//return new Vector2ShaderProperty();
                case ConcreteSlotValueType.Vector1:
                    return new ShaderProperty(PropertyType.Vector1);//return new Vector1ShaderProperty();
                case ConcreteSlotValueType.Boolean:
                    return new ShaderProperty(PropertyType.Boolean);//return new BooleanShaderProperty();
                case ConcreteSlotValueType.Texture2D:
                    return new ShaderProperty(PropertyType.Texture2D);//return new TextureShaderProperty();
                case ConcreteSlotValueType.Texture3D:
                    return new ShaderProperty(PropertyType.Texture3D);//return new Texture3DShaderProperty();
                case ConcreteSlotValueType.Texture2DArray:
                    return new ShaderProperty(PropertyType.Texture2DArray);//return new Texture2DArrayShaderProperty();
                case ConcreteSlotValueType.Cubemap:
                    return new ShaderProperty(PropertyType.Cubemap);//return new CubemapShaderProperty();
                case ConcreteSlotValueType.SamplerState:
                    return new ShaderProperty(PropertyType.SamplerState);//return new SamplerStateShaderProperty();
                case ConcreteSlotValueType.Matrix2:
                    return new ShaderProperty(PropertyType.Matrix2);//return new Matrix2ShaderProperty();
                case ConcreteSlotValueType.Matrix3:
                    return new ShaderProperty(PropertyType.Matrix3);//return new Matrix3ShaderProperty();
                case ConcreteSlotValueType.Matrix4:
                    return new ShaderProperty(PropertyType.Matrix4);//return new Matrix4ShaderProperty();
                case ConcreteSlotValueType.Gradient:
                    return new ShaderProperty(PropertyType.Gradient);//return null;
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

        internal static SlotValueType ToSlotValueType(this ConcreteSlotValueType concreteSlotValueType)
        {
            switch (concreteSlotValueType)
            {
                case ConcreteSlotValueType.Vector4:
                    return SlotValueType.Vector4;
                case ConcreteSlotValueType.Vector3:
                    return SlotValueType.Vector3;
                case ConcreteSlotValueType.Vector2:
                    return SlotValueType.Vector2;
                case ConcreteSlotValueType.Vector1:
                    return SlotValueType.Vector1;
                case ConcreteSlotValueType.Boolean:
                    return SlotValueType.Boolean;
                case ConcreteSlotValueType.Texture2D:
                    return SlotValueType.Texture2D;
                case ConcreteSlotValueType.Texture3D:
                    return SlotValueType.Texture3D;
                case ConcreteSlotValueType.Texture2DArray:
                    return SlotValueType.Texture2DArray;
                case ConcreteSlotValueType.Cubemap:
                    return SlotValueType.Cubemap;
                case ConcreteSlotValueType.SamplerState:
                    return SlotValueType.SamplerState;
                case ConcreteSlotValueType.Matrix2:
                    return SlotValueType.Matrix2;
                case ConcreteSlotValueType.Matrix3:
                    return SlotValueType.Matrix3;
                case ConcreteSlotValueType.Matrix4:
                    return SlotValueType.Matrix4;
                case ConcreteSlotValueType.Gradient:
                    return SlotValueType.Gradient;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal static IShaderControl ToDefaultControl(this ConcreteSlotValueType concreteSlotValueType)
        {
            switch (concreteSlotValueType)
            {
                case ConcreteSlotValueType.Vector4:
                    return new Vector4Control();
                case ConcreteSlotValueType.Vector3:
                    return new Vector3Control();
                case ConcreteSlotValueType.Vector2:
                    return new Vector2Control();
                case ConcreteSlotValueType.Vector1:
                    return new Vector1Control();
                case ConcreteSlotValueType.Boolean:
                    return new ToggleControl();
                case ConcreteSlotValueType.Texture2D:
                    return new TextureControl<Texture>();
                case ConcreteSlotValueType.Texture3D:
                    return new TextureControl<Texture3D>();
                case ConcreteSlotValueType.Texture2DArray:
                    return new TextureControl<Texture2DArray>();
                case ConcreteSlotValueType.Cubemap:
                    return new TextureControl<Cubemap>();
                case ConcreteSlotValueType.SamplerState:
                    return new LabelControl("Default");
                case ConcreteSlotValueType.Matrix2:
                    return new LabelControl("Identity");
                case ConcreteSlotValueType.Matrix3:
                    return new LabelControl("Identity");
                case ConcreteSlotValueType.Matrix4:
                    return new LabelControl("Identity");
                case ConcreteSlotValueType.Gradient:
                    return new GradientControl();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
