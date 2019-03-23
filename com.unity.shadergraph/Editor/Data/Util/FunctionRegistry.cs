using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    struct FunctionSource
    {
        public Precision precision;
        public string function;
    }

    class FunctionRegistry
    {
        Dictionary<string, FunctionSource> m_Sources = new Dictionary<string, FunctionSource>();
        bool m_Validate = false;
        ShaderStringBuilder m_Builder;

        public FunctionRegistry(bool validate = false)
        {
            m_Builder = new ShaderStringBuilder();
            m_Validate = validate;
        }

        internal ShaderStringBuilder builder => m_Builder;

        public Dictionary<string, FunctionSource> sources => m_Sources;
        
        public List<string> names { get; } = new List<string>();

        public void ProvideFunction(string name, Precision precision, Action<ShaderStringBuilder> generator)
        {
            FunctionSource existingSource;
            if (m_Sources.TryGetValue(name, out existingSource))
            {
                if (m_Validate)
                {
                    var startIndex = builder.length;
                    generator(builder);
                    var length = builder.length - startIndex;
                    var source = builder.ToString(startIndex, length);
                    builder.length -= length;
                    if (source != existingSource.function)
                        Debug.LogErrorFormat(@"Function `{0}` has varying implementations:{1}{1}{2}{1}{1}{3}", name, Environment.NewLine, source, existingSource);
                }
            }
            else
            {
                builder.AppendNewLine();
                var startIndex = builder.length;
                generator(builder);
                var length = builder.length - startIndex;
                var source = builder.ToString(startIndex, length);
                m_Sources.Add(name, new FunctionSource() {precision = precision, function = source});
            }

            names.Add(name);
        }
    }
}
