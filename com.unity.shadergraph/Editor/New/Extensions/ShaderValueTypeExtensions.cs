using System;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    internal static class ShaderValueTypeExtensions
    {

#region Conversions
        internal static ConcreteSlotValueType ToConcreteValueType(this SlotValueType slotValueType)
        {
            switch (slotValueType)
            {
                case SlotValueType.Vector4:
                case SlotValueType.DynamicVector:
                case SlotValueType.Dynamic:
                    return ConcreteSlotValueType.Vector4;
                case SlotValueType.Vector3:
                    return ConcreteSlotValueType.Vector3;
                case SlotValueType.Vector2:
                    return ConcreteSlotValueType.Vector2;
                case SlotValueType.Vector1:
                    return ConcreteSlotValueType.Vector1;
                case SlotValueType.Boolean:
                    return ConcreteSlotValueType.Boolean;
                case SlotValueType.Texture2D:
                    return ConcreteSlotValueType.Texture2D;
                case SlotValueType.Texture3D:
                    return ConcreteSlotValueType.Texture3D;
                case SlotValueType.Texture2DArray:
                    return ConcreteSlotValueType.Texture2DArray;
                case SlotValueType.Cubemap:
                    return ConcreteSlotValueType.Cubemap;
                case SlotValueType.SamplerState:
                    return ConcreteSlotValueType.SamplerState;
                case SlotValueType.Matrix2:
                    return ConcreteSlotValueType.Matrix2;
                case SlotValueType.Matrix3:
                    return ConcreteSlotValueType.Matrix3;
                case SlotValueType.Matrix4:
                case SlotValueType.DynamicMatrix:
                    return ConcreteSlotValueType.Matrix4;
                case SlotValueType.Gradient:
                    return ConcreteSlotValueType.Gradient;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
#endregion

#region Controls
        internal static IShaderControl ToDefaultControl(this SlotValueType slotValueType)
        {
            switch (slotValueType)
            {
                case SlotValueType.Vector4:
                    return new Vector4Control();
                case SlotValueType.Vector3:
                    return new Vector3Control();
                case SlotValueType.Vector2:
                    return new Vector2Control();
                case SlotValueType.Vector1:
                    return new Vector1Control();
                case SlotValueType.Boolean:
                    return new ToggleControl();
                case SlotValueType.Texture2D:
                    return new TextureControl<Texture>();
                case SlotValueType.Texture3D:
                    return new TextureControl<Texture3D>();
                case SlotValueType.Texture2DArray:
                    return new TextureControl<Texture2DArray>();
                case SlotValueType.Cubemap:
                    return new TextureControl<Cubemap>();
                case SlotValueType.SamplerState:
                    return new LabelControl("Default");
                case SlotValueType.Matrix2:
                    return new LabelControl("Identity");
                case SlotValueType.Matrix3:
                    return new LabelControl("Identity");
                case SlotValueType.Matrix4:
                    return new LabelControl("Identity");
                case SlotValueType.Gradient:
                    return new GradientControl();
                case SlotValueType.DynamicVector:
                    return new DynamicVectorControl();
                // TODO: Write Dynamic controls
                case SlotValueType.DynamicMatrix:
                case SlotValueType.Dynamic:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
#endregion

    }
}