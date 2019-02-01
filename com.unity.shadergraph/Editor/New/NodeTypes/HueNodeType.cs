namespace UnityEditor.ShaderGraph
{
    sealed class HueNodeType : ShaderNodeType
    {
        InputDescriptor m_InPort = new InputDescriptor(0, "In", SlotValueType.Vector3, new ColorControl());
        InputDescriptor m_OffsetPort = new InputDescriptor(1, "Offset", SlotValueType.Vector1, new Vector1Control());
        InputDescriptor m_ModeParameter = new InputDescriptor(3, "Mode", SlotValueType.Vector1, new PopupControl(new string[] { "Degrees", "Normalized" }, 0));
        OutputDescriptor m_OutPort = new OutputDescriptor(2, "Out", SlotValueType.Vector3);

        internal override void Setup(ref NodeSetupContext context)
        {
            context.CreateNodeType(new NodeTypeDescriptor
            {
                path = "INTERNAL",
                name = "Hue",
                inPorts = new InputDescriptor[] { m_InPort, m_OffsetPort },
                outPorts = new OutputDescriptor[] { m_OutPort },
                parameters = new InputDescriptor[] { m_ModeParameter },
                preview = true
            });
        }

        internal override void OnNodeAdded(NodeChangeContext context, NodeRef node)
        {
            context.SetHlslFunction(node, new HlslFunctionDescriptor
            {
                name = "Unity_Hue",
                source = HlslSource.File("Packages/com.unity.shadergraph/Editor/New/NodeIncludes/ArtisticNodes.hlsl"),
                inArguments = new InputDescriptor[] { m_InPort, m_OffsetPort, m_ModeParameter },
                outArguments = new OutputDescriptor[] { m_OutPort }
            });
        }
    }
}
