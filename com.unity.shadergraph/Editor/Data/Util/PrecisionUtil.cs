namespace UnityEditor.ShaderGraph
{
    internal static class PrecisionUtil
    {
        internal static string ToShaderString(this Precision precision)
        {
            switch(precision)
            {
                case Precision.Real:
                    return "real";
                case Precision.Float:
                    return "float";
                case Precision.Half:
                    return "half";
                default:
                    return "float";
            }
        }
    }
}
