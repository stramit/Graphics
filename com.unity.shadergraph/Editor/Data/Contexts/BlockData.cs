using System;
using UnityEngine;
using UnityEditor.ShaderGraph.Serialization;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    class BlockData : JsonObject
    {
        [SerializeField]
        string m_DisplayName;

        [SerializeField]
        JsonList<PortData> m_InputPorts = new JsonList<PortData>();

        [SerializeField]
        JsonList<PortData> m_OutputPorts = new JsonList<PortData>();

        public string displayName
        {
            get => m_DisplayName;
            set => m_DisplayName = value;
        }

        public JsonList<PortData> inputPorts
        {
            get => m_InputPorts;
            set => m_InputPorts = value;
        }

        public JsonList<PortData> outputPorts
        {
            get => m_OutputPorts;
            set => m_OutputPorts = value;
        }
    }
}
