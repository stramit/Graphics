using UnityEditor.ShaderGraph.Serialization;

namespace UnityEditor.ShaderGraph
{
    class FragmentContext : IContext
    {
        public string name => "Fragment";
        public JsonList<PortData> inputPorts => new JsonList<PortData>()
        { 
            new PortData(string.Empty, typeof(Varyings), PortData.Orientation.Vertical, PortData.Direction.Input),
        };
        public JsonList<PortData> outputPorts => new JsonList<PortData>()
        {
            new PortData(string.Empty, typeof(Surface), PortData.Orientation.Vertical, PortData.Direction.Output),
        };
    }
}
