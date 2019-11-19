using System;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    class BlockData : JsonObject
    {
        [SerializeField]
        string m_DisplayName;

        public string displayName
        {
            get => m_DisplayName;
            set => m_DisplayName = value;
        }
    }
}
