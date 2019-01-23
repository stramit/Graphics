using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.ShaderGraph.Drawing.Slots;

namespace UnityEditor.ShaderGraph
{
    class Vector4Control : IShaderControl
    {
        static readonly string[] k_VectorComponentLabels = { "X", "Y", "Z", "W" };

        public SerializableValueStore defaultValue { get; }

        public SlotValueType[] validPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Vector4 }; }
        }

        public Vector4Control()
        {
        }

        public Vector4Control(Vector4 defaultValue)
        {
            this.defaultValue = new SerializableValueStore()
            {
                vectorValue = defaultValue
            };
        }

        public VisualElement GetControl(ShaderPort port)
        {
            var labels = k_VectorComponentLabels.Take(port.concreteValueType.GetChannelCount()).ToArray();
            return new MultiFloatSlotControlView(port.owner, labels, () => port.portValue.vectorValue, (newValue) => port.portValue.vectorValue = newValue);
        }
    }
}
