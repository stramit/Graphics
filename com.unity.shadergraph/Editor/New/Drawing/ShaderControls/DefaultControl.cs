using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    class DefaultControl : IShaderControl
    {
        public ShaderControlData controlData { get; set; }
        public ShaderValueData defaultValueData { get; }

        public ConcreteSlotValueType[] validPortTypes
        {
            get { return (ConcreteSlotValueType[])Enum.GetValues(typeof(ConcreteSlotValueType)); }
        }

        public DefaultControl()
        {
        }

        public VisualElement GetControl(IShaderValue shaderValue)
        {
            VisualElement control = new VisualElement() { name = "DefaultControl" };
            return control;
        }
    }
}
