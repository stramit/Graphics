using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    public struct HlslTypeRef : ISerializationCallbackReceiver
    {
        [SerializeField]
        string m_TypeName;

        [SerializeField]
        string m_Name;

        [NonSerialized]
        HlslTypeDescriptor m_Value;

        public HlslTypeRef(HlslTypeDescriptor value) : this()
        {
            m_Value = value;
        }

        public HlslTypeDescriptor value => m_Value;

        public void OnBeforeSerialize()
        {
            if (value != null)
            {
                (m_TypeName, m_Name) = HlslTypeRegistry.instance.Get(value);
            }
        }

        public void OnAfterDeserialize()
        {
            if (value == null)
            {
                m_Value = HlslTypeRegistry.instance.Get(m_TypeName, m_Name);
            }

            m_TypeName = null;
            m_Name = null;
        }
    }
}
