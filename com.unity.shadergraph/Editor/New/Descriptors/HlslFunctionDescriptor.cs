namespace UnityEditor.ShaderGraph
{
    internal struct HlslFunctionDescriptor
    {
        public string name { get; set; }
        public HlslSource source { get; set; }
        public InputDescriptor[] inArguments { get; set; }
        public OutputDescriptor[] outArguments { get; set; }

        // private string m_Name = "Node";
        // private HlslSource m_Source;
        // private InputDescriptor[] m_InArguments = new InputDescriptor[0];
        // private OutputDescriptor[] m_OutArguments = new OutputDescriptor[0];

        // public string name
        // {
        //     get { return m_Name; }
        //     set { m_Name = value; }
        // }

        // public HlslSource source
        // {
        //     get { return m_Source; }
        //     set { m_Source = value; }
        // }

        // public InputDescriptor[] inArguments
        // {
        //     get { return m_InArguments; }
        //     set { m_InArguments = value; }
        // }

        // public OutputDescriptor[] outArguments
        // {
        //     get { return m_OutArguments; }
        //     set { m_OutArguments = value; }
        // }
    }
}
