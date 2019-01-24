using System;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{   
    [Serializable]
    public class ShaderControlData
    {
        [SerializeField]
        private string[] m_Labels = new string[0];

        [SerializeField]
        private float[] m_Values = new float[0];

        public string[] labels 
        { 
            get { return m_Labels; } 
            set { m_Labels = value; }
        }

        public float[] values 
        { 
            get { return m_Values; } 
            set { m_Values = value; }
        }
    }
}
