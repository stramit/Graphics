namespace UnityEditor.ShaderGraph.Internal
{
    public struct KeywordEntry
    {
        public string displayName;
        public string referenceName;

        public KeywordEntry(string displayName, string referenceName)
        {
            this.displayName = displayName;
            this.referenceName = referenceName;
        }
    }
}
