using System;
using System.Linq;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    class InputDescriptor : IShaderValueDescriptor
    {
        public InputDescriptor()
        {
        }

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
            get => m_Id;
            set => m_Id = value;
        }
        
        public string name 
        {
            get => m_Name;
            set => m_Name = value;
        }

        public ConcreteSlotValueType valueType => m_ValueType;
        public SlotType portType => SlotType.Input;
        public ShaderValueData defaultValue => m_DefaultValue;

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
    }
}
