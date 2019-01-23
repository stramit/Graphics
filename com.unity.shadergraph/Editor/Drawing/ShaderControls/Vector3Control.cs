using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.ShaderGraph.Drawing.Slots;

namespace UnityEditor.ShaderGraph
{
    class Vector3Control : IShaderControl
    {
        static readonly string[] k_VectorComponentLabels = { "X", "Y", "Z" };
        
        public SerializableValueStore defaultValue { get; }

        public SlotValueType[] validPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Vector3 }; }
        }

        public Vector3Control()
        {
        }

        public Vector3Control(Vector3 defaultValue)
        {
            this.defaultValue = new SerializableValueStore()
            {
                vectorValue = new Vector4(defaultValue.x, defaultValue.y, defaultValue.z, 0.0f)
            };
        }

        public VisualElement GetControl(ShaderPort port)
        {
            var labels = k_VectorComponentLabels.Take(port.concreteValueType.GetChannelCount()).ToArray();
            return new MultiFloatSlotControlView(port.owner, labels, () => port.portValue.vectorValue, (newValue) => port.portValue.vectorValue = newValue);
        }
    }
}
