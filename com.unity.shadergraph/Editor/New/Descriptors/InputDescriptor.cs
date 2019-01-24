using System;
using System.Linq;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    class InputDescriptor
    {
        public InputDescriptor(int id, string name, ConcreteSlotValueType valueType)
        {
            m_Id = id;
            m_Name = name;
            m_ValueType = valueType;
            
            this.control = valueType.ToDefaultControl();
            m_DefaultValue = new ShaderValueData();
        }

        public InputDescriptor(int id, string name, ConcreteSlotValueType valueType, IShaderControl control)
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
            m_DefaultValue = control.defaultValueData != null ? control.defaultValueData : new ShaderValueData();
        }
        
        [SerializeField]
        private int m_Id;

        [SerializeField]
        private string m_Name;

        [SerializeField]
        private ConcreteSlotValueType m_ValueType;
        
        [SerializeField]
        private ShaderValueData m_DefaultValue;

        [SerializeField]
        private SerializableControl m_SerializableControl = new SerializableControl();

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

        public ShaderValueData defaultValue
        {
            get { return m_DefaultValue; }
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

        public SlotType portType
        {
            get { return SlotType.Input; }
        }
    }
}
