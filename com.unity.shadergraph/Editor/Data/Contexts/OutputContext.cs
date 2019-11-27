using UnityEditor.ShaderGraph.Serialization;

namespace UnityEditor.ShaderGraph
{
    class OutputContext : IContext
    {
        public string name => "Output";
        public JsonList<PortData> inputPorts => new JsonList<PortData>()
        { 
            new PortData(string.Empty, typeof(Surface), PortData.Orientation.Vertical, PortData.Direction.Input),
        };
        public JsonList<PortData> outputPorts => new JsonList<PortData>();
    }
}
