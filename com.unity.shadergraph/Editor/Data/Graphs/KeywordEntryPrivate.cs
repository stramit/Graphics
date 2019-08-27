using System;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    struct KeywordEntryPrivate
    {
        public int id;
        public string displayName;
        public string referenceName;

        public KeywordEntryPrivate(int id, string displayName, string referenceName)
        {
            this.id = id;
            this.displayName = displayName;
            this.referenceName = referenceName;
        }
    }
}
