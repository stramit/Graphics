using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.ShaderGraph.Drawing.Slots;

namespace UnityEditor.ShaderGraph
{
    class Vector3Control : VectorControl
    {
        public override SlotValueType[] validPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Vector3 }; }
        }

        public override int portControlWidth
        {
            get { return 126; }
        }

        public Vector3Control()
        {
            this.controlData = new ShaderControlData()
            {
                labels = new string[] { "X", "Y", "Z" }
            };
        }

        public Vector3Control(Vector3 defaultValue, string labelX = "X", string labelY = "Y", string labelZ = "Z")
        {
            this.defaultValueData = new ShaderValueData()
            {
                vector = new Vector4(defaultValue.x, defaultValue.y, defaultValue.z, 0.0f)
            };
            this.controlData = new ShaderControlData()
            {
                labels = new string[] { labelX, labelY, labelZ }
            };
        }
    }
}
