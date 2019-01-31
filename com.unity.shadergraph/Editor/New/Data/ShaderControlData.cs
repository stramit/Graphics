using System;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{   
    [Serializable]
    struct ShaderControlData
    {
        [SerializeField]
        private string[] m_Labels;

        [SerializeField]
        private float[] m_Values;

        public string[] labels
        { 
            get => m_Labels != null ? m_Labels : new string[0];
            set => m_Labels = value;
        }

        public float[] values 
        { 
            get => m_Values != null ? m_Values : new float[0];
            set => m_Values = value;
        }
    }
}
