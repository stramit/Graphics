using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.ShaderGraph.Drawing.Slots;

namespace UnityEditor.ShaderGraph
{
    class Vector1Control : IShaderControl
    {
        static readonly string[] k_VectorComponentLabels = { "X" };
        public float defaultValue { get; }

        public Vector1Control()
        {
            this.defaultValue = 0.0f;
        }

        public Vector1Control(float defaultValue)
        {
            this.defaultValue = defaultValue;
        }

        public SlotValueType[] GetValidPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Vector1 }; }
        }

        public void UpdateDefaultValue(ShaderPort port)
        {
            port.vectorValue = new Vector4(defaultValue, 0, 0, 0);
        }

        public VisualElement GetControl(ShaderPort port)
        {
            var labels = k_VectorComponentLabels.Take(port.concreteValueType.GetChannelCount()).ToArray();
            return new MultiFloatSlotControlView(port.owner, labels, () => port.vectorValue, (newValue) => port.vectorValue = newValue);
        }
    }
}
