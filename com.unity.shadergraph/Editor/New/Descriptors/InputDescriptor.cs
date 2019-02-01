using System;
using System.Linq;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    class InputDescriptor : IShaderValueDescriptor
    {
        public SerializableGuid guid => new SerializableGuid();
        public int id { get; set; }

        public SlotType portType => SlotType.Input;
        public SlotValueType valueType { get; }

        public string name { get; set; }

        public IShaderControl control;
        public ShaderValueData valueData;

        public InputDescriptor(int id, string name, SlotValueType valueType)
        {
            this.id = id;
            this.name = name;
            this.valueType = valueType;
            this.control = valueType.ToDefaultControl();
            this.valueData = new ShaderValueData();
        }

        public InputDescriptor(int id, string name, SlotValueType valueType, IShaderControl control)
        {
            this.id = id;
            this.name = name;
            this.valueType = valueType;
            
            if(!control.validPortTypes.Contains(valueType))
            {
                Debug.LogWarning(string.Format("InputDescriptor {0} tried to define an incompatible Control. Will use default Control instead.", name));
                control = valueType.ToDefaultControl();
            }
            this.control = control;
            this.valueData = control.defaultValueData;
        }
    }
}
