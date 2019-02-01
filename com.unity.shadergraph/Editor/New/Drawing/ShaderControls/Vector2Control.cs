using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.ShaderGraph.Drawing.Slots;

namespace UnityEditor.ShaderGraph
{
    class Vector2Control : VectorControl
    {       
        public override SlotValueType[] validPortTypes
        {
            get { return new SlotValueType[] { SlotValueType.Vector2 }; }
        }

        public override int portControlWidth
        {
            get { return 84; }
        }

        public Vector2Control()
        {
            this.controlData = new ShaderControlData()
            {
                labels = new string[] { "X", "Y" }
            };
        }

        public Vector2Control(Vector2 defaultValue, string labelX = "X", string labelY = "Y")
        {
            this.defaultValueData = new ShaderValueData()
            {
                vector = new Vector4(defaultValue.x, defaultValue.y, 0.0f, 0.0f)
            };
            this.controlData = new ShaderControlData()
            {
                labels = new string[] { labelX, labelY }
            };
        }
    }
}
