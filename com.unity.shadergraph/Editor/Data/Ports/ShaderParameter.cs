using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    class ShaderParameter : IShaderValue
    {
        // TODO: Duplicate from MaterialSlot, Hidden in ShaderPort
        [SerializeField]
        int m_Id;

        // TODO: Duplicate from MaterialSlot, Hidden in ShaderPort
        [SerializeField]
        string m_DisplayName = "Not Initilaized";

        [SerializeField]
        string m_ShaderOutputName;

        [SerializeField]
        private ConcreteSlotValueType m_ConcreteValueType = ConcreteSlotValueType.Vector1;

        [SerializeField]
        private SerializableValueStore m_ShaderValue;

        [SerializeField]
        private SerializableControl m_SerializedControl;

        public ShaderParameter(InPortDescriptor portDescriptor)
        {
            m_Id = portDescriptor.id;
            m_DisplayName = portDescriptor.name;
            m_ShaderOutputName = NodeUtils.GetHLSLSafeName(portDescriptor.name);
            m_ConcreteValueType = portDescriptor.valueType;
            m_ShaderValue = portDescriptor.defaultValue;
            control = portDescriptor.control;
        }

        public int id
        {
            get { return m_Id; }
        }

        // TODO: Defined by both MaterialSlot and IShaderValue
        public string shaderOutputName
        {
            get { return m_ShaderOutputName; }
        }

        public INode owner { get; set; }

        public ConcreteSlotValueType concreteValueType
        {
            get { return m_ConcreteValueType; }
        }

        public SerializableValueStore value
        {
            get { return m_ShaderValue; }
        }

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

        public void UpdateValue(SerializableValueStore value)
        {
            if(!m_ShaderValue.Equals(value))
            {
                m_ShaderValue = value;
                owner.owner.owner.RegisterCompleteObjectUndo("Shader Value Change");
                owner.Dirty(ModificationScope.Node);
            }
        }

        // TODO: Duplicate from ShaderPort
        public void GetPreviewProperties(List<PreviewProperty> properties, string name)
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

        // TODO: Duplicate from ShaderPort
        public void AddDefaultProperty(PropertyCollector properties, GenerationMode generationMode)
        {
            if (!generationMode.IsPreview())
                return;

            var matOwner = owner as AbstractMaterialNode;
            if (matOwner == null)
                throw new Exception(string.Format("Slot {0} either has no owner, or the owner is not a {1}", this, typeof(AbstractMaterialNode)));

            string overrideReferenceName = ShaderValueToVariableName(this);
            IShaderProperty[] defaultProperties = PortUtil.GetDefaultPropertiesFromShaderValue(this, overrideReferenceName);

            foreach(IShaderProperty property in defaultProperties)
                properties.AddShaderProperty(property);
        }

        // TODO: Replaces AbstractMaterialNode.GetVariableNameForSlot
        // Also duplicate from ShaderNode except this uses owner
        private string ShaderValueToVariableName(IShaderValue shaderValue)
        {
            var aNode = owner as AbstractMaterialNode;
            if(aNode == null)
                return string.Empty;

            return string.Format("_{0}_{1}", aNode.GetVariableNameForNode(), NodeUtils.GetHLSLSafeName(shaderValue.shaderOutputName));
        }

        // TODO: Duplicate from MaterialSlot
        protected static PropertyType ConvertConcreteSlotValueTypeToPropertyType(ConcreteSlotValueType slotValue)
        {
            switch (slotValue)
            {
                case ConcreteSlotValueType.Texture2D:
                    return PropertyType.Texture2D;
                case ConcreteSlotValueType.Texture2DArray:
                    return PropertyType.Texture2DArray;
                case ConcreteSlotValueType.Texture3D:
                    return PropertyType.Texture3D;
                case ConcreteSlotValueType.Cubemap:
                    return PropertyType.Cubemap;
                case ConcreteSlotValueType.Gradient:
                    return PropertyType.Gradient;
                case ConcreteSlotValueType.Boolean:
                    return PropertyType.Boolean;
                case ConcreteSlotValueType.Vector1:
                    return PropertyType.Vector1;
                case ConcreteSlotValueType.Vector2:
                    return PropertyType.Vector2;
                case ConcreteSlotValueType.Vector3:
                    return PropertyType.Vector3;
                case ConcreteSlotValueType.Vector4:
                    return PropertyType.Vector4;
                case ConcreteSlotValueType.Matrix2:
                    return PropertyType.Matrix2;
                case ConcreteSlotValueType.Matrix3:
                    return PropertyType.Matrix3;
                case ConcreteSlotValueType.Matrix4:
                    return PropertyType.Matrix4;
                case ConcreteSlotValueType.SamplerState:
                    return PropertyType.SamplerState;
                default:
                    return PropertyType.Vector4;
            }
        }
    }
}
