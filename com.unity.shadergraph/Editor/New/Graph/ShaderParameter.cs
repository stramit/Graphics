using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    class ShaderParameter : IShaderInput
    {
        [SerializeField]
        SerializableGuid m_Guid;

        [SerializeField]
        int m_Id;

        [SerializeField]
        SlotValueType m_ValueType;       

        [SerializeField]
        string m_DisplayName = "Not Initilaized";

        [SerializeField]
        string m_ShaderOutputName;

        [SerializeField]
        private ShaderValueData m_ValueData;

        [SerializeField]
        private SerializableControl m_SerializableControl = new SerializableControl();

        public INode owner { get; set; }
        public int id => m_Id;
        public SerializableGuid guid => m_Guid;
        public SlotValueType valueType => m_ValueType;
        public ConcreteSlotValueType concreteValueType => m_ValueType.ToConcreteValueType();
        public string displayName => m_DisplayName;
        public string outputName => m_ShaderOutputName; 

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

        internal ShaderParameter()
        {
        }

        internal ShaderParameter(InputDescriptor descriptor)
        {
            m_Guid = descriptor.guid;
            m_Id = descriptor.id;
            m_ValueType = descriptor.valueType;
            m_DisplayName = descriptor.name;
            m_ShaderOutputName = NodeUtils.GetHLSLSafeName(descriptor.name);
            m_ValueData = descriptor.valueData;
            control = descriptor.control;
        }

#region ValueData

        public void UpdateValueData(ShaderValueData value)
        {
            if(!this.valueData.Equals(value))
            {
                this.valueData = value;
                owner.owner.owner.RegisterCompleteObjectUndo("Shader Value Change");
                owner.Dirty(ModificationScope.Node);
            }
        }
#endregion

#region Copy
        public IShaderValue Copy()
        {
            return new ShaderParameter()
            {
                m_Guid = this.guid,
                m_Id = this.id,
                m_ValueType = this.valueType,
                m_DisplayName = this.displayName,
                m_ShaderOutputName = this.outputName,
                m_ValueData = this.valueData,
                control = this.control
            };
        }

        public void CopyValuesFrom(ShaderParameter parameter)
        {
            if (parameter != null)
            {
                m_Guid = parameter.guid;
                m_Id = parameter.id;
                m_ValueData = parameter.valueData;
            }
        }
#endregion

    }
}