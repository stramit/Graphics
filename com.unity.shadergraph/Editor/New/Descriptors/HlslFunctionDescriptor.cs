using System;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    internal class HlslFunctionDescriptor
    {
        [SerializeField]
        private string m_Name;

        [SerializeField]
        private HlslSource m_Source;

        public string name 
        { 
            get => m_Name; 
            set => m_Name = value;
        }

        public HlslSource source 
        { 
            get => m_Source; 
            set => m_Source = value; 
        }

        public InputDescriptor[] inArguments { get; set; }
        public OutputDescriptor[] outArguments { get; set; }
    }
}
