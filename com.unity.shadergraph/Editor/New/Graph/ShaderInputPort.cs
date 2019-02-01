using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    class ShaderInputPort : ShaderPort, IShaderInput
    {
        [SerializeField]
        private ShaderValueData m_ValueData;

        [SerializeField]
        private SerializableControl m_SerializableControl = new SerializableControl();

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

        public ShaderInputPort()
        {
        }

        public ShaderInputPort(InputDescriptor portDescriptor) : base(portDescriptor)
        {
            valueData = portDescriptor.valueData;
            control = portDescriptor.control;
        }

#region Controls
        public override VisualElement InstantiateControl()
        {
            var container = new VisualElement { name = "Container" };
            container.Add(control.GetControl(this));
            if(control.portControlWidth != -1)
                container.style.width = control.portControlWidth;
            return container;
        }
#endregion

#region ValueData
        public void UpdateValueData(ShaderValueData valueData)
        {
            if(!this.valueData.Equals(valueData))
            {
                this.valueData = valueData;
                owner.owner.owner.RegisterCompleteObjectUndo("Shader Value Change");
                owner.Dirty(ModificationScope.Node);
            }
        }
#endregion

#region Copy
        public override IShaderValue Copy()
        {
            ShaderInputPort port = base.Copy() as ShaderInputPort;
            port.control = this.control;
            port.valueData = this.valueData;
            return port;
        }

        public override void CopyValuesFrom(MaterialSlot foundSlot)
        {
            var port = foundSlot as ShaderInputPort;
            if (port != null)
            {
                base.CopyValuesFrom(foundSlot);
                this.control = port.control;
                this.valueData = port.valueData;
            }
        }
#endregion

        // ----------------------------------------------------------------------------------------------------
        // LEGACY CODE
        // - Inherited from MaterialSlot
        // - Not used by ShaderNode API


#region Legacy
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

            string overrideReferenceName = this.ToVariableNameSnippet();
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
                return this.ToVariableNameSnippet();

            return this.ToValueSnippet(matOwner.precision);
        }

        protected override string ConcreteSlotValueAsVariable(AbstractMaterialNode.OutputPrecision precision)
        {
            return this.ToValueSnippet(precision);
        }
#endregion

    }
}