using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.ShaderGraph.Drawing.Slots;

namespace UnityEditor.ShaderGraph
{
    class Vector4Control : IShaderControl
    {
        static readonly string[] k_VectorComponentLabels = { "X", "Y", "Z", "W" };
        public Vector4 defaultValue { get; }

        public Vector4Control()
        {
            this.defaultValue = Vector4.zero;
        }

        public Vector4Control(Vector4 defaultValue)
        {
            this.defaultValue = defaultValue;
        }

        public SlotValueType[] GetValidPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Vector4 }; }
        }

        public void UpdateDefaultValue(ShaderPort port)
        {
            port.vectorValue = defaultValue;
        }

        public VisualElement GetControl(ShaderPort port)
        {
            var labels = k_VectorComponentLabels.Take(port.concreteValueType.GetChannelCount()).ToArray();
            return new MultiFloatSlotControlView(port.owner, labels, () => port.vectorValue, (newValue) => port.vectorValue = newValue);
        }
    }
}
