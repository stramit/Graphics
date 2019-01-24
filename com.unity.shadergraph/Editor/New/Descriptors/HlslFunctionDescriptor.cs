namespace UnityEditor.ShaderGraph
{
    class HlslFunctionDescriptor
    {
        private string m_Name = "Node";
        private string m_Body = "";
        private InPortDescriptor[] m_InArguments = new InPortDescriptor[0];
        private OutPortDescriptor[] m_OutArguments = new OutPortDescriptor[0];

        public string name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public string body
        {
            get { return m_Body; }
            set { m_Body = value; }
        }

        public InPortDescriptor[] inArguments
        {
            get { return m_InArguments; }
            set { m_InArguments = value; }
        }

        public OutPortDescriptor[] outArguments
        {
            get { return m_OutArguments; }
            set { m_OutArguments = value; }
        }
    }
}
