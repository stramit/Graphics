using System;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    class OutputDescriptor : IShaderValueDescriptor
    {
        public OutputDescriptor()
        {
        }

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
            get => m_Id;
            set => m_Id = value;
        }
        
        public string name 
        {
            get => m_Name;
            set => m_Name = value;
        }
        
        public ConcreteSlotValueType valueType => m_ValueType;
        public SlotType portType => SlotType.Output;
    }
}
