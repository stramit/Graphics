using UnityEditor.ShaderGraph.Serialization;

namespace UnityEditor.ShaderGraph
{
    interface IContext
    {
        string name { get; }
        JsonList<PortData> inputPorts { get; }
        JsonList<PortData> outputPorts { get; }
    }
}
