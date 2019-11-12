using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    struct FieldRef : ISerializationCallbackReceiver
    {
        [SerializeField]
        string m_Tag;

        [SerializeField]
        string m_Name;

        [NonSerialized]
        FieldDescriptor m_Value;

        public FieldRef(FieldDescriptor value) : this()
        {
            m_Value = value;
        }

        public FieldDescriptor value => m_Value;

        public void OnBeforeSerialize()
        {
            if (value != null)
            {
                m_Tag = value.tag;
                m_Name = value.name;
            }
        }

        public void OnAfterDeserialize()
        {
            if (value == null)
            {
                m_Value = FieldRegistry.instance.Get(m_Tag, m_Name);
            }

            m_Tag = null;
            m_Name = null;
        }
    }
}
