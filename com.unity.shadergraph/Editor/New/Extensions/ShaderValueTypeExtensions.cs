using System;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    internal static class ShaderValueTypeExtensions
    {
        internal static IShaderControl ToDefaultControl(this SlotValueType slotValueType)
        {
            switch (slotValueType)
            {
                // TODO: Enable all Controls
                // case SlotValueType.Vector4:
                //     return new Vector4Control();
                // case SlotValueType.Vector3:
                //     return new Vector3Control();
                // case SlotValueType.Vector2:
                //     return new Vector2Control();
                // case SlotValueType.Vector1:
                //     return new Vector1Control();
                // case SlotValueType.Boolean:
                //     return new ToggleControl();
                // case SlotValueType.Texture2D:
                //     return new TextureControl<Texture>();
                // case SlotValueType.Texture3D:
                //     return new TextureControl<Texture3D>();
                // case SlotValueType.Texture2DArray:
                //     return new TextureControl<Texture2DArray>();
                // case SlotValueType.Cubemap:
                //     return new TextureControl<Cubemap>();
                // case SlotValueType.SamplerState:
                //     return new LabelControl("Default");
                // case SlotValueType.Matrix2:
                //     return new LabelControl("Identity");
                // case SlotValueType.Matrix3:
                //     return new LabelControl("Identity");
                // case SlotValueType.Matrix4:
                //     return new LabelControl("Identity");
                // case SlotValueType.Gradient:
                //     return new GradientControl();
                case SlotValueType.DynamicVector:
                case SlotValueType.DynamicMatrix:
                case SlotValueType.Dynamic:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}