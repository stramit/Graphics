using System;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    class EdgeData : IComparable<EdgeData>
    {
        [SerializeField]
        JsonRef<PortData> m_Input;

        [SerializeField]
        JsonRef<PortData> m_Output;

        public EdgeData() { }

        public EdgeData(PortData input, PortData output)
        {
            m_Input = input;
            m_Output = output;
        }

        public PortData input => m_Input;
        public PortData output => m_Output; 

        protected bool Equals(EdgeData other)
        {
            return Equals(input, other.input) && Equals(output, other.output);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EdgeData)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((output != null ? output.GetHashCode() : 0) * 397) ^ (input != null ? input.GetHashCode() : 0);
            }
        }

        public int CompareTo(EdgeData other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var outputSlotComparison = m_Output.target.jsonId.CompareTo(other.m_Output.target.jsonId);
            if (outputSlotComparison != 0) return outputSlotComparison;
            return m_Input.target.jsonId.CompareTo(other.m_Input.target.jsonId);
        }
    }
}
