namespace UnityEditor.ShaderGraph
{
    class NodeDescriptor
    {
        private string m_Name = "New Node";
        private InPortDescriptor[] m_InPorts = new InPortDescriptor[0];
        private OutPortDescriptor[] m_OutPorts = new OutPortDescriptor[0];

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
    }
}
