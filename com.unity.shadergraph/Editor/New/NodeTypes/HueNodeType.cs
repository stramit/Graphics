namespace UnityEditor.ShaderGraph
{
    [Title("INTERNAL", "Hue")]
    class HueNodeType : ShaderNode
    {
        InputDescriptor m_InPort = new InputDescriptor(0, "In", ConcreteSlotValueType.Vector3, new ColorControl());
        InputDescriptor m_OffsetPort = new InputDescriptor(1, "Offset", ConcreteSlotValueType.Vector1, new Vector1Control());
        OutputDescriptor m_OutPort = new OutputDescriptor(2, "Out", ConcreteSlotValueType.Vector3);

        InputDescriptor m_ModeParameter = new InputDescriptor(3, "Mode", ConcreteSlotValueType.Vector1, new PopupControl(new string[] { "Degrees", "Normalized" }, 0));

        public override void Setup(ref NodeSetupContext context)
        {
            context.CreateType(new NodeTypeDescriptor
            {
                name = "Hue",
                inPorts = new InputDescriptor[] { m_InPort, m_OffsetPort },
                outPorts = new OutputDescriptor[] { m_OutPort },
                parameters = new InputDescriptor[] { m_ModeParameter },
                preview = true
            });
        }

        public override void OnModified(ref NodeChangeContext context)
        {
            context.SetHlslFunction(new HlslFunctionDescriptor
            {
                name = "Unity_Hue",
                body = s_FunctionBody,
                inArguments = new InputDescriptor[] { m_InPort, m_OffsetPort, m_ModeParameter },
                outArguments = new OutputDescriptor[] { m_OutPort }
            });
        }

        static string s_FunctionBody { get { return 
@"// RGB to HSV
real4 K = real4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
real4 P = lerp(real4(In.bg, K.wz), real4(In.gb, K.xy), step(In.b, In.g));
real4 Q = lerp(real4(P.xyw, In.r), real4(In.r, P.yzx), step(P.x, In.r));
real D = Q.x - min(Q.w, Q.y);
real E = 1e-10;
real3 hsv = real3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);

real divisor = lerp(360, 1, Mode);
real hue = hsv.x + Offset / divisor;
hsv.x = (hue < 0)
        ? hue + 1
        : (hue > 1)
            ? hue - 1
            : hue;

// HSV to RGB
real4 K2 = real4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
real3 P2 = abs(frac(hsv.xxx + K2.xyz) * 6.0 - K2.www);
Out = hsv.z * lerp(K2.xxx, saturate(P2 - K2.xxx), hsv.y);
"; }}
    }
}

