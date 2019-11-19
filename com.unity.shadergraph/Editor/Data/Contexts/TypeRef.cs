using System;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    struct TypeRef<T> : ISerializationCallbackReceiver
    {
        [SerializeField]
        string m_FullName;

        [NonSerialized]
        Type m_Type;

        [NonSerialized]
        T m_Instance;

        public TypeRef(Type value) : this()
        {
            m_Type = value;
            m_Instance = (T)Activator.CreateInstance(value);
        }

        public Type type => m_Type;
        public T instance => m_Instance;

        public void OnBeforeSerialize()
        {
            if (type != null)
            {
                m_FullName = type.FullName;
            }
        }

        public void OnAfterDeserialize()
        {
            if (type == null)
            {
                TypeCache.TypeCollection collection = TypeCache.GetTypesDerivedFrom<T>();
                foreach(var type in collection)
                {
                    if(type.FullName.Equals(m_FullName))
                    {
                        m_Type = type;
                        m_Instance = (T)Activator.CreateInstance(type);
                    }
                }
            }

            m_FullName = null;
        }
    }
}
