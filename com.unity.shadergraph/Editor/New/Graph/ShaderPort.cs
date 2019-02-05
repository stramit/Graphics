using System;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    class ShaderPort : MaterialSlot, IShaderValue
    {
        [SerializeField]
        SerializableGuid m_Guid;

        [SerializeField]
        SlotValueType m_ValueType;        

        private ConcreteSlotValueType m_ConcreteValueType;

        public SerializableGuid guid => m_Guid;

        public override SlotValueType valueType => m_ValueType;
        public override ConcreteSlotValueType concreteValueType => m_ConcreteValueType;

        public string outputName => shaderOutputName;  

        public ShaderPort()
        {
        }

        public ShaderPort(IShaderValueDescriptor portDescriptor)
            : base(portDescriptor.id, portDescriptor.name, portDescriptor.name, portDescriptor.portType, ShaderStageCapability.All, false)
        {
            m_Guid = portDescriptor.guid;
            m_ValueType = portDescriptor.valueType;
            m_ConcreteValueType = m_ValueType.ToConcreteValueType();
        }

        public void SetConcreteType(ConcreteSlotValueType concreteValueType)
        {
            m_ConcreteValueType = concreteValueType;
        }

#region Copy
        public virtual IShaderValue Copy()
        {
            return new ShaderPort()
            {
                m_Guid = this.guid,
                m_ValueType = this.valueType,
                displayName = this.displayName,
                shaderOutputName = this.outputName
            };
        }

        public override void CopyValuesFrom(MaterialSlot foundSlot)
        {
            var port = foundSlot as ShaderPort;
            if (port != null)
            {
                m_Guid = port.guid;
            }
        }
#endregion

        // ----------------------------------------------------------------------------------------------------
        // LEGACY CODE
        // - Inherited from MaterialSlot
        // - Not used by ShaderNode API

#region Legacy
        // TODO: Should only be used for ShaderInputPort but required by MaterialSlot
        public override void AddDefaultProperty(PropertyCollector properties, GenerationMode generationMode)
        {
        }
#endregion

    }
}