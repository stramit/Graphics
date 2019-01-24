namespace UnityEditor.ShaderGraph
{
    internal struct NodeTypeDescriptor
    {
        public string name { get; set; }
        public string path { get; set; }
        public InputDescriptor[] inPorts { get; set; }
        public OutputDescriptor[] outPorts { get; set; }
        public InputDescriptor[] parameters {get; set; }
        public bool preview {get; set; }

        // private string m_Name = "New Node";
        // private string m_Path = "";
        // private InputDescriptor[] m_InPorts = new InputDescriptor[0];
        // private OutputDescriptor[] m_OutPorts = new OutputDescriptor[0];
        // private InputDescriptor[] m_Parameters = new InputDescriptor[0];
        // private bool m_Preview;

        // public string name
        // {
        //     get { return m_Name; }
        //     set { m_Name = value; }
        // }

        // public string path
        // {
        //     get { return m_Path; }
        //     set { m_Path = value; }
        // }

        // public InputDescriptor[] inPorts
        // {
        //     get { return m_InPorts; }
        //     set { m_InPorts = value; }
        // }

        // public OutputDescriptor[] outPorts
        // {
        //     get { return m_OutPorts; }
        //     set { m_OutPorts = value; }
        // }

        // public InputDescriptor[] parameters
        // {
        //     get { return m_Parameters; }
        //     set { m_Parameters = value; }
        // }

        // public bool preview
        // {
        //     get { return m_Preview; }
        //     set { m_Preview = value; }
        // }
    }
}
