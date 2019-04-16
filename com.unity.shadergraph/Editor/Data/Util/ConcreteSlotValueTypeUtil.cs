namespace UnityEditor.ShaderGraph
{
    internal static class ConcreteSlotValueTypeUtil
    {
        internal static string ToShaderString(this ConcreteSlotValueType concreteSlotValueType)
        {
            var precisionToken = "$precision";

            switch (concreteSlotValueType)
            {
                case ConcreteSlotValueType.Boolean:
                case ConcreteSlotValueType.Vector1:
                    return precisionToken;
                case ConcreteSlotValueType.Vector2:
                    return precisionToken + "2";
                case ConcreteSlotValueType.Vector3:
                    return precisionToken + "3";
                case ConcreteSlotValueType.Vector4:
                    return precisionToken + "4";
                case ConcreteSlotValueType.Matrix2:
                    return precisionToken + "2x2";
                case ConcreteSlotValueType.Matrix3:
                    return precisionToken + "3x3";
                case ConcreteSlotValueType.Matrix4:
                    return precisionToken + "4x4";
                case ConcreteSlotValueType.Texture2D:
                    return "Texture2D";
                case ConcreteSlotValueType.Texture2DArray:
                    return "Texture2DArray";
                case ConcreteSlotValueType.Texture3D:
                    return "Texture3D";
                case ConcreteSlotValueType.Cubemap:
                    return "TextureCube";
                case ConcreteSlotValueType.SamplerState:
                    return "SamplerState";
                case ConcreteSlotValueType.Gradient:
                    return "Gradient";
                default:
                    return "Error";
            }
        }
    }
}
