using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.ShaderGraph.Drawing.Slots;

namespace UnityEditor.ShaderGraph
{
    class Vector1Control : IShaderControl
    {
        static readonly string[] k_VectorComponentLabels = { "X" };

        public SerializableValueStore defaultValue { get; }

        public SlotValueType[] validPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Vector1 }; }
        }

        public Vector1Control()
        {
        }

        public Vector1Control(float defaultValue)
        {
            this.defaultValue = new SerializableValueStore()
            {
                vectorValue = new Vector4(defaultValue, 0.0f, 0.0f, 0.0f)
            };
        }

        public VisualElement GetControl(ShaderPort port)
        {
            var labels = k_VectorComponentLabels.Take(port.concreteValueType.GetChannelCount()).ToArray();
            return new MultiFloatSlotControlView(port.owner, labels, () => port.portValue.vectorValue, (newValue) => port.portValue.vectorValue = newValue);
        }
    }
}
