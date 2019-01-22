using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    class ShaderPort : MaterialSlot
    {
        [SerializeField]
        private SlotValueType m_ValueType = SlotValueType.Vector1;

        [SerializeField]
        private Vector4 m_VectorValue = Vector4.zero;

        [SerializeField]
        private SerializedControl m_SerializedControl;

        private IShaderControl m_Control;
        public IShaderControl control
        {
            get
            {
                if (m_Control == null)
                    m_Control = m_SerializedControl.Deserialize();
                return m_Control;
            }
        }

        public ShaderPort()
        {
        }

        public ShaderPort(
            int slotId,
            string displayName,
            SlotType slotType,
            SlotValueType valueType,
            IShaderControl control,
            ShaderStageCapability stageCapability = ShaderStageCapability.All)
            : base(slotId, displayName, displayName, slotType, stageCapability, false)
        {
            m_ValueType = valueType;
            m_Control = control;
            m_SerializedControl = new SerializedControl(control);

            control.UpdateDefaultValue(this);
        }

        public override SlotValueType valueType
        {
            get { return m_ValueType; }
        }

        public Vector4 vectorValue
        {
            get { return m_VectorValue; }
            set { m_VectorValue = value; }
        }

        public override ConcreteSlotValueType concreteValueType
        {
            get
            {
                switch (m_ValueType)
                {
                    case SlotValueType.Vector4:
                        return ConcreteSlotValueType.Vector4;
                    case SlotValueType.Vector3:
                        return ConcreteSlotValueType.Vector3;
                    case SlotValueType.Vector2:
                        return ConcreteSlotValueType.Vector2;
                    case SlotValueType.Vector1:
                        return ConcreteSlotValueType.Vector1;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override VisualElement InstantiateControl()
        {
            return m_Control.GetControl(this);
        }

        public override void GetPreviewProperties(List<PreviewProperty> properties, string name)
        {
            var propType = ConvertConcreteSlotValueTypeToPropertyType(concreteValueType);
            var pp = new PreviewProperty(propType) { name = name };

            switch (propType)
            {
                case PropertyType.Vector4:
                case PropertyType.Vector3:
                case PropertyType.Vector2:
                    pp.vector4Value = new Vector4(vectorValue.x, vectorValue.y, vectorValue.z, vectorValue.w);
                    break;
                case PropertyType.Vector1:
                    pp.floatValue = vectorValue.x;
                    break;
            }

            properties.Add(pp);
        }

        protected override string ConcreteSlotValueAsVariable(AbstractMaterialNode.OutputPrecision precision)
        {
            var channelCount = SlotValueHelper.GetChannelCount(concreteValueType);
            switch (concreteValueType)
            {
                case ConcreteSlotValueType.Vector1:
                    return NodeUtils.FloatToShaderValue(vectorValue.x);
                case ConcreteSlotValueType.Vector4:
                case ConcreteSlotValueType.Vector3:
                case ConcreteSlotValueType.Vector2:
                    {
                        string values = NodeUtils.FloatToShaderValue(vectorValue.x);
                        for (var i = 1; i < channelCount; i++)
                            values += ", " + NodeUtils.FloatToShaderValue(vectorValue[i]);
                        return string.Format("{0}{1}({2})", precision, channelCount, values);
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void AddDefaultProperty(PropertyCollector properties, GenerationMode generationMode)
        {
            if (!generationMode.IsPreview())
                return;

            var matOwner = owner as AbstractMaterialNode;
            if (matOwner == null)
                throw new Exception(string.Format("Slot {0} either has no owner, or the owner is not a {1}", this, typeof(AbstractMaterialNode)));

            IShaderProperty property;
            switch (concreteValueType)
            {
                case ConcreteSlotValueType.Vector4:
                    property = new Vector4ShaderProperty();
                    break;
                case ConcreteSlotValueType.Vector3:
                    property = new Vector3ShaderProperty();
                    break;
                case ConcreteSlotValueType.Vector2:
                    property = new Vector2ShaderProperty();
                    break;
                case ConcreteSlotValueType.Vector1:
                    property = new Vector1ShaderProperty();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            property.overrideReferenceName = matOwner.GetVariableNameForSlot(id);
            property.generatePropertyBlock = false;
            properties.AddShaderProperty(property);
        }

        public override void CopyValuesFrom(MaterialSlot foundSlot)
        {
            var slot = foundSlot as ShaderPort;
            if (slot != null)
            {
                vectorValue = slot.vectorValue;
            }
        }
    }
}
