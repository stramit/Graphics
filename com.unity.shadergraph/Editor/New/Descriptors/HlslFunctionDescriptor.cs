namespace UnityEditor.ShaderGraph
{
    internal struct HlslFunctionDescriptor
    {
        public string name { get; set; }
        public HlslSource source { get; set; }
        public InputDescriptor[] inArguments { get; set; }
        public OutputDescriptor[] outArguments { get; set; }
    }
}
