using System;
using System.Linq;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    class InputDescriptor : IShaderValueDescriptor
    {
        [SerializeField]
        SerializableGuid m_Guid;

        [SerializeField]
        int m_Id;

        [SerializeField]
        SlotValueType m_ValueType;       

        [SerializeField]
        string m_Name = "Not Initilaized";

        [SerializeField]
        private ShaderValueData m_ValueData;

        [SerializeField]
        private SerializableControl m_SerializableControl = new SerializableControl();

        public SerializableGuid guid => m_Guid;
        public SlotType portType => SlotType.Input;
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

        public ShaderValueData valueData
        {
            get => m_ValueData;
            set => m_ValueData = value;
        }

        private IShaderControl m_Control;
        public IShaderControl control
        {
            get
            {
                if (m_Control == null)
                    m_Control = m_SerializableControl.control;
                return m_Control;
            }
            set
            {
                m_Control = value;
                m_SerializableControl.control = value;
            }
        }

        public InputDescriptor(int id, string name, SlotValueType valueType)
        {
            m_Id = id;
            m_Name = name;
            m_ValueType = valueType;
            this.control = valueType.ToDefaultControl();
            this.valueData = new ShaderValueData();
        }

        public InputDescriptor(int id, string name, SlotValueType valueType, IShaderControl control)
        {
            m_Id = id;
            m_Name = name;
            m_ValueType = valueType;
            
            if(!control.validPortTypes.Contains(valueType))
            {
                Debug.LogWarning(string.Format("InputDescriptor {0} tried to define an incompatible Control. Will use default Control instead.", name));
                control = valueType.ToDefaultControl();
            }
            this.control = control;
            this.valueData = control.defaultValueData;
        }
    }
}
