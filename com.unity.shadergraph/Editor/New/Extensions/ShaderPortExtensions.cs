using System.Linq;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    internal static class ShaderPortExtensions
    {

#region Value
        internal static string InputValue(this ShaderPort port, IGraph graph, GenerationMode generationMode)
        {
            IEdge[] edges;
            if (port.HasEdges(out edges))
            {
                var fromSocketRef = edges[0].outputSlot;
                var fromNode = graph.GetNodeFromGuid<AbstractMaterialNode>(fromSocketRef.nodeGuid);
                if (fromNode == null)
                    return string.Empty;

                var slot = fromNode.FindOutputSlot<MaterialSlot>(fromSocketRef.slotId);
                if (slot == null)
                    return string.Empty;

                return ShaderGenerator.AdaptNodeOutput(fromNode, slot.id, port.concreteValueType);
            }

            return port.GetDefaultValue(generationMode);
        }
#endregion

#region Edges
        internal static bool HasEdges(this ShaderPort port)
        {
            var edges = port.owner.owner.GetEdges(port.slotReference);
            return edges.Any();
        }

        internal static bool HasEdges(this ShaderPort port, out IEdge[] edges)
        {
            edges = port.owner.owner.GetEdges(port.slotReference).ToArray();
            return edges.Any();
        }
#endregion

    }
}