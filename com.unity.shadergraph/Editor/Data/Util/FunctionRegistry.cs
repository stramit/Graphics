using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    struct ShaderSnippet
    {
        public string identifier;
        public string snippet;

        public static ShaderSnippet Build(string name, string snippet)
        {
            return new ShaderSnippet()
            {
                identifier = name,
                snippet = snippet,
            };
        }

        public override bool Equals(object obj)
        {
            var other = (ShaderSnippet)obj;
            return this.identifier == other.identifier
                && this.snippet == other.snippet;
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
        List<KeyValuePair<Guid, ShaderSnippet>> m_Snippets = new List<KeyValuePair<Guid, ShaderSnippet>>();
        ShaderStringBuilder m_Builder = new ShaderStringBuilder();
        bool m_Validate = false;

        public List<KeyValuePair<Guid, ShaderSnippet>> snippets => m_Snippets;
        public List<string> names { get; } = new List<string>();

        public ShaderSnippetRegistry(bool validate = false)
        {
            m_Validate = validate;
        }

        public void ProvideSnippet(ShaderSnippetDescriptor descriptor)
        {
            m_Builder.Clear();
            // ShaderSnippet existingSnippet;
            // if (m_Sources.TryGetValue(descriptor.source, out existingSnippet))
            // {
            //     if (m_Validate)
            //     {
            //         descriptor.builder(m_Builder);
            //         var snippet = ShaderSnippet.Build(descriptor.name, m_Builder.ToString());
            //         if (!snippet.Equals(existingSnippet))
            //             Debug.LogErrorFormat(@"Function `{0}` has varying implementations:{1}{1}{2}{1}{1}{3}", name, Environment.NewLine, source, existingSnippet);
            //     }
            // }
            // else
            // {
                descriptor.builder(m_Builder);
                var snippet = ShaderSnippet.Build(descriptor.identifier, m_Builder.ToString());
                m_Snippets.Add(new KeyValuePair<Guid, ShaderSnippet>(descriptor.source, snippet));
            // }

            names.Add(descriptor.identifier);
        }

        public string[] GetSnippets()
        {
            List<string> snippetStrings = new List<string>();
            foreach(KeyValuePair<Guid, ShaderSnippet> entry in snippets)
            {
                snippetStrings.Add(entry.Value.snippet);
            }

            return snippetStrings.ToArray();
        }

        public string[] GetUniqueSnippets()
        {
            Dictionary<string, string> snippetStrings = new Dictionary<string, string>();
            foreach(KeyValuePair<Guid, ShaderSnippet> entry in snippets)
            {
                ShaderSnippet snippet = entry.Value;
                if(!snippetStrings.ContainsKey(snippet.identifier))
                    snippetStrings.Add(snippet.identifier, snippet.snippet);
            }

            return snippetStrings.Select(s => s.Value).ToArray();
        }

        public bool ContainsIdentifier(string identifier)
        {
            foreach(KeyValuePair<Guid, ShaderSnippet> entry in snippets)
            {
                if(entry.Value.identifier == identifier)
                    return true;
            }
            
            return false;
        }
    }
}
