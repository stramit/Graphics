using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    class FieldRegistry
    {
        public static FieldRegistry instance { get; } = new FieldRegistry();

        Dictionary<(string, string), FieldDescriptor> m_Descriptors = new Dictionary<(string, string), FieldDescriptor>();

        public void Add(FieldDescriptor fieldDescriptor)
        {
            m_Descriptors[(fieldDescriptor.tag, fieldDescriptor.name)] = fieldDescriptor;
        }

        public FieldDescriptor Get(string tag, string name) => m_Descriptors[(tag, name)];
    }
}
