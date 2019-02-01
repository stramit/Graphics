using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.ShaderGraph.Drawing.Slots;

namespace UnityEditor.ShaderGraph
{
    class Vector4Control : VectorControl
    {
        public override SlotValueType[] validPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Vector4 }; }
        }

        public override int portControlWidth
        {
            get { return 168; }
        }

        public Vector4Control()
        {
            this.controlData = new ShaderControlData()
            {
                labels = new string[] { "X", "Y", "Z", "W" }
            };
        }

        public Vector4Control(Vector4 defaultValue, string labelX = "X", string labelY = "Y", string labelZ = "Z", string labelW = "W")
        {
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
