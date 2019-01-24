namespace UnityEditor.ShaderGraph
{
    class NodeTypeDescriptor
    {
        private string m_Name = "New Node";
        private InPortDescriptor[] m_InPorts = new InPortDescriptor[0];
        private OutPortDescriptor[] m_OutPorts = new OutPortDescriptor[0];
        private InPortDescriptor[] m_Parameters = new InPortDescriptor[0];
        private bool m_Preview;

        public string name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public InPortDescriptor[] inPorts
        {
            get { return m_InPorts; }
            set { m_InPorts = value; }
        }

        public OutPortDescriptor[] outPorts
        {
            get { return m_OutPorts; }
            set { m_OutPorts = value; }
        }

        public InPortDescriptor[] parameters
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
