using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    class ShaderParameter : IShaderValue
    {
        public ShaderParameter(InputDescriptor portDescriptor)
        {
            m_Id = portDescriptor.id;
            m_DisplayName = portDescriptor.name;
            m_ShaderOutputName = NodeUtils.GetHLSLSafeName(portDescriptor.name);
            m_ConcreteSlotValueType = portDescriptor.valueType;
            m_ShaderValueData = portDescriptor.defaultValue;
            control = portDescriptor.control;
        }

        [SerializeField]
        int m_Id;

        [SerializeField]
        private SerializableGuid m_Guid = new SerializableGuid();

        [SerializeField]
        string m_DisplayName = "Not Initilaized";

        [SerializeField]
        string m_ShaderOutputName;

        [SerializeField]
        private ConcreteSlotValueType m_ConcreteSlotValueType = ConcreteSlotValueType.Vector1;

        [SerializeField]
        private ShaderValueData m_ShaderValueData;

        [SerializeField]
        private SerializableControl m_SerializableControl = new SerializableControl();

        public INode owner { get; set; }

        public int id => m_Id;
        public Guid guid => m_Guid.guid;
        public string shaderOutputName => m_ShaderOutputName;
        public ConcreteSlotValueType concreteValueType => m_ConcreteSlotValueType;
        public ShaderValueData value => m_ShaderValueData;

        public string displayName
        {
            get => m_DisplayName;
            set => m_DisplayName = value;
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

        public void UpdateValue(ShaderValueData value)
        {
            if(!m_ShaderValueData.Equals(value))
            {
                m_ShaderValueData = value;
                owner.owner.owner.RegisterCompleteObjectUndo("Shader Value Change");
                owner.Dirty(ModificationScope.Node);
            }
        }

        public void CopyValuesFrom(ShaderParameter parameter)
        {
            if (parameter != null)
            {
                m_ConcreteSlotValueType = parameter.concreteValueType;
                m_ShaderValueData = parameter.value;
                control = parameter.control;
            }
        }

        public INode ToConcreteNode()
        {
            return null; // TODO - Fill this out
        }
    }
}
