using UnityEditor.ShaderGraph.Serialization;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    interface IContext
    {
        string name { get; }
        JsonList<PortData> inputPorts { get; }
        JsonList<PortData> outputPorts { get; }
    }

    class VertexContext : IContext
    {
        public string name => "Vertex";
        public JsonList<PortData> inputPorts => new JsonList<PortData>();
        public JsonList<PortData> outputPorts => new JsonList<PortData>()
        {
            new PortData(string.Empty, ShaderTypes.Structs.Varyings.descriptor, PortData.Orientation.Vertical, PortData.Direction.Output),
        };
    }

    class FragmentContext : IContext
    {
        public string name => "Fragment";
        public JsonList<PortData> inputPorts => new JsonList<PortData>()
        { 
            new PortData(string.Empty, ShaderTypes.Structs.Varyings.descriptor, PortData.Orientation.Vertical, PortData.Direction.Input),
        };
        public JsonList<PortData> outputPorts => new JsonList<PortData>()
        {
            new PortData(string.Empty, ShaderTypes.Structs.Output.descriptor, PortData.Orientation.Vertical, PortData.Direction.Output),
        };
    }

    class OutputContext : IContext
    {
        public string name => "Output";
        public JsonList<PortData> inputPorts => new JsonList<PortData>()
        { 
            new PortData(string.Empty, ShaderTypes.Structs.Output.descriptor, PortData.Orientation.Vertical, PortData.Direction.Input),
        };
        public JsonList<PortData> outputPorts => new JsonList<PortData>();
    }
}
