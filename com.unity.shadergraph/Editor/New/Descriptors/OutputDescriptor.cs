using System;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    struct OutputDescriptor : IShaderValueDescriptor
    {
        [SerializeField]
        SerializableGuid m_Guid;

        [SerializeField]
        int m_Id;

        [SerializeField]
        SlotValueType m_ValueType;       

        [SerializeField]
        string m_Name;

        public SerializableGuid guid => m_Guid;
        public SlotType portType => SlotType.Output;
        public SlotValueType valueType => m_ValueType;

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

        public OutputDescriptor(int id, string name, SlotValueType valueType)
        {
            m_Guid = new SerializableGuid();
            m_Id = id;
            m_Name = name;
            m_ValueType = valueType;
        }
    }
}
