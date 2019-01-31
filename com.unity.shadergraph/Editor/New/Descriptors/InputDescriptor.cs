using System.Linq;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    class InputDescriptor : IShaderValueDescriptor
    {
        public SerializableGuid guid => new SerializableGuid();

        public SlotType portType => SlotType.Input;
        public SlotValueType valueType { get; }

        public string name { get; }

        public IShaderControl control;
        public ShaderValueData valueData;

        public InputDescriptor(string name, SlotValueType valueType)
        {
            this.name = name;
            this.valueType = valueType;
            this.control = valueType.ToDefaultControl();
            this.valueData = new ShaderValueData();
        }

        public InputDescriptor(string name, SlotValueType valueType, IShaderControl control)
        {
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
