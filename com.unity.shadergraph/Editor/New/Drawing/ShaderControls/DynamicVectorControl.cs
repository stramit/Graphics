using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.ShaderGraph.Drawing.Slots;

namespace UnityEditor.ShaderGraph
{
    class DynamicVectorControl : VectorControl
    {
        public override SlotValueType[] validPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.DynamicVector }; }
        }

        public override int portControlWidth
        {
            get { return -1; }
        }

        public DynamicVectorControl()
        {
            this.controlData = new ShaderControlData()
            {
                labels = new string[] { "X", "Y", "Z", "W" }
            };
        }

        public DynamicVectorControl(Vector4 defaultValue, string labelX = "X", string labelY = "Y", string labelZ = "Z", string labelW = "W")
        {
            var labels = 
            this.defaultValueData = new ShaderValueData()
            {
                vector = defaultValue
            };
            this.controlData = new ShaderControlData()
            {
                labels = new string[] { labelX, labelY, labelZ, labelW }
            };
        }
    }
}
