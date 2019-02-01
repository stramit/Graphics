using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEditor.ShaderGraph
{
    public struct NodeSetupContext
    {
        readonly AbstractMaterialGraph m_Graph;
        readonly int m_CurrentSetupContextId;
        readonly NodeTypeState m_TypeState;

        bool m_NodeTypeCreated;

        internal bool nodeTypeCreated => m_NodeTypeCreated;

        internal NodeSetupContext(AbstractMaterialGraph graph, int currentSetupContextId, NodeTypeState typeState)
        {
            m_Graph = graph;
            m_CurrentSetupContextId = currentSetupContextId;
            m_TypeState = typeState;
            m_NodeTypeCreated = false;
        }

		// TODO: make NodeTypeDescriptor internal, just pass values here
        internal void CreateNodeType(NodeTypeDescriptor typeDescriptor)
        {
            Validate();

            // Before doing anything, we perform validation on the provided NodeTypeDescriptor.

            // We might allow multiple types later on, or maybe it will go via another API point. For now, we only allow
            // a single node type to be provided.
            if (m_NodeTypeCreated)
            {
                throw new InvalidOperationException($"An {nameof(ShaderNodeType)} can only have 1 type.");
            }

            foreach (InputDescriptor port in typeDescriptor.inPorts)
            {
                if (m_TypeState.inputPorts.Any(x => x.id == port.id) || m_TypeState.outputPorts.Any(x => x.id == port.id) || m_TypeState.parameters.Any(x => x.id == port.id))
                    throw new ArgumentException($"A Port or Parameter with id {port.id} already exists.", nameof(port));

                m_TypeState.inputPorts.Add(port);
            }

            foreach (OutputDescriptor port in typeDescriptor.outPorts)
            {
                if (m_TypeState.inputPorts.Any(x => x.id == port.id) || m_TypeState.outputPorts.Any(x => x.id == port.id) || m_TypeState.parameters.Any(x => x.id == port.id))
                    throw new ArgumentException($"A Port or Parameter with id {port.id} already exists.", nameof(port));

                m_TypeState.outputPorts.Add(port);
            }

            foreach (InputDescriptor parameter in typeDescriptor.parameters)
            {
                if (m_TypeState.inputPorts.Any(x => x.id == parameter.id) || m_TypeState.outputPorts.Any(x => x.id == parameter.id) || m_TypeState.parameters.Any(x => x.id == parameter.id))
                    throw new ArgumentException($"A Port or Parameter with id {parameter.id} already exists.", nameof(parameter));

                m_TypeState.parameters.Add(parameter);
            }

            m_TypeState.type = typeDescriptor;
            m_TypeState.type.name = typeDescriptor.name;
            // Provide auto-generated name if one is not provided.
            if (string.IsNullOrWhiteSpace(m_TypeState.type.name))
            {
                m_TypeState.type.name = m_TypeState.GetType().Name;

                // Strip "Node" from the end of the name. We also make sure that we don't strip it to an empty string,
                // in case someone decided that `Node` was a good name for a class.
                const string nodeSuffix = "Node";
                if (m_TypeState.type.name.Length > nodeSuffix.Length && m_TypeState.type.name.EndsWith(nodeSuffix))
                    m_TypeState.type.name = m_TypeState.type.name.Substring(0, m_TypeState.type.name.Length - nodeSuffix.Length);
            }

            m_TypeState.type.path = typeDescriptor.path;
            // Don't want nodes showing up at the root and cluttering everything.
            if (string.IsNullOrWhiteSpace(m_TypeState.type.path))
                m_TypeState.type.path = "Uncategorized";

            // copy port and control lists
            /*m_TypeState.type.inputs = new List<InputPort>(typeDescriptor.inputs);
            m_TypeState.type.outputs = new List<OutputPort>(typeDescriptor.outputs);
            if (typeDescriptor.controls != null)
                m_TypeState.type.controls = new List<Control>(typeDescriptor.controls);
            else
                m_TypeState.type.controls = new List<Control>();*/

            m_NodeTypeCreated = true;
        }

        void Validate()
        {
            if (m_CurrentSetupContextId != m_Graph.currentContextId)
            {
                throw new InvalidOperationException($"{nameof(NodeSetupContext)} is only valid during the call to {nameof(ShaderNodeType)}.{nameof(ShaderNodeType.Setup)} it was provided for.");
            }
        }
    }
}
