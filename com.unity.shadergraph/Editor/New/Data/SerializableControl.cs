using System;
using System.Reflection;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    struct SerializableControl
    {
        [SerializeField]
        private string m_AssemblyName;

        [SerializeField]
        private string m_TypeName;

        [SerializeField]
        private ShaderControlData m_ControlData;

        public IShaderControl control
        {
            get
            {
                IShaderControl control;
                if(!string.IsNullOrEmpty(m_AssemblyName) && !string.IsNullOrEmpty(m_TypeName))
                    control = (IShaderControl)Activator.CreateInstance(m_AssemblyName, m_TypeName).Unwrap();
                else
                    control = new DefaultControl();
                control.controlData = m_ControlData;
                return control;
            }
            set
            {
                m_AssemblyName = value.GetType().Assembly.FullName;
                m_TypeName = value.GetType().FullName;
                m_ControlData = value.controlData;
            }
        }
    }
}