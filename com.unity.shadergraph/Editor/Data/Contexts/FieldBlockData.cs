using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    sealed class FieldBlockData : BlockData
    {
        [SerializeField]
        FieldRef m_Field;

        public FieldDescriptor field
        {
            get => m_Field.value;
            set => m_Field = new FieldRef(value);
        }
    }
}
