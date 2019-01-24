using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Title("Test")]
    class TestNode : AbstractMaterialNode
    {
        InPortDescriptor m_Port0 = new InPortDescriptor(0, "V1D", ConcreteSlotValueType.Vector1, new Vector1Control(1.0f, "R"));
        InPortDescriptor m_Port1 = new InPortDescriptor(1, "V1", ConcreteSlotValueType.Vector1);

        InPortDescriptor m_Port2 = new InPortDescriptor(2, "V2D", ConcreteSlotValueType.Vector2, new Vector2Control(Vector2.one, "R", "G"));
        InPortDescriptor m_Port3 = new InPortDescriptor(3, "V2", ConcreteSlotValueType.Vector2);

        InPortDescriptor m_Port4 = new InPortDescriptor(4, "V3D", ConcreteSlotValueType.Vector3, new Vector3Control(Vector3.one, "R", "G", "B"));
        InPortDescriptor m_Port5 = new InPortDescriptor(5, "V3", ConcreteSlotValueType.Vector3);

        InPortDescriptor m_Port6 = new InPortDescriptor(6, "V4D", ConcreteSlotValueType.Vector4, new Vector4Control(Vector4.one, "R", "G", "B", "A"));
        InPortDescriptor m_Port7 = new InPortDescriptor(7, "V4", ConcreteSlotValueType.Vector4);

        InPortDescriptor m_Port8 = new InPortDescriptor(8, "3ColorD", ConcreteSlotValueType.Vector3, new ColorControl(Color.red, true));
        InPortDescriptor m_Port9 = new InPortDescriptor(9, "3Color", ConcreteSlotValueType.Vector3, new ColorControl());

        InPortDescriptor m_Port10 = new InPortDescriptor(10, "4ColorD", ConcreteSlotValueType.Vector4, new ColorControl(Color.red, true));
        InPortDescriptor m_Port11 = new InPortDescriptor(11, "4Color", ConcreteSlotValueType.Vector4, new ColorControl());

        InPortDescriptor m_Port12 = new InPortDescriptor(12, "Bool", ConcreteSlotValueType.Boolean, new ToggleControl(true));
        InPortDescriptor m_Port13 = new InPortDescriptor(13, "BoolD", ConcreteSlotValueType.Boolean);

        InPortDescriptor m_Port14 = new InPortDescriptor(14, "GradientD", ConcreteSlotValueType.Gradient, new GradientControl(new Gradient() { colorKeys = new GradientColorKey[] { new GradientColorKey(Color.red, 0), new GradientColorKey(Color.blue, 1) }} ));
        InPortDescriptor m_Port15 = new InPortDescriptor(15, "Gradient", ConcreteSlotValueType.Gradient);

        InPortDescriptor m_Port16 = new InPortDescriptor(16, "1Label", ConcreteSlotValueType.Vector1, new LabelControl("Test"));

        InPortDescriptor m_Port17 = new InPortDescriptor(17, "Texture2D", ConcreteSlotValueType.Texture2D);
        InPortDescriptor m_Port18 = new InPortDescriptor(18, "Texture3D", ConcreteSlotValueType.Texture3D);
        InPortDescriptor m_Port19 = new InPortDescriptor(19, "Texture2DArray", ConcreteSlotValueType.Texture2DArray);
        InPortDescriptor m_Port20 = new InPortDescriptor(20, "Cubemap", ConcreteSlotValueType.Cubemap);

        InPortDescriptor m_Port21 = new InPortDescriptor(21, "Sampler", ConcreteSlotValueType.SamplerState);

        InPortDescriptor m_Port22 = new InPortDescriptor(22, "Matrix2x2", ConcreteSlotValueType.Matrix2);
        InPortDescriptor m_Port23 = new InPortDescriptor(23, "Matrix3x3", ConcreteSlotValueType.Matrix3);
        InPortDescriptor m_Port24 = new InPortDescriptor(24, "Matrix4x4", ConcreteSlotValueType.Matrix4);

        InPortDescriptor m_Port25 = new InPortDescriptor(25, "V1PopupD", ConcreteSlotValueType.Vector1, new PopupControl(new string[] {"Option A", "Option B", "Option C"}, 1));
        InPortDescriptor m_Port26 = new InPortDescriptor(26, "V1Popup", ConcreteSlotValueType.Vector1, new PopupControl(new string[] {"Option A", "Option B", "Option C"}));

        InPortDescriptor m_Port27 = new InPortDescriptor(27, "V1SliderD", ConcreteSlotValueType.Vector1, new SliderControl(0.0f, -1.0f, 1.0f));
        InPortDescriptor m_Port28 = new InPortDescriptor(28, "V1Slider", ConcreteSlotValueType.Vector1, new SliderControl());

        InPortDescriptor m_Port29 = new InPortDescriptor(29, "V1IntegerD", ConcreteSlotValueType.Vector1, new IntegerControl(1));
        InPortDescriptor m_Port30 = new InPortDescriptor(30, "V1Integer", ConcreteSlotValueType.Vector1, new IntegerControl());

        public TestNode()
        {
            NodeSetup(new NodeDescriptor()
            {
                name = "Test",
                inPorts = new InPortDescriptor[]
                {
                    m_Port0,
                    m_Port1,
                    m_Port2,
                    m_Port3,
                    m_Port4,
                    m_Port5,
                    m_Port6,
                    m_Port7,
                    m_Port8,
                    m_Port9,
                    m_Port10,
                    m_Port11,
                    m_Port12,
                    m_Port13,
                    m_Port14,
                    m_Port15,
                    m_Port16,
                    m_Port17,
                    m_Port18,
                    m_Port19,
                    m_Port20,
                    m_Port21,
                    m_Port22,
                    m_Port23,
                    m_Port24,
                    m_Port25,
                    m_Port26,
                    m_Port27,
                    m_Port28,
                    m_Port29,
                    m_Port30,
                }
            });
        }
    }
}

