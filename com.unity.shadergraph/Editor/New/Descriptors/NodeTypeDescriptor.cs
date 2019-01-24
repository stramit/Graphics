namespace UnityEditor.ShaderGraph
{
    class NodeTypeDescriptor
    {
        private string m_Name = "New Node";
        private InputDescriptor[] m_InPorts = new InputDescriptor[0];
        private OutputDescriptor[] m_OutPorts = new OutputDescriptor[0];
        private InputDescriptor[] m_Parameters = new InputDescriptor[0];
        private bool m_Preview;

        public string name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public InputDescriptor[] inPorts
        {
            get { return m_InPorts; }
            set { m_InPorts = value; }
        }

        public OutputDescriptor[] outPorts
        {
            get { return m_OutPorts; }
            set { m_OutPorts = value; }
        }

        public InputDescriptor[] parameters
        {
            get { return m_Parameters; }
            set { m_Parameters = value; }
        }

        public bool preview
        {
            get { return m_Preview; }
            set { m_Preview = value; }
        }
    }
}
