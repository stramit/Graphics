using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
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

    struct ShaderSnippetDescriptor : IDisposable
    {
        public ShaderSnippetRegistry registry { get; set; }

        public Guid source;
        public string identifier;

        public void Dispose()
        {
            registry.EndSnippet(this);
        }
    }

    class ShaderSnippetRegistry
    {
        Dictionary<string, ShaderSnippet> m_Snippets = new Dictionary<string, ShaderSnippet>();
        ShaderStringBuilder m_Builder = new ShaderStringBuilder();
        bool m_Validate = false;

        public Dictionary<string, ShaderSnippet> snippets => m_Snippets;
        public List<string> names { get; } = new List<string>();
        public bool allowDuplicates { get; set; }

        public ShaderSnippetRegistry(bool validate = false)
        {
            m_Validate = validate;
        }

        public ShaderSnippetDescriptor ProvideSnippet(string identifier, Guid source, out ShaderStringBuilder s)
        {
            s = m_Builder;
            
            return(new ShaderSnippetDescriptor()
            {
                registry = this,
                identifier = identifier,
                source = source,
            });
        }

        public void EndSnippet(ShaderSnippetDescriptor descriptor)
        {
            ShaderSnippet existingSnippet;
            if (!allowDuplicates && m_Snippets.TryGetValue(descriptor.identifier, out existingSnippet))
            {
                if (m_Validate)
                {
                    var snippet = ShaderSnippet.Build(descriptor.source, m_Builder.ToString());
                    if (!snippet.Equals(existingSnippet))
                        Debug.LogErrorFormat(@"Function `{0}` has varying implementations:{1}{1}{2}{1}{1}{3}", descriptor.identifier, Environment.NewLine, snippet, existingSnippet);
                }
            }
            else
            {
                var snippet = ShaderSnippet.Build(descriptor.source, m_Builder.ToString());
                m_Snippets.Add(descriptor.identifier, snippet);
            }

            names.Add(descriptor.identifier);
            m_Builder.Clear();
        }

        public string GetSnippetsAsString(bool appendNewLineBetweenSnippets = false)
        {
            m_Builder.Clear();
            foreach(KeyValuePair<string, ShaderSnippet> entry in snippets)
            {
                m_Builder.AppendLines(entry.Value.snippet);
                if(appendNewLineBetweenSnippets)
                    m_Builder.AppendNewLine();
            }
            return m_Builder.ToString();
        }
    }
}
