using System;
using System.Linq;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    class InPortDescriptor : IPortDescriptor
    {
        [SerializeField]
        private int m_Id;

        [SerializeField]
        private string m_Name;

        [SerializeField]
        private SlotValueType m_ValueType;
        
        [SerializeField]
        private SerializableValueStore m_DefaultValue;

        [SerializeField]
        private SerializableControl m_SerializedControl;

        public int id
        {
            get { return m_Id; }
        }

        public string name
        {
            get { return m_Name; }
        }

        public SlotValueType valueType
        {
            get { return m_ValueType; }
        }

        public SlotType portType
        {
            get { return SlotType.Input; }
        }

        public SerializableValueStore defaultValue
        {
            get { return m_DefaultValue; }
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

        public InPortDescriptor(int id, string name, SlotValueType valueType)
        {
            m_Id = id;
            m_Name = name;
            m_ValueType = valueType;
            
            this.control = defaultControl;
            m_DefaultValue = new SerializableValueStore();
        }

        public InPortDescriptor(int id, string name, SlotValueType valueType, IShaderControl control)
        {
            m_Id = id;
            m_Name = name;
            m_ValueType = valueType;

            if(!control.validPortTypes.Contains(valueType))
            {
                Debug.LogWarning(string.Format("InPortDescrption {0} tried to define an incompatible Control. Will use default Control instead.", name));
                control = defaultControl;
            }
            this.control = control;
            m_DefaultValue = control.defaultValue != null ? control.defaultValue : new SerializableValueStore();
        }

        private IShaderControl defaultControl
        {
            get
            {
                switch (m_ValueType)
                {
                    case SlotValueType.Vector4:
                        return new Vector4Control();
                    case SlotValueType.Vector3:
                        return new Vector3Control();
                    case SlotValueType.Vector2:
                        return new Vector4Control();
                    case SlotValueType.Vector1:
                        return new Vector1Control();
                    case SlotValueType.Boolean:
                        return new ToggleControl();
                    case SlotValueType.Texture2D:
                        return new TextureControl<Texture>();
                    case SlotValueType.Texture3D:
                        return new TextureControl<Texture3D>();
                    case SlotValueType.Texture2DArray:
                        return new TextureControl<Texture2DArray>();
                    case SlotValueType.Cubemap:
                        return new TextureControl<Cubemap>();
                    case SlotValueType.SamplerState:
                        return new LabelControl("Default");
                    case SlotValueType.Matrix2:
                        return new LabelControl("Identity");
                    case SlotValueType.Matrix3:
                        return new LabelControl("Identity");
                    case SlotValueType.Matrix4:
                        return new LabelControl("Identity");
                    case SlotValueType.Gradient:
                        return new GradientControl();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
