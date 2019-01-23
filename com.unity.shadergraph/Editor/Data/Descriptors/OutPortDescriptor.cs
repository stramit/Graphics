using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    class OutPortDescriptor : IPortDescriptor
    {
        [SerializeField]
        private int m_Id;

        [SerializeField]
        private string m_Name;

        [SerializeField]
        private SlotValueType m_ValueType;

        public int id
        {
            get { return m_Id; }
        }

        public string name
        {
            get { return m_Name; }
        }

        public SlotValueType valueType
        {
            get { return m_ValueType; }
        }

        public SlotType portType
        {
            get { return SlotType.Output; }
        }

        public OutPortDescriptor(int id, string name, SlotValueType valueType)
        {
            m_Id = id;
            m_Name = name;
            m_ValueType = valueType;
        }
    }
}
