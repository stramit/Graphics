using System.Collections.Generic;

namespace UnityEditor.ShaderGraph.Internal
{
    public struct ShaderPass
    {
        public string name;
        public string templatePath;
        public IEnumerable<string> pragmas;
        public IEnumerable<string> includes;
        public IEnumerable<KeywordDescriptor> keywords;
    }
}
