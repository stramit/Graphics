using UnityEditor.Experimental.GraphView;

namespace UnityEditor.ShaderGraph.Drawing
{
    static class NodeViewExtensions
    {
        public static void AddPort(this Node node, PortData portData)
        {
            var orientation = portData.orientation == PortData.Orientation.Horizontal ? Orientation.Horizontal : Orientation.Vertical;
            var direction = portData.direction == PortData.Direction.Input ? Direction.Input : Direction.Output;
            var capacity = direction == Direction.Input ? Port.Capacity.Single : Port.Capacity.Multi;
            var container = direction == Direction.Input ? node.inputContainer : node.outputContainer;

            var port = Port.Create<UnityEditor.Experimental.GraphView.Edge>(orientation, direction, capacity, null);
            port.userData = portData;
            port.portName = portData.displayName;
            container.Add(port);
        }

        public static void RemovePort(this Node node, Port port)
        {
            var container = port.direction == Direction.Input ? node.inputContainer : node.outputContainer;
            container.Remove(port);
        }
    }
}
