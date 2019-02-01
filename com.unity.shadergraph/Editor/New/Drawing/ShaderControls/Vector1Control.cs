using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.ShaderGraph.Drawing.Slots;

namespace UnityEditor.ShaderGraph
{
    class Vector1Control : VectorControl
    {
        public override SlotValueType[] validPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Vector1 }; }
        }

        public override int portControlWidth
        {
            get { return 42; }
        }

        public Vector1Control()
        {
            this.controlData = new ShaderControlData()
            {
                labels = new string[] { "X" }
            };
        }

        public Vector1Control(float defaultValue, string labelX = "X")
        {
            this.defaultValueData = new ShaderValueData()
            {
                vector = new Vector4(defaultValue, 0.0f, 0.0f, 0.0f)
            };
            this.controlData = new ShaderControlData()
            {
                labels = new string[] { labelX }
            };
        }
    }
}
