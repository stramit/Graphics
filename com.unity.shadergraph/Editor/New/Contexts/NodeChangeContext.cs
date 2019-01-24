using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    internal struct NodeChangeContext
    {
        readonly AbstractMaterialGraph m_Graph;
        readonly int m_Id;
        readonly NodeTypeState m_TypeState;

        internal NodeChangeContext(AbstractMaterialGraph graph, int id, NodeTypeState typeState) : this()
        {
            m_Graph = graph;
            m_Id = id;
            m_TypeState = typeState;
        }

        internal AbstractMaterialGraph graph => m_Graph;

        internal int id => m_Id;

        internal NodeTypeState typeState => m_TypeState;

        public object GetData(NodeRef nodeRef)
        {
            Validate();

            return nodeRef.node.data;
        }

        // TODO: Decide whether this should be immediate
        // The issue could be that an exception is thrown mid-way, and then the node is left in a halfway broken state.
        // Maybe we can verify that it's valid immediately, but then postpone setting the value until the whole method
        // finishes.
        public void SetData(NodeRef nodeRef, object value)
        {
            Validate();

            var type = value.GetType();
            if (type.GetConstructor(Type.EmptyTypes) == null)
            {
                throw new ArgumentException($"{type.FullName} cannot be used as node data because it doesn't have a public, parameterless constructor.");
            }

            // TODO: Maybe do a proper check for whether the type is serializable?
            nodeRef.node.data = value;
        }

        public void SetHlslFunction(NodeRef nodeRef, HlslFunctionDescriptor functionDescriptor)
        {
            Validate();
            // TODO: Validation
            // Return value must be an output port
            // All output ports must be assigned exactly once
            // TODO: Copy input
            nodeRef.node.function = functionDescriptor;
            nodeRef.node.Dirty(ModificationScope.Graph);
        }

        internal void Validate()
        {
            if (m_Id != m_Graph.currentContextId)
            {
                throw new InvalidOperationException($"{nameof(NodeChangeContext)} is only valid during the {nameof(ShaderNodeType)} it was provided for.");
            }
        }
    }
}