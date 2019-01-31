using System;
using UnityEngine.UIElements;

namespace UnityEditor.ShaderGraph
{
    class DefaultControl : IShaderControl
    {
        public ShaderControlData controlData { get; set; }
        public ShaderValueData defaultValueData { get; set; }

        public SlotValueType[] validPortTypes
        {
            get { return (SlotValueType[])Enum.GetValues(typeof(SlotValueType)); }
        }

        public int portControlWidth
        {
            get { return 0; }
        }

        public DefaultControl()
        {
        }

        public VisualElement GetControl(IShaderInput shaderInput)
        {
            VisualElement control = new VisualElement() { name = "DefaultControl" };
            return control;
        }
    }
}
