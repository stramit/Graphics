using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    struct ShaderSnippet
    {
        public Guid source;
        public string snippet;

        public static ShaderSnippet Build(Guid source, string snippet)
        {
            return new ShaderSnippet()
            {
                source = source,
                snippet = snippet,
            };
        }

        public override bool Equals(object obj)
        {
            var other = (ShaderSnippet)obj;
            return this.source == other.source
                && this.snippet == other.snippet;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    struct ShaderSnippetDescriptor
    {
        public Guid source;
        public string identifier;
        public Action<ShaderStringBuilder> builder;
    }

    class ShaderSnippetRegistry
    {
        Dictionary<string, ShaderSnippet> m_Snippets = new Dictionary<string, ShaderSnippet>();
        ShaderStringBuilder m_Builder = new ShaderStringBuilder();
        bool m_Validate = false;

        public Dictionary<string, ShaderSnippet> snippets => m_Snippets;
        public List<string> names { get; } = new List<string>();

        public ShaderSnippetRegistry(bool validate = false)
        {
            m_Validate = validate;
        }

        public void ProvideSnippet(ShaderSnippetDescriptor descriptor)
        {
            m_Builder.Clear();
            ShaderSnippet existingSnippet;
            if (m_Snippets.TryGetValue(descriptor.identifier, out existingSnippet))
            {
                if (m_Validate)
                {
                    descriptor.builder(m_Builder);
                    var snippet = ShaderSnippet.Build(descriptor.source, m_Builder.ToString());
                    if (!snippet.Equals(existingSnippet))
                        Debug.LogErrorFormat(@"Function `{0}` has varying implementations:{1}{1}{2}{1}{1}{3}", descriptor.identifier, Environment.NewLine, snippet, existingSnippet);
                }
            }
            else
            {
                descriptor.builder(m_Builder);
                var snippet = ShaderSnippet.Build(descriptor.source, m_Builder.ToString());
                m_Snippets.Add(descriptor.identifier, snippet);
            }

            names.Add(descriptor.identifier);
        }

        public string[] GetSnippets()
        {
            return snippets.Select(s => s.Value.snippet).ToArray();
        }

        public string[] GetUniqueSnippets()
        {
            Dictionary<string, string> snippetStrings = new Dictionary<string, string>();
            foreach(KeyValuePair<string, ShaderSnippet> entry in snippets)
            {
                ShaderSnippet snippet = entry.Value;
                if(!snippetStrings.ContainsKey(entry.Key))
                    snippetStrings.Add(entry.Key, snippet.snippet);
            }

            return snippetStrings.Select(s => s.Value).ToArray();
        }
    }
}
