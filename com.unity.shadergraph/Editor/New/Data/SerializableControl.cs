using System;
using System.Reflection;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    internal class SerializableControl
    {
        [SerializeField]
        private string m_AssemblyName;

        [SerializeField]
        private string m_TypeName;

        [SerializeField]
        private string[] m_Labels;

        [SerializeField]
        private float[] m_Values;

        public SerializableControl(IShaderControl control)
        {
            m_AssemblyName = control.GetType().Assembly.FullName;
            m_TypeName = control.GetType().FullName;
            m_Labels = control.labels;
            m_Values = control.values;
        }

        public IShaderControl Deserialize()
        {
            var control = (IShaderControl)Activator.CreateInstance(m_AssemblyName, m_TypeName).Unwrap();
            control.labels = m_Labels;
            control.values = m_Values;
            return control;
        }
    }
}
