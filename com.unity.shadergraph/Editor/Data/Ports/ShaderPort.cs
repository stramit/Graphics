using System;
using System.Collections.Generic;
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

        public ShaderPort(OutPortDescriptor portDescriptor)
            : base(portDescriptor.id, portDescriptor.name, portDescriptor.name, portDescriptor.portType, ShaderStageCapability.All, false)
        {
            m_ConcreteValueType = portDescriptor.valueType;
        }

        public ShaderPort(InPortDescriptor portDescriptor)
            : base(portDescriptor.id, portDescriptor.name, portDescriptor.name, portDescriptor.portType, ShaderStageCapability.All, false)
        {
            m_ConcreteValueType = portDescriptor.valueType;
            m_ShaderValue = portDescriptor.defaultValue;
            control = portDescriptor.control;
        }

        [SerializeField]
        private ConcreteSlotValueType m_ConcreteValueType = ConcreteSlotValueType.Vector1;

        public override ConcreteSlotValueType concreteValueType
        {
            get { return m_ConcreteValueType; }
        }

        [SerializeField]
        private SerializableValueStore m_ShaderValue;

        public SerializableValueStore value
        {
            get { return m_ShaderValue; }
        }

        public void UpdateValue(SerializableValueStore value)
        {
            if(!m_ShaderValue.Equals(value))
            {
                m_ShaderValue = value;
                owner.owner.owner.RegisterCompleteObjectUndo("Shader Value Change");
                owner.Dirty(ModificationScope.Node);
            }
        }

        [SerializeField]
        private SerializableControl m_SerializedControl;

        private IShaderControl m_Control;
        public IShaderControl control
        {
            get
            {
                if (m_Control == null)
                    m_Control = m_SerializedControl.Deserialize();
                return m_Control;
            }
            set
            {
                m_Control = value;
                m_SerializedControl = new SerializableControl(value);
            }
        }

        public override SlotValueType valueType
        {
            get
            {
                switch (m_ConcreteValueType)
                {
                    case ConcreteSlotValueType.Vector4:
                        return SlotValueType.Vector4;
                    case ConcreteSlotValueType.Vector3:
                        return SlotValueType.Vector3;
                    case ConcreteSlotValueType.Vector2:
                        return SlotValueType.Vector2;
                    case ConcreteSlotValueType.Vector1:
                        return SlotValueType.Vector1;
                    case ConcreteSlotValueType.Boolean:
                        return SlotValueType.Boolean;
                    case ConcreteSlotValueType.Texture2D:
                        return SlotValueType.Texture2D;
                    case ConcreteSlotValueType.Texture3D:
                        return SlotValueType.Texture3D;
                    case ConcreteSlotValueType.Texture2DArray:
                        return SlotValueType.Texture2DArray;
                    case ConcreteSlotValueType.Cubemap:
                        return SlotValueType.Cubemap;
                    case ConcreteSlotValueType.SamplerState:
                        return SlotValueType.SamplerState;
                    case ConcreteSlotValueType.Matrix2:
                        return SlotValueType.Matrix2;
                    case ConcreteSlotValueType.Matrix3:
                        return SlotValueType.Matrix3;
                    case ConcreteSlotValueType.Matrix4:
                        return SlotValueType.Matrix4;
                    case ConcreteSlotValueType.Gradient:
                        return SlotValueType.Gradient;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override VisualElement InstantiateControl()
        {
            return control.GetControl(this);
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
                    pp.vector4Value = value.vectorValue;
                    break;
                case PropertyType.Vector1:
                    pp.floatValue = value.vectorValue.x;
                    break;
                case PropertyType.Boolean:
                    pp.booleanValue = value.booleanValue;
                    break;
                case PropertyType.Texture2D:
                case PropertyType.Texture3D:
                case PropertyType.Texture2DArray:
                    pp.textureValue = value.textureValue;
                    break;
                case PropertyType.Cubemap: // TODO - Remove PreviewProperty.cubemapValue
                    pp.cubemapValue = (Cubemap)value.textureValue;
                    break;
                case PropertyType.SamplerState:
                    return;
                case PropertyType.Matrix2:
                case PropertyType.Matrix3:
                case PropertyType.Matrix4:
                    return;
                case PropertyType.Gradient:
                    pp.gradientValue = value.gradientValue;
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
            Matrix4x4 matrix = m_ShaderValue.matrixValue;
            switch (concreteValueType)
            {
                case ConcreteSlotValueType.Vector1:
                    return NodeUtils.FloatToShaderValue(value.vectorValue.x);
                case ConcreteSlotValueType.Vector4:
                case ConcreteSlotValueType.Vector3:
                case ConcreteSlotValueType.Vector2:
                    {
                        string values = NodeUtils.FloatToShaderValue(value.vectorValue.x);
                        for (var i = 1; i < channelCount; i++)
                            values += ", " + NodeUtils.FloatToShaderValue(value.vectorValue[i]);
                        return string.Format("{0}{1}({2})", precision, channelCount, values);
                    }
                case ConcreteSlotValueType.Boolean:
                    return (value.booleanValue ? 1 : 0).ToString();
                case ConcreteSlotValueType.Texture2D:
                case ConcreteSlotValueType.Texture3D:
                case ConcreteSlotValueType.Texture2DArray:
                case ConcreteSlotValueType.Cubemap:
                case ConcreteSlotValueType.SamplerState:
                    return matOwner.GetVariableNameForSlot(id);
                case ConcreteSlotValueType.Matrix2:
                    return string.Format("{0}2x2 ({1},{2},{3},{4})", precision, 
                        matrix.m00, matrix.m01, 
                        matrix.m10, matrix.m11);
                case ConcreteSlotValueType.Matrix3:
                    return string.Format("{0}3x3 ({1},{2},{3},{4},{5},{6},{7},{8},{9})", precision, 
                        matrix.m00, matrix.m01, matrix.m02, 
                        matrix.m10, matrix.m11, matrix.m12,
                        matrix.m20, matrix.m21, matrix.m22);
                case ConcreteSlotValueType.Matrix4:
                    return string.Format("{0}4x4 ({1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16})", precision, 
                        matrix.m00, matrix.m01, matrix.m02, matrix.m03, 
                        matrix.m10, matrix.m11, matrix.m12, matrix.m13,
                        matrix.m20, matrix.m21, matrix.m22, matrix.m23,
                        matrix.m30, matrix.m31, matrix.m32, matrix.m33);
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

            string overrideReferenceName = matOwner.GetVariableNameForSlot(id);
            IShaderProperty[] defaultProperties = PortUtil.GetDefaultPropertiesFromShaderValue(this);

            foreach(IShaderProperty property in defaultProperties)
                properties.AddShaderProperty(property);
        }

        public override void CopyValuesFrom(MaterialSlot foundSlot)
        {
            var port = foundSlot as ShaderPort;
            if (port != null)
            {
                m_ConcreteValueType = port.concreteValueType;
                m_ShaderValue = port.value;
                control = port.control;
            }
        }
    }
}
