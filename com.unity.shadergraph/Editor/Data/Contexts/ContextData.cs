using System;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    class ContextData : JsonObject
    {
        [SerializeField]
        ShaderStage m_Stage;

        [SerializeField]
        JsonList<BlockData> m_Blocks = new JsonList<BlockData>();

        [SerializeField]
        Vector2 m_Position;

        public ShaderStage stage
        {
            get => m_Stage;
            set => m_Stage = value;
        }

        public List<BlockData> blocks => m_Blocks;

        public Vector2 position
        {
            get => m_Position;
            set => m_Position = value;
        }
    }
}
