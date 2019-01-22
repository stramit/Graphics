using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.ShaderGraph.Drawing.Slots;

namespace UnityEditor.ShaderGraph
{
    class Vector2Control : IShaderControl
    {
        static readonly string[] k_VectorComponentLabels = { "X", "Y" };
        public Vector2 defaultValue { get; }

        public Vector2Control()
        {
            this.defaultValue = Vector2.zero;
        }

        public Vector2Control(Vector2 defaultValue)
        {
            this.defaultValue = defaultValue;
        }

        public SlotValueType[] GetValidPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Vector2 }; }
        }

        public void UpdateDefaultValue(ShaderPort port)
        {
            port.vectorValue = new Vector4(defaultValue.x, defaultValue.y, 0, 0);
        }

        public VisualElement GetControl(ShaderPort port)
        {
            var labels = k_VectorComponentLabels.Take(port.concreteValueType.GetChannelCount()).ToArray();
            return new MultiFloatSlotControlView(port.owner, labels, () => port.vectorValue, (newValue) => port.vectorValue = newValue);
        }
    }
}
