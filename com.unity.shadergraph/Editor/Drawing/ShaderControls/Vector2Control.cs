using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.ShaderGraph.Drawing.Slots;

namespace UnityEditor.ShaderGraph
{
    class Vector2Control : IShaderControl
    {
        static readonly string[] k_VectorComponentLabels = { "X", "Y" };
        
        public SerializableValueStore defaultValue { get; }

        public SlotValueType[] validPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Vector2 }; }
        }

        public Vector2Control()
        {
        }

        public Vector2Control(Vector2 defaultValue)
        {
            this.defaultValue = new SerializableValueStore()
            {
                vectorValue = new Vector4(defaultValue.x, defaultValue.y, 0.0f, 0.0f)
            };
        }

        public VisualElement GetControl(ShaderPort port)
        {
            var labels = k_VectorComponentLabels.Take(port.concreteValueType.GetChannelCount()).ToArray();
            return new MultiFloatSlotControlView(port.owner, labels, () => port.portValue.vectorValue, (newValue) => port.portValue.vectorValue = newValue);
        }
    }
}
