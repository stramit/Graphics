using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Title("Test")]
    class TestNode : AbstractMaterialNode
    {
        InPortDescriptor m_Port0 = new InPortDescriptor(0, "V1D", SlotValueType.Vector1, new Vector1Control(1.0f));
        InPortDescriptor m_Port1 = new InPortDescriptor(1, "V1", SlotValueType.Vector1);

        InPortDescriptor m_Port2 = new InPortDescriptor(2, "V2D", SlotValueType.Vector2, new Vector2Control(Vector2.one));
        InPortDescriptor m_Port3 = new InPortDescriptor(3, "V2", SlotValueType.Vector2);

        InPortDescriptor m_Port4 = new InPortDescriptor(4, "V3D", SlotValueType.Vector3, new Vector3Control(Vector3.one));
        InPortDescriptor m_Port5 = new InPortDescriptor(5, "V3", SlotValueType.Vector3);

        InPortDescriptor m_Port6 = new InPortDescriptor(6, "V4D", SlotValueType.Vector4, new Vector4Control(Vector4.one));
        InPortDescriptor m_Port7 = new InPortDescriptor(7, "V4", SlotValueType.Vector4);

        InPortDescriptor m_Port8 = new InPortDescriptor(8, "3ColorD", SlotValueType.Vector3, new ColorControl(Color.red, true));
        InPortDescriptor m_Port9 = new InPortDescriptor(9, "3Color", SlotValueType.Vector3, new ColorControl());

        InPortDescriptor m_Port10 = new InPortDescriptor(10, "4ColorD", SlotValueType.Vector4, new ColorControl(Color.red, true));
        InPortDescriptor m_Port11 = new InPortDescriptor(11, "4Color", SlotValueType.Vector4, new ColorControl());

        InPortDescriptor m_Port12 = new InPortDescriptor(12, "Bool", SlotValueType.Boolean, new ToggleControl(true));
        InPortDescriptor m_Port13 = new InPortDescriptor(13, "BoolD", SlotValueType.Boolean);

        InPortDescriptor m_Port14 = new InPortDescriptor(14, "GradientD", SlotValueType.Gradient, new GradientControl(new Gradient() { colorKeys = new GradientColorKey[] { new GradientColorKey(Color.red, 0), new GradientColorKey(Color.blue, 1) }} ));
        InPortDescriptor m_Port15 = new InPortDescriptor(15, "Gradient", SlotValueType.Gradient);

        InPortDescriptor m_Port16 = new InPortDescriptor(16, "1Label", SlotValueType.Vector1, new LabelControl("Test"));

        InPortDescriptor m_Port17 = new InPortDescriptor(17, "Texture2D", SlotValueType.Texture2D);
        InPortDescriptor m_Port18 = new InPortDescriptor(18, "Texture3D", SlotValueType.Texture3D);
        InPortDescriptor m_Port19 = new InPortDescriptor(19, "Texture2DArray", SlotValueType.Texture2DArray);
        InPortDescriptor m_Port20 = new InPortDescriptor(20, "Cubemap", SlotValueType.Cubemap);

        InPortDescriptor m_Port21 = new InPortDescriptor(21, "Sampler", SlotValueType.SamplerState);

        InPortDescriptor m_Port22 = new InPortDescriptor(22, "Matrix2x2", SlotValueType.Matrix2);
        InPortDescriptor m_Port23 = new InPortDescriptor(23, "Matrix3x3", SlotValueType.Matrix3);
        InPortDescriptor m_Port24 = new InPortDescriptor(24, "Matrix4x4", SlotValueType.Matrix4);

        InPortDescriptor m_Port25 = new InPortDescriptor(25, "V1PopupD", SlotValueType.Vector1, new PopupControl(new string[] {"Option A", "Option B", "Option C"}, 0));
        InPortDescriptor m_Port26 = new InPortDescriptor(26, "V1Popup", SlotValueType.Vector1, new PopupControl(new string[] {"Option A", "Option B", "Option C"}));

        InPortDescriptor m_Port27 = new InPortDescriptor(27, "V1SliderD", SlotValueType.Vector1, new SliderControl(0.5f, 0.0f, 1.0f));
        InPortDescriptor m_Port28 = new InPortDescriptor(28, "V1Slider", SlotValueType.Vector1, new SliderControl());

        InPortDescriptor m_Port29 = new InPortDescriptor(29, "V1IntegerD", SlotValueType.Vector1, new IntegerControl(1));
        InPortDescriptor m_Port30 = new InPortDescriptor(30, "V1Integer", SlotValueType.Vector1, new IntegerControl());

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

