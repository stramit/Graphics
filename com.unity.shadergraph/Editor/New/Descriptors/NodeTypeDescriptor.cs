namespace UnityEditor.ShaderGraph
{
    struct NodeTypeDescriptor
    {
        public string name { get; set; }
        public string path { get; set; }
        public InputDescriptor[] inPorts { get; set; }
        public OutputDescriptor[] outPorts { get; set; }
        public InputDescriptor[] parameters {get; set; }
        public bool preview {get; set; }
    }
}