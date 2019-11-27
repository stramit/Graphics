using UnityEditor.ShaderGraph.Serialization;

namespace UnityEditor.ShaderGraph
{
    class VertexContext : IContext
    {
        public string name => "Vertex";
        public JsonList<PortData> inputPorts => new JsonList<PortData>();
        public JsonList<PortData> outputPorts => new JsonList<PortData>()
        {
            new PortData(string.Empty, typeof(Varyings), PortData.Orientation.Vertical, PortData.Direction.Output),
        };
    }
}
