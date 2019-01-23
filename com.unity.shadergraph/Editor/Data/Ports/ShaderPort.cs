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
        private bool m_BooleanValue = false;

        [SerializeField]
        private SerializedGradient m_GradientValue = new SerializedGradient();

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

        public bool booleanValue
        {
            get { return m_BooleanValue; }
            set { m_BooleanValue = value; }
        }

        public Gradient gradientValue
        {
            get { return m_GradientValue.gradient; }
            set { m_GradientValue.gradient = value; }
        }

        public void SetGradientValue(Gradient g)
        {
            m_GradientValue.Serialize(g);
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
                    case SlotValueType.Boolean:
                        return ConcreteSlotValueType.Boolean;
                    case SlotValueType.Gradient:
                        return ConcreteSlotValueType.Gradient;
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
                    pp.vector4Value = vectorValue;
                    break;
                case PropertyType.Vector1:
                    pp.floatValue = vectorValue.x;
                    break;
                case PropertyType.Boolean:
                    pp.booleanValue = booleanValue;
                    break;
                case PropertyType.Gradient:
                    pp.gradientValue = gradientValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            properties.Add(pp);
        }

        protected override string ConcreteSlotValueAsVariable(AbstractMaterialNode.OutputPrecision precision)
        {
            var matOwner = owner as AbstractMaterialNode;
            if (matOwner == null)
                throw new Exception(string.Format("Slot {0} either has no owner, or the owner is not a {1}", this, typeof(AbstractMaterialNode)));

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
                case ConcreteSlotValueType.Boolean:
                        return (booleanValue ? 1 : 0).ToString();
                case ConcreteSlotValueType.Gradient:
                        return string.Format("Unity{0}()", matOwner.GetVariableNameForSlot(id));
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
                case ConcreteSlotValueType.Boolean:
                    property = new BooleanShaderProperty();
                    break;
                case ConcreteSlotValueType.Gradient:
                    AddGradientProperties(matOwner, properties, generationMode);
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            property.overrideReferenceName = matOwner.GetVariableNameForSlot(id);
            property.generatePropertyBlock = false;
            properties.AddShaderProperty(property);
        }

        private void AddGradientProperties(AbstractMaterialNode matOwner, PropertyCollector properties, GenerationMode generationMode)
        {
            if (generationMode == GenerationMode.Preview)
            {
                properties.AddShaderProperty(new Vector1ShaderProperty()
                {
                    overrideReferenceName = string.Format("{0}_Type", matOwner.GetVariableNameForSlot(id)),
                    value = (int)gradientValue.mode,
                    generatePropertyBlock = false
                });

                properties.AddShaderProperty(new Vector1ShaderProperty()
                {
                    overrideReferenceName = string.Format("{0}_ColorsLength", matOwner.GetVariableNameForSlot(id)),
                    value = gradientValue.colorKeys.Length,
                    generatePropertyBlock = false
                });

                properties.AddShaderProperty(new Vector1ShaderProperty()
                {
                    overrideReferenceName = string.Format("{0}_AlphasLength", matOwner.GetVariableNameForSlot(id)),
                    value = gradientValue.alphaKeys.Length,
                    generatePropertyBlock = false
                });

                for (int i = 0; i < 8; i++)
                {
                    properties.AddShaderProperty(new Vector4ShaderProperty()
                    {
                        overrideReferenceName = string.Format("{0}_ColorKey{1}", matOwner.GetVariableNameForSlot(id), i),
                        value = i < gradientValue.colorKeys.Length ? GradientUtils.ColorKeyToVector(gradientValue.colorKeys[i]) : Vector4.zero,
                        generatePropertyBlock = false
                    });
                }

                for (int i = 0; i < 8; i++)
                {
                    properties.AddShaderProperty(new Vector4ShaderProperty()
                    {
                        overrideReferenceName = string.Format("{0}_AlphaKey{1}", matOwner.GetVariableNameForSlot(id), i),
                        value = i < gradientValue.alphaKeys.Length ? GradientUtils.AlphaKeyToVector(gradientValue.alphaKeys[i]) : Vector2.zero,
                        generatePropertyBlock = false
                    });
                }
            }

            var prop = new GradientShaderProperty();
            prop.overrideReferenceName = matOwner.GetVariableNameForSlot(id);
            prop.generatePropertyBlock = false;
            prop.value = gradientValue;

            if (generationMode == GenerationMode.Preview)
                prop.OverrideMembers(matOwner.GetVariableNameForSlot(id));

            properties.AddShaderProperty(prop);
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
