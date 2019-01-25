using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    class OutputDescriptor : IShaderValueDescriptor
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

        public int id  => m_Id;
        public string name => m_Name;
        public ConcreteSlotValueType valueType => m_ValueType;
        public SlotType portType => SlotType.Output;
    }
}
