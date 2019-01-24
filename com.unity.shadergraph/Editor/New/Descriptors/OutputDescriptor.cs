using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    class OutputDescriptor
    {
        public OutputDescriptor(int id, string name, ConcreteSlotValueType valueType)
        {
            m_Id = id;
            m_Name = name;
            m_ValueType = valueType;
        }
        
        [SerializeField]
        private int m_Id;

        [SerializeField]
        private string m_Name;

        [SerializeField]
        private ConcreteSlotValueType m_ValueType;

        public int id
        {
            get { return m_Id; }
        }

        public string name
        {
            get { return m_Name; }
        }

        public ConcreteSlotValueType valueType
        {
            get { return m_ValueType; }
        }

        public SlotType portType
        {
            get { return SlotType.Output; }
        }
    }
}
