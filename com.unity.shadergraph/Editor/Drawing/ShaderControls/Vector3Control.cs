using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.ShaderGraph.Drawing.Slots;

namespace UnityEditor.ShaderGraph
{
    class Vector3Control : IShaderControl
    {
        static readonly string[] k_VectorComponentLabels = { "X", "Y", "Z" };
        public Vector3 defaultValue { get; }

        public Vector3Control()
        {
            this.defaultValue = Vector3.zero;
        }

        public Vector3Control(Vector3 defaultValue)
        {
            this.defaultValue = defaultValue;
        }

        public SlotValueType[] GetValidPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Vector3 }; }
        }

        public void UpdateDefaultValue(ShaderPort port)
        {
            port.vectorValue = new Vector4(defaultValue.x, defaultValue.y, defaultValue.z, 0);
        }

        public VisualElement GetControl(ShaderPort port)
        {
            var labels = k_VectorComponentLabels.Take(port.concreteValueType.GetChannelCount()).ToArray();
            return new MultiFloatSlotControlView(port.owner, labels, () => port.vectorValue, (newValue) => port.vectorValue = newValue);
        }
    }
}
