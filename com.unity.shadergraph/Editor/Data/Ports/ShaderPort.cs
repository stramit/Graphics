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
            m_ValueType = portDescriptor.valueType;
        }

        public ShaderPort(InPortDescriptor portDescriptor)
            : base(portDescriptor.id, portDescriptor.name, portDescriptor.name, portDescriptor.portType, ShaderStageCapability.All, false)
        {
            m_ValueType = portDescriptor.valueType;
            m_ShaderValue = portDescriptor.defaultValue;
            control = portDescriptor.control;
        }

        [SerializeField]
        private SlotValueType m_ValueType = SlotValueType.Vector1;

        public override SlotValueType valueType
        {
            get { return m_ValueType; }
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
                    case SlotValueType.Texture2D:
                        return ConcreteSlotValueType.Texture2D;
                    case SlotValueType.Texture3D:
                        return ConcreteSlotValueType.Texture3D;
                    case SlotValueType.Texture2DArray:
                        return ConcreteSlotValueType.Texture2DArray;
                    case SlotValueType.Cubemap:
                        return ConcreteSlotValueType.Cubemap;
                    case SlotValueType.SamplerState:
                        return ConcreteSlotValueType.SamplerState;
                    case SlotValueType.Matrix2:
                        return ConcreteSlotValueType.Matrix2;
                    case SlotValueType.Matrix3:
                        return ConcreteSlotValueType.Matrix3;
                    case SlotValueType.Matrix4:
                        return ConcreteSlotValueType.Matrix4;
                    case SlotValueType.Gradient:
                        return ConcreteSlotValueType.Gradient;
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
                case ConcreteSlotValueType.Texture2D:
                    property = new TextureShaderProperty();
                    break;
                case ConcreteSlotValueType.Texture3D:
                    property = new Texture3DShaderProperty();
                    break;
                case ConcreteSlotValueType.Texture2DArray:
                    property = new Texture2DArrayShaderProperty();
                    break;
                case ConcreteSlotValueType.Cubemap:
                    property = new CubemapShaderProperty();
                    break;
                case ConcreteSlotValueType.SamplerState:
                    property = new SamplerStateShaderProperty();
                    break;
                case ConcreteSlotValueType.Matrix2:
                    property = new Matrix2ShaderProperty();
                    break;
                case ConcreteSlotValueType.Matrix3:
                    property = new Matrix3ShaderProperty();
                    break;
                case ConcreteSlotValueType.Matrix4:
                    property = new Matrix4ShaderProperty();
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
            properties.AddShaderProperty(new Vector1ShaderProperty()
            {
                overrideReferenceName = string.Format("{0}_Type", matOwner.GetVariableNameForSlot(id)),
                value = (int)value.gradientValue.mode,
                generatePropertyBlock = false
            });

            properties.AddShaderProperty(new Vector1ShaderProperty()
            {
                overrideReferenceName = string.Format("{0}_ColorsLength", matOwner.GetVariableNameForSlot(id)),
                value = value.gradientValue.colorKeys.Length,
                generatePropertyBlock = false
            });

            properties.AddShaderProperty(new Vector1ShaderProperty()
            {
                overrideReferenceName = string.Format("{0}_AlphasLength", matOwner.GetVariableNameForSlot(id)),
                value = value.gradientValue.alphaKeys.Length,
                generatePropertyBlock = false
            });

            for (int i = 0; i < 8; i++)
            {
                properties.AddShaderProperty(new Vector4ShaderProperty()
                {
                    overrideReferenceName = string.Format("{0}_ColorKey{1}", matOwner.GetVariableNameForSlot(id), i),
                    value = i < value.gradientValue.colorKeys.Length ? GradientUtils.ColorKeyToVector(value.gradientValue.colorKeys[i]) : Vector4.zero,
                    generatePropertyBlock = false
                });
            }

            for (int i = 0; i < 8; i++)
            {
                properties.AddShaderProperty(new Vector4ShaderProperty()
                {
                    overrideReferenceName = string.Format("{0}_AlphaKey{1}", matOwner.GetVariableNameForSlot(id), i),
                    value = i < value.gradientValue.alphaKeys.Length ? GradientUtils.AlphaKeyToVector(value.gradientValue.alphaKeys[i]) : Vector2.zero,
                    generatePropertyBlock = false
                });
            }

            var prop = new GradientShaderProperty();
            prop.overrideReferenceName = matOwner.GetVariableNameForSlot(id);
            prop.generatePropertyBlock = false;
            prop.value = value.gradientValue;
            prop.OverrideMembers(matOwner.GetVariableNameForSlot(id));

            properties.AddShaderProperty(prop);
        }

        public override void CopyValuesFrom(MaterialSlot foundSlot)
        {
            var port = foundSlot as ShaderPort;
            if (port != null)
            {
                m_ValueType = port.valueType;
                m_ShaderValue = port.value;
                control = port.control;
            }
        }
    }
}
