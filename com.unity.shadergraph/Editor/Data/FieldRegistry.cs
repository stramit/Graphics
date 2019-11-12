using System.Collections.Generic;
using System.Reflection;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    class FieldRegistry
    {
        public static FieldRegistry instance { get; } = new FieldRegistry();

        static FieldRegistry() {}

        FieldRegistry()
        {
            foreach (var type in TypeCache.GetTypesWithAttribute<FieldsAttribute>())
            {
                foreach (var fieldInfo in type.GetFields(BindingFlags.Static | BindingFlags.Public))
                {
                    if (fieldInfo.FieldType != typeof(FieldDescriptor) || fieldInfo.GetCustomAttribute<NoBlockAttribute>() != null)
                    {
                        continue;
                    }

                    var fieldDescriptor = (FieldDescriptor)fieldInfo.GetValue(null);
                    m_Descriptors.Add((fieldDescriptor.tag, fieldDescriptor.name), fieldDescriptor);
                }
            }
        }

        Dictionary<(string, string), FieldDescriptor> m_Descriptors = new Dictionary<(string, string), FieldDescriptor>();

        public FieldDescriptor Get(string tag, string name) => m_Descriptors[(tag, name)];
    }
}
