using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Title("Test")]
    class TestNode : AbstractMaterialNode, IGeneratesBodyCode, IGeneratesFunction
    {
        public TestNode()
        {
            name = "Test";
            UpdateNodeAfterDeserialization();
        }

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new ShaderPort(0, "Gradient", SlotType.Input, SlotValueType.Gradient, new GradientControl()));

            /*AddSlot(new ShaderPort(0, "1D", SlotType.Input, SlotValueType.Vector1, new Vector1Control(1.0f)));
            AddSlot(new ShaderPort(1, "1ND", SlotType.Input, SlotValueType.Vector1, new Vector1Control()));

            AddSlot(new ShaderPort(2, "2D", SlotType.Input, SlotValueType.Vector2, new Vector2Control(Vector2.one)));
            AddSlot(new ShaderPort(3, "2ND", SlotType.Input, SlotValueType.Vector2, new Vector2Control()));

            AddSlot(new ShaderPort(4, "3D", SlotType.Input, SlotValueType.Vector3, new Vector3Control(Vector3.one)));
            AddSlot(new ShaderPort(5, "3ND", SlotType.Input, SlotValueType.Vector3, new Vector3Control()));

            AddSlot(new ShaderPort(6, "4D", SlotType.Input, SlotValueType.Vector4, new Vector4Control(Vector4.one)));
            AddSlot(new ShaderPort(7, "4ND", SlotType.Input, SlotValueType.Vector4, new Vector4Control()));

            AddSlot(new ShaderPort(8, "3COLD", SlotType.Input, SlotValueType.Vector3, new ColorControl(Color.red, true)));
            AddSlot(new ShaderPort(9, "3COLND", SlotType.Input, SlotValueType.Vector3, new ColorControl()));

            AddSlot(new ShaderPort(10, "4COLD", SlotType.Input, SlotValueType.Vector4, new ColorControl(Color.red, true)));
            AddSlot(new ShaderPort(11, "4COLND", SlotType.Input, SlotValueType.Vector4, new ColorControl()));

            AddSlot(new ShaderPort(12, "GRADD", SlotType.Input, SlotValueType.Gradient, new GradientControl(new Gradient() { colorKeys = new GradientColorKey[] { new GradientColorKey(Color.red, 0), new GradientColorKey(Color.blue, 1) }} )));
            AddSlot(new ShaderPort(13, "GRADND", SlotType.Input, SlotValueType.Gradient, new GradientControl()));

            AddSlot(new ShaderPort(14, "BD", SlotType.Input, SlotValueType.Boolean, new ToggleControl(true)));
            AddSlot(new ShaderPort(15, "BND", SlotType.Input, SlotValueType.Boolean, new ToggleControl()));*/
        }

        public void GenerateNodeCode(ShaderGenerator visitor, GraphContext graphContext, GenerationMode generationMode)
        {

        }

        public void GenerateNodeFunction(FunctionRegistry registry, GraphContext graphContext, GenerationMode generationMode)
        {

        }
    }
}
