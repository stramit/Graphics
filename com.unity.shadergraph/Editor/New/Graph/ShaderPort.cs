using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    class ShaderPort : MaterialSlot, IShaderValue
    {
        public ShaderPort()
        {
        }

        public ShaderPort(OutputDescriptor portDescriptor)
            : base(portDescriptor.id, portDescriptor.name, portDescriptor.name, portDescriptor.portType, ShaderStageCapability.All, false)
        {
            m_ConcreteSlotValueType = portDescriptor.valueType;
        }

        public ShaderPort(InputDescriptor portDescriptor)
            : base(portDescriptor.id, portDescriptor.name, portDescriptor.name, portDescriptor.portType, ShaderStageCapability.All, false)
        {
            m_ConcreteSlotValueType = portDescriptor.valueType;
            m_ShaderValueData = portDescriptor.defaultValue;
            control = portDescriptor.control;
        }

        [SerializeField]
        private ConcreteSlotValueType m_ConcreteSlotValueType = ConcreteSlotValueType.Vector1;

        [SerializeField]
        private ShaderValueData m_ShaderValueData;

        [SerializeField]
        private SerializableControl m_SerializableControl = new SerializableControl();

        public override ConcreteSlotValueType concreteValueType => m_ConcreteSlotValueType;
        public ShaderValueData value => m_ShaderValueData;

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

        public override VisualElement InstantiateControl()
        {
            var container = new VisualElement { name = "Container" };
            container.Add(control.GetControl(this));
            if(control.portControlWidth != -1)
                container.style.width = control.portControlWidth;
            return container;
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

        public override void CopyValuesFrom(MaterialSlot foundSlot)
        {
            var port = foundSlot as ShaderPort;
            if (port != null)
            {
                m_ConcreteSlotValueType = port.concreteValueType;
                m_ShaderValueData = port.value;
                control = port.control;
            }
        }

        // ----------------------------------------------------------------------------------------------------
        // LEGACY CODE
        // - Inherited from MaterialSlot
        // - Not used by ShaderNode API

        public override SlotValueType valueType
        {
            get { return concreteValueType.ToSlotValueType(); }
        }

        public override void GetPreviewProperties(List<PreviewProperty> properties, string name)
        {
            PreviewProperty pp = this.ToPreviewProperty(name);
            properties.Add(pp);
        }

        public override void AddDefaultProperty(PropertyCollector properties, GenerationMode generationMode)
        {
            if (!generationMode.IsPreview())
                return;

            var matOwner = owner as AbstractMaterialNode;
            if (matOwner == null)
                throw new Exception(string.Format("Slot {0} either has no owner, or the owner is not a {1}", this, typeof(AbstractMaterialNode)));

            string overrideReferenceName = this.ToVariableName();
            IShaderProperty[] defaultProperties = this.ToDefaultPropertyArray(overrideReferenceName);

            foreach(IShaderProperty property in defaultProperties)
                properties.AddShaderProperty(property);
        }

        public override string GetDefaultValue(GenerationMode generationMode)
        {
            var matOwner = owner as AbstractMaterialNode;
            if (matOwner == null)
                throw new Exception(string.Format("Slot {0} either has no owner, or the owner is not a {1}", this, typeof(AbstractMaterialNode)));

            if (generationMode.IsPreview())
                return this.ToVariableName();

            return this.ToVariableValue(matOwner.precision);
        }

        protected override string ConcreteSlotValueAsVariable(AbstractMaterialNode.OutputPrecision precision)
        {
            return this.ToVariableValue(precision);
        }
    }
}
