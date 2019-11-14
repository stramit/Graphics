using System;
using UnityEngine;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    sealed class FieldBlockData : BlockData
    {
        [SerializeField]
        readonly FieldRef m_Field;

        public FieldBlockData(FieldDescriptor descriptor)
        {
            m_Field = new FieldRef(descriptor);
            
            inputPorts.Add(new PortData(descriptor.fullName, typeof(TestType), PortData.Orientation.Horizontal, PortData.Direction.Input));
        }

        public FieldDescriptor field => m_Field.value;
    }
}
