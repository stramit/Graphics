using System.Collections.Generic;
using System.Reflection;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    class HlslTypeRegistry
    {
        public static HlslTypeRegistry instance { get; } = new HlslTypeRegistry();

        Dictionary<HlslTypeDescriptor, (string, string)> m_Keys = new Dictionary<HlslTypeDescriptor, (string, string)>();
        
        Dictionary<(string, string), HlslTypeDescriptor> m_Values = new Dictionary<(string, string), HlslTypeDescriptor>(); 

        static HlslTypeRegistry() {}

        HlslTypeRegistry()
        {
            foreach (var type in TypeCache.GetTypesWithAttribute<TypesAttribute>())
            {
                foreach (var fieldInfo in type.GetFields(BindingFlags.Static | BindingFlags.Public))
                {
                    if (fieldInfo.FieldType != typeof(HlslTypeDescriptor))
                    {
                        continue;
                    }

                    var typeDescriptor = (HlslTypeDescriptor)fieldInfo.GetValue(null);
                    m_Values.Add((type.FullName, fieldInfo.Name), typeDescriptor);
                }
            }
        }

        public IEnumerable<(string, string)> keys => m_Keys.Values;

        public IEnumerable<HlslTypeDescriptor> values => m_Values.Values;

        public HlslTypeDescriptor Get(string tag, string name) => m_Values[(tag, name)];

        public (string, string) Get(HlslTypeDescriptor descriptor) => m_Keys[descriptor];
    }
}
