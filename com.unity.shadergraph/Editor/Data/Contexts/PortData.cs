using System;
using UnityEngine;
using UnityEditor.ShaderGraph.Serialization;

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
        readonly TypeRef<IValueType> m_ValueType;

        [SerializeField]
        readonly Orientation m_Orientation;

        [SerializeField]
        readonly Direction m_Direction;

        public PortData(string displayName, Type type, Orientation orientation, Direction direction)
        {
            m_DisplayName = displayName;
            m_ValueType = new TypeRef<IValueType>(type);
            m_Orientation = orientation;
            m_Direction = direction;
        }

        public string displayName => m_DisplayName;

        public TypeRef<IValueType> valueType => m_ValueType;

        public Orientation orientation => m_Orientation;

        public Direction direction => m_Direction;
    }
}
