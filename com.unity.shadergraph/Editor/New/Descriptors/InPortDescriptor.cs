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
        private ConcreteSlotValueType m_ValueType;
        
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

        public ConcreteSlotValueType valueType
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

        public InPortDescriptor(int id, string name, ConcreteSlotValueType valueType)
        {
            m_Id = id;
            m_Name = name;
            m_ValueType = valueType;
            
            this.control = defaultControl;
            m_DefaultValue = new SerializableValueStore();
        }

        public InPortDescriptor(int id, string name, ConcreteSlotValueType valueType, IShaderControl control)
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
                    case ConcreteSlotValueType.Vector4:
                        return new Vector4Control();
                    case ConcreteSlotValueType.Vector3:
                        return new Vector3Control();
                    case ConcreteSlotValueType.Vector2:
                        return new Vector4Control();
                    case ConcreteSlotValueType.Vector1:
                        return new Vector1Control();
                    case ConcreteSlotValueType.Boolean:
                        return new ToggleControl();
                    case ConcreteSlotValueType.Texture2D:
                        return new TextureControl<Texture>();
                    case ConcreteSlotValueType.Texture3D:
                        return new TextureControl<Texture3D>();
                    case ConcreteSlotValueType.Texture2DArray:
                        return new TextureControl<Texture2DArray>();
                    case ConcreteSlotValueType.Cubemap:
                        return new TextureControl<Cubemap>();
                    case ConcreteSlotValueType.SamplerState:
                        return new LabelControl("Default");
                    case ConcreteSlotValueType.Matrix2:
                        return new LabelControl("Identity");
                    case ConcreteSlotValueType.Matrix3:
                        return new LabelControl("Identity");
                    case ConcreteSlotValueType.Matrix4:
                        return new LabelControl("Identity");
                    case ConcreteSlotValueType.Gradient:
                        return new GradientControl();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
