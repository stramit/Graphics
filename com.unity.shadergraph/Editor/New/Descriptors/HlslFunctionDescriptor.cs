namespace UnityEditor.ShaderGraph
{
    class HlslFunctionDescriptor
    {
        private string m_Name = "Node";
        private string m_Body = "";
        private InputDescriptor[] m_InArguments = new InputDescriptor[0];
        private OutputDescriptor[] m_OutArguments = new OutputDescriptor[0];

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

        public InputDescriptor[] inArguments
        {
            get { return m_InArguments; }
            set { m_InArguments = value; }
        }

        public OutputDescriptor[] outArguments
        {
            get { return m_OutArguments; }
            set { m_OutArguments = value; }
        }
    }
}
