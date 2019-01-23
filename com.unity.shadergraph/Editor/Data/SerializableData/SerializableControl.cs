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

        public SerializableControl(IShaderControl control)
        {
            m_AssemblyName = control.GetType().Assembly.FullName;
            m_TypeName = control.GetType().FullName;
        }

        public IShaderControl Deserialize()
        {
            return (IShaderControl)Activator.CreateInstance(m_AssemblyName, m_TypeName).Unwrap();
        }
    }
}
