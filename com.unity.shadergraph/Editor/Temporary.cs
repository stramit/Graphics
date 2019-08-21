using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    class Splice : IDisposable
    {
        ShaderStringBuilder m_Builder;
        ShaderStringBuilder m_Destination;
        string m_Token;

        public ShaderStringBuilder builder => m_Builder;

        public Splice(string token, ShaderStringBuilder destination)
        {
            m_Builder = new ShaderStringBuilder();
            m_Destination = destination;
            m_Token = token;
        }

        public void Dispose()
        {
            SpliceToken(m_Builder, m_Destination, m_Token);
        }

        public void SpliceToken(ShaderStringBuilder source, ShaderStringBuilder destination, string token)
        {
            var result = source.ToString();
            if(string.IsNullOrEmpty(result))
            {
                result = "// <None>";
            }

            destination.Replace($"${{{token}}}", result);
        }
    }

    static class KeywordUtil
    {
        public static string ToDeclarationString(this KeywordDefinition keywordDefinition)
        {
            switch(keywordDefinition)
            {
                case KeywordDefinition.MultiCompile:
                    return "multi_compile";
                case KeywordDefinition.ShaderFeature:
                    return "shader_feature";
                default:
                    return string.Empty;
            }
        }

        public static string ToDeclarationString(this KeywordDescriptor keyword)
        {
            // Get definition type using scope
            string scopeString = keyword.scope == KeywordScope.Local ? "_local" : string.Empty;
            string definitionString = $"{keyword.definition.ToDeclarationString()}{scopeString}";

            switch(keyword.type)
            {
                case KeywordType.Boolean:
                    return $"#pragma {definitionString} _ {keyword.referenceName}";
                case KeywordType.Enum:
                    var enumEntryDefinitions = keyword.entries.Select(x => $"{keyword.referenceName}_{x.referenceName}");
                    string enumEntriesString = string.Join(" ", enumEntryDefinitions);
                    return $"#pragma {definitionString} {enumEntriesString}";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

namespace UnityEditor.ShaderGraph.Internal
{
    [Serializable]
    public struct KeywordDescriptor
    {
        public string displayName;
        public string referenceName;
        public KeywordType type;
        public KeywordDefinition definition;
        public KeywordScope scope;
        public IEnumerable<KeywordEntry> entries;
    }

    public struct KeywordEntry
    {
        public string displayName;
        public string referenceName;
    }

    public enum KeywordType { Boolean, Enum }
    public enum KeywordDefinition { ShaderFeature, MultiCompile };
    public enum KeywordScope { Global, Local };
}
