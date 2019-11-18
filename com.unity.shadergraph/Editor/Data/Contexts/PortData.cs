using System;
using UnityEngine;
using UnityEditor.ShaderGraph.Serialization;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    sealed class PortData : JsonObject
    {
        public enum Orientation
        {
            Horizontal,
            Vertical,
        }

        public enum Direction
        {
            Input,
            Output,
        }

        [SerializeField]
        readonly string m_DisplayName;

        [SerializeField]
        readonly HlslTypeRef m_ShaderType;

        [SerializeField]
        readonly Orientation m_Orientation;

        [SerializeField]
        readonly Direction m_Direction;

        public PortData(string displayName, HlslTypeDescriptor type, Orientation orientation, Direction direction)
        {
            m_DisplayName = displayName;
            m_ShaderType = new HlslTypeRef(type);
            m_Orientation = orientation;
            m_Direction = direction;
        }

        public string displayName => m_DisplayName;

        public HlslTypeRef shaderType => m_ShaderType;

        public Orientation orientation => m_Orientation;

        public Direction direction => m_Direction;
    }
}
